using System;
using System.Collections.Generic;
using System.Linq;

using Lidgren.Network;

namespace NetworkMessages
{
	public class ShipOutboundMessage
	{
		public ShipMessageCodes Code = ShipMessageCodes.Unknown;
		public NetOutgoingMessage Payload = null;

		public ShipOutboundMessage(ShipMessageCodes code)
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
