using System;
using System.Collections.Generic;

using Lidgren.Network;

using Agrotera.Core.Types;

namespace Agrotera.ShipLink.Messages.ShipInfo
{
    public class UpdateShipStatusMessages : CustomPackedNetworkMessage
    {
		public Vector3F Position = Vector3F.Zero;
		public double Rotation = 0;
		public Vector3F MotionVector = Vector3F.Zero;

		public double PowerBuffer = 0.0f;
		public double MaxPowerBuffer = 0.0f;
		public double Coolant = 1.0f;
		public double CurrentHull = 0;

		public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
		{
			msg.Write(Position);
			msg.Write(Rotation);
			msg.Write(MotionVector);

			msg.Write(PowerBuffer);
			msg.Write(Coolant);
			msg.Write(CurrentHull);

			return msg;
		}

		public override void Unpack(NetIncomingMessage msg)
		{
			Position = msg.ReadVector3F();
			Rotation = msg.ReadDouble();
			MotionVector = msg.ReadVector3F();

			PowerBuffer = msg.ReadDouble();
			Coolant = msg.ReadDouble();
			CurrentHull = msg.ReadDouble();
		}
	}
}
