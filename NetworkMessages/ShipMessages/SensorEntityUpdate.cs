using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Core.Types;
using Lidgren.Network;

namespace NetworkMessages.ShipMessages
{
	public class SensorEntityUpdate : ShipOutboundMessage
	{
		public int ID = int.MinValue;
		public Vector3F Position = Vector3F.Zero;
		public Vector3F Velocity = Vector3F.Zero;
		public double TimeStamp = double.MinValue;

		public SensorEntityUpdate() : base(MessageCodes.UpdateEntityCode)
		{

		}

		public override void Pack(NetOutgoingMessage msg)
		{
			base.Pack(msg);
			msg.Write(ID);
			msg.Write(Position);
			msg.Write(Velocity);
			msg.Write(TimeStamp);
		}

		public static SensorEntityUpdate Unpack(NetIncomingMessage msg)
		{
			SensorEntityUpdate p = new SensorEntityUpdate();
			p.ID = msg.ReadInt32();
			p.Position = msg.ReadVector3F();
			p.Velocity = msg.ReadVector3F();
			p.TimeStamp = msg.ReadDouble();
			return p;
		}
	}
}
