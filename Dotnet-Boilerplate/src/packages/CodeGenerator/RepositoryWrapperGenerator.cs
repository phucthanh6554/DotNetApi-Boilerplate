using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class RepositoryWrapperGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var syntaxTrees = context.Compilation.SyntaxTrees;

        var helper = new SyntaxHelper();

        string source = string.Empty;
        foreach (var syntax in syntaxTrees)
        {
            var modelTypes = syntax.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(x => x.AttributeLists.Any(y => y.ToString().StartsWith("[DbModel")))
                .ToList();

            if(modelTypes.Count == 0)
                continue;

            source = $@"
using Microsoft.EntityFrameworkCore;
using @namespace;

namespace @namespace{{
    public class RepositoryWrapper : IRepositoryWrapper<GenerateDbContext>{{
    public GenerateDbContext dbContext {{get; set;}}

    public RepositoryWrapper(GenerateDbContext context)
    {{
        dbContext = context;
    }}
";

            var listModelNamespaces = new List<string>();
            var currentNamespace = string.Empty;
            foreach (var model in modelTypes)
            {
                source += $@"public {model.Identifier}Repository _{model.Identifier} {{
                    get {{
                        return new {model.Identifier}Repository(dbContext);
                    }}
                }}" + '\n';

                var namespaceSyntaxFound = helper.GetParentSyntax<NamespaceDeclarationSyntax>(model, out var namespaceDeclaration);

                if(namespaceSyntaxFound && namespaceDeclaration.Name.ToString() != currentNamespace){
                    currentNamespace = namespaceDeclaration.Name.ToString();
                    listModelNamespaces.Add(currentNamespace);
                }
            }

            if(listModelNamespaces.Count > 0) {
                // Get first namespace
                var firstNamespace = listModelNamespaces.First();

                var projectNamespace = firstNamespace.Split('.')[0];

                source = source.Replace("@namespace", projectNamespace);
            }

            source += "}}";
        }

        if(!string.IsNullOrEmpty(source))
            context.AddSource("GenerateRepoWrapper.g.cs", source);
    }

    public void Initialize(GeneratorInitializationContext context)
    {

    }
}