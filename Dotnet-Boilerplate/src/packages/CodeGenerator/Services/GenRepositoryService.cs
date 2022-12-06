using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Services
{
    internal sealed class GenRepositoryService
    {
        private readonly SyntaxHelper _helper = new SyntaxHelper();

        private const string Template = $@"
        using Microsoft.EntityFrameworkCore;
        using Repository.Implements;

        namespace @namespace
        {{
            public sealed partial class @modelNameRepository : Repository<@modelNameRepository>
            {{
                public @modelNameRepository(DbContext dbContext) : base(dbContext)
                {{
                }}
            }}
        }}
";

        public string GenRepository(GeneratorExecutionContext context ,ClassDeclarationSyntax modelType)
        {
            var source = Template;

            var mainNamespace = context.Compilation.AssemblyName;

            source = source.Replace("@namespace", mainNamespace);

            source = source.Replace("@modelName", modelType.Identifier.ToString());

            return source;
        }
    }
}
