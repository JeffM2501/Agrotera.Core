using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Types;
using Lidgren.Network;

namespace NetworkMessages.ShipMessages
{
	public class WantExistingShip : ShipOutboundMessage
	{
		public int ShipID = int.MinValue;
		public List<string> Requirements = new List<string>();

		public WantExistingShip() : base(MessageCodes.ExistingShip)
		{

		}

		public override void Pack(NetOutgoingMessage msg)
		{
			base.Pack(msg);
			msg.Write(ShipID);
			msg.Write(Requirements.Count);
			foreach(string s in Requirements)
				msg.Write(s);
		}

		public static WantExistingShip Unpack(NetIncomingMessage msg)
		{
			WantExistingShip p = new WantExistingShip();
			p.ShipID = msg.ReadInt32();
			int count = msg.ReadInt32();
			for(int i = 0; i < count; i++)
				p.Requirements.Add(msg.ReadString());
			return p;
		}
	}
}
