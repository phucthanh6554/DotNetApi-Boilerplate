using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DbModelAttribute : Attribute
    {
    }
}
