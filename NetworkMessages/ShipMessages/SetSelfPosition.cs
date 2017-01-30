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
		public Location Position = Location.Zero;
		public Vector3D Velocity = Vector3D.Zero;
		public Rotation Orientation = Rotation.Zero;
		public double TimeStamp = double.MinValue;

		public SetSelfPosition() : base(ShipMessageCodes.SetSelfPosition)
		{

		}

		public override void Pack(NetOutgoingMessage msg)
		{
			base.Pack(msg);
			msg.Write(Position);
			msg.Write(Velocity);
			msg.Write(Orientation);
			msg.Write(TimeStamp);
		}

		public static SetSelfPosition Unpack(NetIncomingMessage msg)
		{
			SetSelfPosition p = new SetSelfPosition();
			p.Position = msg.ReadLocation();
			p.Velocity = msg.ReadVector3D();
			p.Orientation = msg.ReadRotation();
			p.TimeStamp = msg.ReadDouble();
			return p;
		}
	}
}
