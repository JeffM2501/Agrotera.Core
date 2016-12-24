using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace NetworkMessages.ShipMessages
{
	public class AssignShip : ShipOutboundMessage
	{
		public int ShipID = int.MinValue;
		public string Name = string.Empty;
		public string ClassName = string.Empty;

		public AssignShip() : base(MessageCodes.AssignShip)
		{

		}

		public override void Pack(NetOutgoingMessage msg)
		{
			base.Pack(msg);
			msg.Write(ShipID);
			msg.Write(Name);
			msg.Write(ClassName);
		}

		public static AssignShip Unpack(NetIncomingMessage msg)
		{
			AssignShip p = new AssignShip();
			p.ShipID = msg.ReadInt32();
			p.Name = msg.ReadString();
			p.ClassName = msg.ReadString();
			
			return p;
		}
	}
}
