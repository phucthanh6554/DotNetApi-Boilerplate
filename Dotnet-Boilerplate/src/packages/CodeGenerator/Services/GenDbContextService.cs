using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator.Services
{
    internal sealed class GenDbContextService
    {
        private readonly SyntaxHelper _helper = new SyntaxHelper();

        private const string Template = $@"
        using Microsoft.EntityFrameworkCore;
        @listUsing
        
        namespace @namespace
        {{
            public partial class GenerateDbContext : DbContext
            {{
                @listDbSet

                public GenerateDbContext(DbContextOptions options) : base(options)
                {{

                }}
            }}
        }}
";

        public string GenDbContext(GeneratorExecutionContext context)
        {
            var source = Template;

            var mainNamespace = context.Compilation.AssemblyName;

            // Replace namespace
            source = source.Replace("@namespace", mainNamespace);

            var syntaxTrees = context.Compilation.SyntaxTrees;

            var dbModelTypes = _helper.GetListDbModel(syntaxTrees.ToList());

            // replace db set
            source = source.Replace("@listDbSet", BuildListDbSet(dbModelTypes));

            // Replace listUsing
            var listDbModelNamespace = _helper.GetListNamespace(dbModelTypes);
            var listUsing = string.Empty;

            listDbModelNamespace.ForEach(s => listUsing += $"using {s};" + '\n');

            source = source.Replace("@listUsing", listUsing);

            return source;
        }

        private string BuildListDbSet(List<ClassDeclarationSyntax> modelTypes)
        {
            string result = string.Empty;
            modelTypes.ForEach(x => result += $"public DbSet<{x.Identifier}> {x.Identifier} {{get; set;}}" + '\n');

            return result;
        }
    }
}
