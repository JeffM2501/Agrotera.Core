using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Core.Types
{
    public class NamedStringValue
    {
        public string Name = string.Empty;
        public string Value = string.Empty;
        public NamedStringValue() { }
        public NamedStringValue(string name, string value) { Name = name; Value = value; }
    }
}
