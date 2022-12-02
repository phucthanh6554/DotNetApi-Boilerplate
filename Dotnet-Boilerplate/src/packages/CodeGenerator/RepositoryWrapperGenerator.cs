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
    public class RepositoryWrapperGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;

            foreach (var syntaxTree in syntaxTrees)
            {
                var types = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .Where(x => x.AttributeLists.Any(y => y.ToString().StartsWith("[DbModel")))
                    .ToList();

                foreach (var model in types)
                {
                    GetParentSyntax<NamespaceDeclarationSyntax>(model, out var namespaceSyntax);

                    var source = $@"
using System;
using Repository.Implements;
using {namespaceSyntax.Name};
using Microsoft.EntityFrameworkCore;

public class {model.Identifier}Repository : Repository<{model.Identifier}> {{
    public {model.Identifier}Repository(DbContext context) : base(context){{
        /////////
    }}
}}
";

                    context.AddSource($"{model.Identifier}Repo.g.cs", source);
                }
            }
        }

        private bool GetParentSyntax<T>(SyntaxNode node, out T result) where T : SyntaxNode
        {
            result = null;

            if (node == null)
                return false;

            var currentNode = node;

            while(currentNode != null)
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

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
