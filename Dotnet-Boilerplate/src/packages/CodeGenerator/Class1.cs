using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeGenerator
{
    public class Class1
    {
        public static void Test()
        {
            var models = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetExportedTypes())
                .Where(x => x.CustomAttributes.Any(y => y.GetType() == typeof(DbModelAttribute))).ToList();
        }
    }
}
