using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreModificationCheckAttribute : Attribute
    {
    }
}
