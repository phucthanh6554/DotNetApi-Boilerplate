using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator2.Services
{
    internal class GenRepoWrapperService
    {
        private readonly SyntaxHelper _helper = new SyntaxHelper();

        private const string Template = $@"
            using Microsoft.EntityFrameworkCore;

            namespace @namespace
            {{
                public class RepositoryWrapper : IRepositoryWrapper<GenerateDbContext>
                {{
                    public GenerateDbContext dbContext {{ get; set; }}

                    public RepositoryWrapper(GenerateDbContext _dbContext)
                    {{
                        dbContext = _dbContext;
                    }}

                    @listRepository
                }}
            }}
";

        public string GenRepoWrapper(GeneratorExecutionContext context)
        {
            string source = Template;

            var mainNamespace = context.Compilation.AssemblyName;

            source = source.Replace("@namespace", mainNamespace);

            var listModel = _helper.GetListDbModel(context.Compilation.SyntaxTrees.ToList());
            source = source.Replace("@listRepository", BuildListRepository(listModel));

            return source;
        }

        public string BuildListRepository(List<ClassDeclarationSyntax> modelTypes)
        {
            string result = string.Empty;

            string repositoryTemplate = @"public @modelRepository @model {get {
                return new @modelRepository(dbContext);
            }}
";

            modelTypes.ForEach(x => result += repositoryTemplate.Replace("@model", x.Identifier.ToString()));

            return result;
        }
    }
}
