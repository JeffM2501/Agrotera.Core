using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Types;
using Lidgren.Network;

namespace NetworkMessages.ShipMessages
{
	public class ShipNavigationStatus : ShipOutboundMessage
	{
		public SetShipCourse.CourseTypes CurrentMode = SetShipCourse.CourseTypes.Heading;
		public double TargetHeading = double.MinValue;
		public double TargetSpeed = double.MinValue;

		public double TurnSpeed = 0;
		public double CurrentHeading = 0;
		public double MovementSpeed = 0;

		public Location TargetWaypoint = Location.Invalid;
		public int WaypointCount = 0;

		public bool AtTargetHeading = false;
		public bool AtTargetWaypoint = false;

		public ShipNavigationStatus() : base(MessageCodes.ShipNavigationStatus)
		{
		}


		public override void Pack(NetOutgoingMessage msg)
		{
			base.Pack(msg);
			msg.Write((byte)CurrentMode);
			msg.Write(TargetHeading);
			msg.Write(TurnSpeed);
			msg.Write(CurrentHeading);
			msg.Write(MovementSpeed);

			msg.Write(TargetWaypoint);
			msg.Write(WaypointCount);

			msg.Write(AtTargetHeading);
			msg.Write(AtTargetWaypoint);
		}

		public static ShipNavigationStatus Unpack(NetIncomingMessage msg)
		{
			ShipNavigationStatus p = new ShipNavigationStatus();

			p.CurrentMode = (SetShipCourse.CourseTypes)msg.ReadByte();
			p.TargetHeading = msg.ReadDouble();
			p.TurnSpeed = msg.ReadDouble();
			p.CurrentHeading = msg.ReadDouble();
			p.MovementSpeed = msg.ReadDouble();
			p.TargetWaypoint = msg.ReadLocation();
			p.WaypointCount = msg.ReadInt32();

			p.AtTargetHeading = msg.ReadBoolean();
			p.AtTargetWaypoint = msg.ReadBoolean();

			return p;
		}
	}
}
