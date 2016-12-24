using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Types
{
    public class NamedFloatValue
    {
        public string Name = string.Empty;
        public double Value = double.MinValue;
        public NamedFloatValue() { }
        public NamedFloatValue(string name, double value) { Name = name; Value = value; }
    }
}
