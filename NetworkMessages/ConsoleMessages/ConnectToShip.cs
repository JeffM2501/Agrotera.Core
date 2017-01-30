using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace NetworkMessages.ConsoleMessages
{
    public class ConnectToShip : ConsoleOutboundMessage
    {
        public List<string> Consoles = new List<string>();

        public ConnectToShip() : base(ConsoleMessageCodes.ConnectToShip)
        {

        }

        public override void Pack(NetOutgoingMessage msg)
        {
            base.Pack(msg);
            msg.Write(Consoles.Count);
            foreach (string s in Consoles)
                msg.Write(s);
        }

        public static ConnectToShip Unpack(NetIncomingMessage msg)
        {
            ConnectToShip p = new ConnectToShip();
            int count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
                p.Consoles.Add(msg.ReadString());
            return p;
        }
    }
}