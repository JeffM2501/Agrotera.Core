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
        public enum CourseTypes
        {
            Manual = 0,
            Heading,
            Waypoints,
        }

        public CourseTypes CourseType = CourseTypes.Manual;

        public double Speed = 0;
        public double TurnSpeed = 0;
        public double DesiredHeading = 0;

        public List<Location> Waypoints = new List<Location>();

        public SetShipCourse() : base(ShipMessageCodes.SetCourse)
		{

        }

        public override void Pack(NetOutgoingMessage msg)
        {
            base.Pack(msg);
            msg.Write((byte)CourseType);
            msg.Write(Speed);

            switch (CourseType)
            {
                case CourseTypes.Manual:
                    msg.Write(TurnSpeed);
                    break;

                case CourseTypes.Heading:
                    msg.Write(DesiredHeading);
                    break;

                case CourseTypes.Waypoints:
                    msg.Write((Int32)Waypoints.Count);
                    foreach (var w in Waypoints)
                        msg.Write(w);
                    break;
            }
        }

		protected static void Unpack(NetIncomingMessage msg, SetShipCourse p)
		{
			byte t = msg.ReadByte();
			p.CourseType = (CourseTypes)t;
			p.Speed = msg.ReadDouble();

			switch(p.CourseType)
			{
				case CourseTypes.Manual:
					p.TurnSpeed = msg.ReadDouble();
					break;

				case CourseTypes.Heading:
					p.DesiredHeading = msg.ReadDouble();
					break;

				case CourseTypes.Waypoints:
					{
						int count = msg.ReadInt32();

						for(int i = 0; i < count; i++)
							p.Waypoints.Add(msg.ReadLocation());
					}
					break;
			}
		}

		public static SetShipCourse Unpack(NetIncomingMessage msg)
        {
            SetShipCourse p = new SetShipCourse();
			Unpack(msg, p);
			return p;
		}
    }
}
