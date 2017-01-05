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
		public Vector3D Position = Vector3D.Zero;
		public Vector3D Velocity = Vector3D.Zero;
		public QuaternionD Orientation = QuaternionD.Identity;
		public double TimeStamp = double.MinValue;

		public SetSelfPosition() : base(MessageCodes.SetSelfPosition)
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
			p.Position = msg.ReadVector3D();
			p.Velocity = msg.ReadVector3D();
			p.Orientation = msg.ReadQuaternionD();
			p.TimeStamp = msg.ReadDouble();
			return p;
		}
	}
}
