
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
        var syntaxTrees = context.Compilation.SyntaxTrees;

        string source = string.Empty;
        foreach (var syntaxTree in syntaxTrees)
        {
            var dbModelTypes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(x => x.AttributeLists.Any(y => y.ToString().StartsWith("[DbModel")))
                .ToList();

            if(dbModelTypes.Count == 0)
                continue;

            source = $@"
using Microsoft.EntityFrameworkCore;

public partial class GenerateDbContext : DbContext{{
    public GenerateDbContext(DbContextOptions options) : base(options){{

    }}
";
            var listModelNamespaces = new List<string>();
            var currentNamespace = string.Empty;
            foreach (var model in dbModelTypes)
            {
                source += $@"public DbSet<{model.Identifier}> {model.Identifier} {{get;set;}}" + '\n';

                var namespaceSyntaxFound = GetParentSyntax<NamespaceDeclarationSyntax>(model, out var namespaceDeclaration);

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
            
            source += "}";

            Console.WriteLine(source);

        }
        context.AddSource("GenerateDbContext.g.cs", source);
    }

    public void Initialize(GeneratorInitializationContext context)
    {

    }

    private bool GetParentSyntax<T>(SyntaxNode node, out T result) where T : SyntaxNode
    {
        result = null;

        if (node == null)
            return false;

        var currentNode = node;

        while (currentNode != null)
        {
            currentNode = currentNode.Parent;

            if (currentNode is T)
            {
                result = currentNode as T;
                return true;
            }
        }

        return false;
    }
}
