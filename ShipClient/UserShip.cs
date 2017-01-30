using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entities;
using Core.Types;
using NetworkMessages;
using NetworkMessages.ShipMessages;
using Entities.Classes.Components;

namespace ShipClient
{
	public class UserShip : Entities.Classes.Ship
	{
		protected Location LastUpdatePosition = Location.Zero;
		protected Vector3D LastUpdateVelocity = Vector3D.Zero;
		public double LastPositionUpdate = double.MinValue;

		public List<ShipInboundMessage> InboundMessages = new List<ShipInboundMessage>();
		public List<ShipOutboundMessage> OutboundMessages = new List<ShipOutboundMessage>();

        public event EventHandler NavCompModeChanged = null;

        public UserShip() : base()
        {
            NaviComp.ReadOnly = true;
        }


        public class ShipCentricSensorEntity : KnownEntity
		{
			public Vector3F ShipRelativePosition = Vector3F.Zero;
			public Vector3F ShipRelativeVelocity = Vector3F.Zero;

			public bool Visible = false;

			public ShipCentricSensorEntity(Entity ent) : base(ent)
			{

			}
		}
		
		protected ShipInboundMessage[] PopOffNInbound(int count)
		{
			if(InboundMessages.Count < count)
			{
				ShipInboundMessage[] t = InboundMessages.ToArray();
				InboundMessages.Clear();
				return t;
			}
			else
			{
				var a = InboundMessages.GetRange(0, count).ToArray();
				InboundMessages.RemoveRange(0, count);
				return a;
			}
		}

		protected void Send(ShipOutboundMessage msg)
		{
			OutboundMessages.Add(msg);
		}

        public event EventHandler PostionsUpdated = null;

		public void UpdatePositions()
		{
			Position += (Velocity * Timer.Delta);

			double visSquared = VisualRadius() * VisualRadius();

			foreach(var item in KnownEntities.Values)
			{
                double delta = (Timer.Now - item.LastTimestamp);

                item.BaseEntity.Position = item.LastPosition + (item.LastVelocity * delta);
				item.BaseEntity.Velocity = item.LastVelocity;

                item.BaseEntity.Orientation = item.LastOrientation + (item.LastAngularVelocity * delta);

				ShipCentricSensorEntity ent = item as ShipCentricSensorEntity;
				if(ent == null)
					continue;

				// for 32 bit rendering
				ent.ShipRelativePosition = Location.FromRelativeDobules(ent.BaseEntity.Position, Position);
				ent.ShipRelativeVelocity = Vector3F.FromRelativeDobules(ent.BaseEntity.Velocity, Velocity);

                ent.Visible = ent.ShipRelativePosition.LengthSquared() <= visSquared;
			}

            if (PostionsUpdated != null)
                PostionsUpdated.Invoke(this, EventArgs.Empty);

        }

        public event EventHandler MessagesProcessed = null;

		public void ProcessMessages()
		{
			if(InboundMessages.Count == 0)
				return;

			foreach(var msg in PopOffNInbound(10))
			{
				if(msg.Processed)
					continue;

                if (msg.Code == ShipMessageCodes.SetSelfPosition)
                    UpdateSelfPosition(SetSelfPosition.Unpack(msg.Payload));
                else if (msg.Code == ShipMessageCodes.UpdateEntity)
                    UpdateSensorEntity(SensorEntityUpdate.Unpack(msg.Payload));
                else if (msg.Code == ShipMessageCodes.UpdateEnityDetails)
                    UpdateSensorEntity(SensorEntityDetails.UnpackDeets(msg.Payload));
                else if (msg.Code == ShipMessageCodes.ShipNavigationStatus)
                    UpdateNavStatus(ShipNavigationStatus.Unpack(msg.Payload));

				msg.Processed = true;
			}

            if (MessagesProcessed != null)
                MessagesProcessed.Invoke(this, EventArgs.Empty);

        }

