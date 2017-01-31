using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Lidgren.Network;

namespace NetworkMessages
{
    public class ConsoleOutboundMessage
    {
        public ConsoleMessageCodes Code = ConsoleMessageCodes.Unknown;
        public NetOutgoingMessage Payload = null;

        public List<int> AffectedConsoles = new List<int>();

        public ConsoleOutboundMessage(ConsoleMessageCodes code)
        {
            Code = code;
        }

        public virtual void Pack(NetOutgoingMessage msg)
        {
            Payload = msg;
            msg.Write((Int32)Code);
        }
    }
}
