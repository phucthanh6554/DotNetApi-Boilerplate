using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CodeGenerator
{
    [Generator]
    public class RepositoryGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxHelper = new SyntaxHelper();
            var syntaxTrees = context.Compilation.SyntaxTrees;

            var mainNamespace = context.Compilation.GetEntryPoint(context.CancellationToken).ContainingNamespace.ToDisplayString();

            Console.WriteLine(mainNamespace);

            foreach (var syntaxTree in syntaxTrees)
            {
                var types = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .Where(x => x.AttributeLists.Any(y => y.ToString().StartsWith("[DbModel")))
                    .ToList();

                foreach (var model in types)
                {
                    syntaxHelper.GetParentSyntax<NamespaceDeclarationSyntax>(model, out var namespaceSyntax);

                    var source = $@"
using System;
using Repository.Implements;
using {namespaceSyntax.Name};
using Microsoft.EntityFrameworkCore;

namespace {namespaceSyntax.Name.ToString().Split('.')[0]}
{{
    public class {model.Identifier}Repository : Repository<{model.Identifier}> {{
        public {model.Identifier}Repository(DbContext context) : base(context){{
            /////////
        }}
    }}
}}";
                    context.AddSource($"{model.Identifier}Repo.g.cs", source);
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
