
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class DbContextGenerator : ISourceGenerator
{

    public void Execute(GeneratorExecutionContext context)
    {
        var helper = new SyntaxHelper();
        var syntaxTrees = context.Compilation.SyntaxTrees;

        string source = string.Empty;
        var mainNamespace = context.Compilation.GetEntryPoint(context.CancellationToken).ContainingNamespace.ToDisplayString();
        
        foreach (var syntaxTree in syntaxTrees)
        {
            var dbModelTypes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(x => x.AttributeLists.Any(y => y.ToString().StartsWith("[DbModel")))
                .ToList();

            if(dbModelTypes.Count == 0)
                continue;

            source = $@"
using Microsoft.EntityFrameworkCore;

namespace @namespace{{
    public partial class GenerateDbContext : DbContext{{
        public GenerateDbContext(DbContextOptions options) : base(options){{

        }}";
            var listModelNamespaces = new List<string>();
            var currentNamespace = string.Empty;
            foreach (var model in dbModelTypes)
            {
                source += $@"public DbSet<{model.Identifier}> {model.Identifier} {{get;set;}}" + '\n';

                var namespaceSyntaxFound = helper.GetParentSyntax<NamespaceDeclarationSyntax>(model, out var namespaceDeclaration);

                if(namespaceSyntaxFound && namespaceDeclaration.Name.ToString() != currentNamespace){
                    currentNamespace = namespaceDeclaration.Name.ToString();
                    listModelNamespaces.Add(currentNamespace);
                }
            }

            string namespaceStr = string.Empty;

            foreach(var ns in listModelNamespaces){
                namespaceStr += $"using {ns}; \n";
            }

            source = namespaceStr + source + '\n';
            
            source += "}}";

            if(listModelNamespaces.Count > 0) {
                // Get first namespace
                var firstNamespace = listModelNamespaces.First();

                var projectNamespace = firstNamespace.Split('.')[0];

                source = source.Replace("@namespace", projectNamespace);
            }

            if(!string.IsNullOrEmpty(source))
                context.AddSource("GenerateDbContext.g.cs", source);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        
    }
}
