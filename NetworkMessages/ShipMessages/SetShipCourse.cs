using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using Core.Types;

namespace NetworkMessages.ShipMessages
{
    public class SetShipCourse : ShipOutboundMessage
    {
        public Vector3D Velocity = Vector3D.UnitX;
        public EulerAnglesD Orientation = EulerAnglesD.Zero;

        public SetShipCourse() : base(MessageCodes.SetCourse)
		{

        }

        public override void Pack(NetOutgoingMessage msg)
        {
            base.Pack(msg);
            msg.Write(Velocity);
            msg.Write(Orientation);
        }

        public static SetShipCourse Unpack(NetIncomingMessage msg)
        {
            SetShipCourse p = new SetShipCourse();
            p.Velocity = msg.ReadVector3D();
            p.Orientation = msg.ReadEulerAnglesD();

            return p;
        }
    }
}
