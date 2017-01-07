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
		public Vector3D Position = Vector3D.Zero;
		public Vector3D Velocity = Vector3D.Zero;
        public EulerAnglesD Orientation = EulerAnglesD.Zero;
        public EulerAnglesD Rotation = EulerAnglesD.Zero;
		public double TimeStamp = double.MinValue;

		public SensorEntityUpdate() : base(MessageCodes.UpdateEntity)
		{

		}

		public override void Pack(NetOutgoingMessage msg)
		{
			base.Pack(msg);
			msg.Write((Int32)ID);
			msg.Write(Position);
			msg.Write(Velocity);
            msg.Write(Orientation);
            msg.Write(Rotation);
            msg.Write(TimeStamp);
		}

		public static SensorEntityUpdate Unpack(NetIncomingMessage msg)
		{
			SensorEntityUpdate p = new SensorEntityUpdate();
			p.ID = msg.ReadInt32();
			p.Position = msg.ReadVector3D();
			p.Velocity = msg.ReadVector3D();
            p.Orientation = msg.ReadEulerAnglesD();
            p.Rotation = msg.ReadEulerAnglesD();
            p.TimeStamp = msg.ReadDouble();
			return p;
		}
	}
}
