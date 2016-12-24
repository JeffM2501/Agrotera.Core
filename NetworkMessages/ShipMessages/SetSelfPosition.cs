using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Types;
using Lidgren.Network;

namespace NetworkMessages.ShipMessages
{
	public class SetSelfPosition : ShipOutboundMessage
	{
		public Vector3F Position = Vector3F.Zero;
		public Vector3F Velocity = Vector3F.Zero;
		public double TimeStamp = double.MinValue;

		public SetSelfPosition() : base(MessageCodes.SetSelfPosition)
		{

		}

		public override void Pack(NetOutgoingMessage msg)
		{
			base.Pack(msg);
			msg.Write(Position);
			msg.Write(Velocity);
			msg.Write(TimeStamp);
		}

		public static SetSelfPosition Unpack(NetIncomingMessage msg)
		{
			SetSelfPosition p = new SetSelfPosition();
			p.Position = msg.ReadVector3F();
			p.Velocity = msg.ReadVector3F();
			p.TimeStamp = msg.ReadDouble();
			return p;
		}
	}
}
