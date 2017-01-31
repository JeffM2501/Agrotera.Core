using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console.Client.Consoles
{
    public class HelmConsole : Console
    {
        public HelmConsole(int id)
        {
            Name = "Helm";
            ID = id;
        }
    }
}
