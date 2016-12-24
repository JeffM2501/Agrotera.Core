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
		public double Timestamp = double.MinValue;
		public MessageCodes Code = MessageCodes.Unknown;
		public NetIncomingMessage Payload = null;

		public ShipInboundMessage(NetIncomingMessage msg)
		{
			Timestamp = Timer.Now;
			int c = msg.ReadInt32();
			
			try
			{
				Code = (MessageCodes)c;
			}
			catch (System.Exception ex)
			{
				
			}
			Payload = msg;
		}
	}
}
