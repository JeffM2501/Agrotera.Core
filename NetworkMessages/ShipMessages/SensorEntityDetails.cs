using System;
using System.Collections.Generic;

using Core.Types;
using NetworkMessages;
using Lidgren.Network;

namespace NetworkMessages.ShipMessages
{
    public class SensorEntityDetails : SensorEntityUpdate
    {
        public string Name = string.Empty;
        public string VisualGraphics = string.Empty;

        public SensorEntityDetails()
		{
            Code = MessageCodes.UpdateEnityDetails;
        }

        public override void Pack(NetOutgoingMessage msg)
        {
            base.Pack(msg);
            msg.Write(Name);
            msg.Write(VisualGraphics);
        }

        public static SensorEntityDetails UnpackDeets(NetIncomingMessage msg)
        {
            SensorEntityDetails p = new SensorEntityDetails();
            p.ID = msg.ReadInt32();

            p.Position = msg.ReadLocation();
            p.Velocity = msg.ReadVector3D();
			p.Orientation = msg.ReadRotation();
            p.Rotation = msg.ReadRotation();
            p.TimeStamp = msg.ReadDouble();
            p.Name = msg.ReadString();
            p.VisualGraphics = msg.ReadString();

            return p;
        }
    }
}
