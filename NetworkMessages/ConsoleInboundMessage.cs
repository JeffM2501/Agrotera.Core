using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using Core.Types;

namespace NetworkMessages
{
    public class ConsoleInboundMessage
    {
        public bool Processed = false;

        public double Timestamp = double.MinValue;
        public ConsoleMessageCodes Code = ConsoleMessageCodes.Unknown;
        public NetIncomingMessage Payload = null;

        public ConsoleInboundMessage(NetIncomingMessage msg)
        {
            Timestamp = Timer.Now;
            int c = msg.ReadInt32();

            try
            {
                Code = (ConsoleMessageCodes)c;
            }
            catch (System.Exception /*ex*/)
            {

            }
            Payload = msg;
        }
    }
}