		public override void RefreshEntity(KnownEntity ke, SensorEntityUpdate update)
		{
			ke.Refresh(update);

			ShipCentricSensorEntity sce = ke as ShipCentricSensorEntity;
			if(sce == null)
				return;

			sce.ShipRelativePosition = Location.FromRelativeDobules(sce.BaseEntity.Position, Position);
			sce.ShipRelativeVelocity = new Vector3F(sce.BaseEntity.Velocity.X, sce.BaseEntity.Velocity.Y, sce.BaseEntity.Velocity.Z);
		}

		protected override KnownEntity NewSensorEnity(Entity ent)
		{
			return new ShipCentricSensorEntity(ent);
		}

        protected void UpdateNavStatus(ShipNavigationStatus status)
        {
            NaviComp.DesiredSpeed = status.TargetSpeed;
            NaviComp.DesiredTurnSpeed = status.TurnSpeed;

            bool lastAtHeading = NaviComp.AtHeading;

            NavigationComputer.NavigationModes lastMode = NaviComp.Mode;

            switch (status.CurrentMode)
            {
                default:
                case SetShipCourse.CourseTypes.Manual:
                    NaviComp.Mode = Entities.Classes.Components.NavigationComputer.NavigationModes.Direct;
     
                    break;

                case SetShipCourse.CourseTypes.Heading:
                    NaviComp.Mode = Entities.Classes.Components.NavigationComputer.NavigationModes.Heading;
                    NaviComp.DesiredHeading.Angle = status.TargetHeading;
                    break;

                case SetShipCourse.CourseTypes.Waypoints:
                    NaviComp.Mode = Entities.Classes.Components.NavigationComputer.NavigationModes.Course;
                    NaviComp.DesiredHeading.Angle = status.TargetHeading;
                    NaviComp.Waypoints.Clear();
                    NaviComp.Waypoints.Add(new Entities.Classes.Components.NavigationComputer.CourseWaypoint(status.TargetWaypoint));
                    break;
            }

            if (lastMode != NaviComp.Mode && NavCompModeChanged != null)
                NavCompModeChanged.Invoke(this, EventArgs.Empty);

            NaviComp.AtHeading = status.AtTargetHeading;

            if (status.AtTargetHeading && !lastAtHeading)
                NaviComp.CallAtTargetHeading();

            if (status.AtTargetWaypoint)
                NaviComp.CallAtWaypoint(NaviComp.Waypoints[0]);
        }

        protected void UpdateSelfPosition(SetSelfPosition sp)
		{
			LastPositionUpdate = Timer.Now; // update to use synced clock and sp.TimeStamp;

			LastUpdatePosition = sp.Position;
			LastUpdateVelocity = sp.Velocity;

			// TODO, use a synced clock and do proper DR
			Position = LastUpdatePosition;// + (LastUpdateVelocity * (Timer.Now - LastPositionUpdate));
			Velocity = LastUpdateVelocity;
		}

        public void UpdateSelfDR()
        {
            Position = LastUpdatePosition + (LastUpdateVelocity * (Timer.Now - LastPositionUpdate));
        }

        public void SetCourseManual(double speed, double turn)
        {
			SetShipCourse sc = new SetShipCourse();
            sc.CourseType = SetShipCourse.CourseTypes.Manual;
            sc.Speed = speed;
            sc.TurnSpeed = turn;
			Send(sc);
		}

        public void SetCourseHeading(double speed, double heading)
        {
            SetShipCourse sc = new SetShipCourse();
            sc.CourseType = SetShipCourse.CourseTypes.Heading;
            sc.Speed = speed;
            sc.DesiredHeading = heading;
            Send(sc);
        }

        public void SetCourseWaypoints(double speed, List<Location> waypoints)
        {
            SetShipCourse sc = new SetShipCourse();
            sc.CourseType = SetShipCourse.CourseTypes.Waypoints;
            sc.Speed = speed;
            sc.Waypoints = waypoints;
            Send(sc);
        }
    }
}
