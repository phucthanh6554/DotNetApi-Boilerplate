using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

public class SyntaxHelper{
    public bool GetParentSyntax<T>(SyntaxNode node, out T result) where T : SyntaxNode
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

    public List<string> GetListNamespace(List<ClassDeclarationSyntax> classDeclarationSyntaxes)
    {
        var result = new List<string>();

        foreach(var classDeclaration in classDeclarationSyntaxes)
        {
            var foundNamespace = GetParentSyntax<NamespaceDeclarationSyntax>(classDeclaration, out var namespaceNode);

            if (foundNamespace)
            {
                result.Add(namespaceNode.Name.ToString());
            }
        }

        return result.Distinct().ToList();
    }

    public List<ClassDeclarationSyntax> GetListDbModel(List<SyntaxTree> syntaxTrees)
    {
        var result = new List<ClassDeclarationSyntax>();

        foreach (var syntaxTree in syntaxTrees)
        {
            var dbModelTypes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(x => x.AttributeLists.Any(y => y.ToString().StartsWith("[DbModel")))
                .ToList();

            if (dbModelTypes.Any())
                result.AddRange(dbModelTypes);
        }

        return result;
    }
}