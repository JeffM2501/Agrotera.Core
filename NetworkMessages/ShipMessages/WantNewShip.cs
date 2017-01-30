using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Types;
using Lidgren.Network;

namespace NetworkMessages.ShipMessages
{
	public class WantNewShip : ShipOutboundMessage
	{
		public List<string> Requirements = new List<string>();

		public WantNewShip() : base(ShipMessageCodes.NewShip)
		{

		}

		public override void Pack(NetOutgoingMessage msg)
		{
			base.Pack(msg);
			msg.Write(Requirements.Count);
			foreach(string s in Requirements)
				msg.Write(s);
		}

		public static WantNewShip Unpack(NetIncomingMessage msg)
		{
			WantNewShip p = new WantNewShip();
			int count = msg.ReadInt32();
			for(int i = 0; i < count; i++)
				p.Requirements.Add(msg.ReadString());
			return p;
		}
	}
}
