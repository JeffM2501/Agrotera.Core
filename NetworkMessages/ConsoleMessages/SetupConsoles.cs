using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace NetworkMessages.ConsoleMessages
{
    public class SetupConsoles : ConsoleOutboundMessage
    {
        public class ConnsoleInfo
        {
            public int ID = 0;
            public string Name = string.Empty;
        }

        public List<ConnsoleInfo> Consoles = new List<ConnsoleInfo>();

        public SetupConsoles() : base(ConsoleMessageCodes.SetupConsoles)
        {

        }

        public override void Pack(NetOutgoingMessage msg)
        {
            base.Pack(msg);
            msg.Write(Consoles.Count);
            foreach (ConnsoleInfo s in Consoles)
            {
                msg.Write(s.ID);
                msg.Write(s.Name);
            }
        }

        public static SetupConsoles Unpack(NetIncomingMessage msg)
        {
            SetupConsoles p = new SetupConsoles();
            int count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ConnsoleInfo info = new ConnsoleInfo();
                info.ID = msg.ReadInt32();
                info.Name = msg.ReadString();
                p.Consoles.Add(info);
            }
            return p;
        }
    }
}
