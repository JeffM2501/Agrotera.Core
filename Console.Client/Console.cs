using NetworkMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console.Client
{
    public class Console
    {
        public string Name = string.Empty;
        public int ID = -1;

        public virtual void ProcessMessage(ConsoleInboundMessage msg)
        {
        }
    }
}
