using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Types;

using Entities.Classes;
using Entities;
using NetworkMessages;
using NetworkMessages.ShipMessages;
using Entities.Classes.Components;

namespace ScenarioServer.Classes
{
    public class UserShip : Ship, IEntityContorller
    {
        public long RemoteConnectionID = long.MinValue;

        public List<ShipInboundMessage> InboundMessages = new List<ShipInboundMessage>();
        public List<ShipOutboundMessage> OutboundMessages = new List<ShipOutboundMessage>();

        public double LastPositionUpdate = double.MinValue;
 
        public UserShip() : base()
        {
            Controller = this;

            SensorEntityUpdated += UserShip_SensorEntityUpdated;
            SensorEntityAppeared += UserShip_SensorEntityUpdated;

			NaviComp.ArrivedAtWaypoint += NaviComp_ArrivedAtWaypoint;
			NaviComp.HeadingReached += NaviComp_HeadingReached;
			NaviComp.CourseComplete += NaviComp_ArrivedAtWaypoint;
        }

		private void NaviComp_HeadingReached(object sender, EventArgs e)
		{
			SendNavStatusUpdate(false);
		}

		private void NaviComp_ArrivedAtWaypoint(object sender, NavigationComputer.CourseWaypoint e)
		{
			SendNavStatusUpdate(true);
		}

		public void Send(ShipOutboundMessage msg)
		{
			OutboundMessages.Add(msg);
		}

        private void UserShip_SensorEntityUpdated(object sender, KnownEntity e)
        {
            SendSensorEntityUpdate(e);
        }

        public virtual void RemoteDisconnect()
        {
            RemoteConnectionID = long.MinValue;
        }

        public virtual void RemoteReconnect(long id)
        {
            RemoteConnectionID = id;
			SendCourseAndPosition();
		}

        void IEntityContorller.AddEntity(Entity ent)
        {
        }

        void IEntityContorller.UpdateEntity(Entity ent)
        {
            ProcessMessages();

            ProcessUpdates();
        }

        protected void ProcessUpdates()
        {
            if (Timer.Now - LastPositionUpdate > 0.1)
                SendCourseAndPosition();
        }

        protected ShipInboundMessage[] PopOffNInbound(int count)
        {
            if (count > InboundMessages.Count)
            {
				ShipInboundMessage[] t = InboundMessages.ToArray();
                InboundMessages.Clear();
                return t;
            }
            else
            {
                return InboundMessages.GetRange(0, count).ToArray();
            }
        }

        protected void ProcessMessages()
        {
            if (InboundMessages.Count == 0)
                return;

            foreach(var msg in PopOffNInbound(10))
            {
                switch(msg.Code)
                {
                    case ShipMessageCodes.SetCourse:
                        SetCourse(SetShipCourse.Unpack(msg.Payload));
                        break;
                } 
            }
        }

        protected void SetCourse(SetShipCourse msg)
        {
			switch(msg.CourseType)
            {
                case SetShipCourse.CourseTypes.Manual:
                    NaviComp.SetDirectNavigation(msg.TurnSpeed, msg.Speed);
                    break;

                case SetShipCourse.CourseTypes.Heading:
                    NaviComp.SteerTo(msg.DesiredHeading, msg.Speed);
                    break;

                case SetShipCourse.CourseTypes.Waypoints:
                    {
                        List<NavigationComputer.CourseWaypoint> waypoints = new List<NavigationComputer.CourseWaypoint>();

                        foreach(var loc in msg.Waypoints)
                        {
                            NavigationComputer.CourseWaypoint wp = new NavigationComputer.CourseWaypoint();
                            wp.TargetPosition = loc;
                            wp.AcceptableDistance = 25;
                            waypoints.Add(wp);
                        }
                        NaviComp.PlotCourse(waypoints, msg.Speed, true);
                    }
                    break;
            }

			SendNavStatusUpdate(false);


			SendCourseAndPosition();
        }

        public void SendUpdatedPostion()
        {
            if (Timer.Now - LastPositionUpdate > 0.05)
                SendCourseAndPosition();
        }

        public void SendCourseAndPosition()
        {
			SetSelfPosition sm = new SetSelfPosition();
            sm.Position = Position;
            sm.Velocity = Velocity;
			sm.Orientation = Orientation;
            sm.TimeStamp = Timer.Now;

			Send(sm);

            LastPositionUpdate = Timer.Now;
        }

		protected SetShipCourse.CourseTypes ConvertNavType(NavigationComputer.NavigationModes t)
		{
			switch(t)
			{
				case NavigationComputer.NavigationModes.Course:
					return SetShipCourse.CourseTypes.Waypoints;

				case NavigationComputer.NavigationModes.Heading:
					return SetShipCourse.CourseTypes.Heading;
			}

			return SetShipCourse.CourseTypes.Manual;
		}

		public void SendNavStatusUpdate(bool atWaypoint)
		{
			ShipNavigationStatus status = new ShipNavigationStatus();

			status.CurrentMode = ConvertNavType(NaviComp.Mode);
			status.TargetHeading = status.CurrentMode == SetShipCourse.CourseTypes.Manual ? double.MinValue : NaviComp.DesiredHeading.Angle;
			status.TargetSpeed = NaviComp.DesiredSpeed;

			status.MovementSpeed = Velocity.Length();
			status.TurnSpeed = AngularVelocity.Angle;

			status.CurrentHeading = Orientation.Angle;

			status.AtTargetHeading = Rotation.AngleBetween(Orientation, NaviComp.DesiredHeading) < 0.01;

			status.AtTargetWaypoint = atWaypoint;

			status.WaypointCount = NaviComp.Waypoints.Count;
			if (NaviComp.Mode == NavigationComputer.NavigationModes.Course && NaviComp.Waypoints.Count > 0)
				status.TargetWaypoint = NaviComp.Waypoints[0].TargetPosition;

			Send(status);
		}

        public void SendSensorEntityUpdate(KnownEntity ent)
        {
            if (ent.LastTrasmitUpdate <= 0)
            {
                SensorEntityDetails sd = new SensorEntityDetails();

                sd.ID = ent.BaseEntity.ID;
                sd.Position = ent.LastPosition;
                sd.Velocity = ent.LastVelocity;
				sd.Orientation = ent.LastOrientation;
                sd.TimeStamp = ent.LastTimestamp;
                sd.Name = ent.BaseEntity.Name;
                sd.VisualGraphics = ent.BaseEntity.VisualGraphics;

				Send(sd);
			}
            else
            {
                SensorEntityUpdate sm = new SensorEntityUpdate();
                sm.ID = ent.BaseEntity.ID;
                sm.Position = ent.LastPosition;
                sm.Velocity = ent.LastVelocity;
				sm.Orientation = ent.LastOrientation;
				sm.TimeStamp = ent.LastTimestamp;

				Send(sm);
            }

            ent.LastTrasmitUpdate = Timer.Now;
        }
    }
}
