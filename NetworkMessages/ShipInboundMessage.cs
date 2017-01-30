using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using Core.Types;

namespace NetworkMessages
{
	public class ShipInboundMessage
	{
		public bool Processed = false;

		public double Timestamp = double.MinValue;
		public ShipMessageCodes Code = ShipMessageCodes.Unknown;
		public NetIncomingMessage Payload = null;

		public ShipInboundMessage(NetIncomingMessage msg)
		{
			Timestamp = Timer.Now;
			int c = msg.ReadInt32();
			
			try
			{
				Code = (ShipMessageCodes)c;
			}
			catch (System.Exception /*ex*/)
			{
				
			}
			Payload = msg;
		}
	}
}
