using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Types;

using Entities.Classes;
using Entities;
using NetworkMessages;
using NetworkMessages.ShipMessages;

namespace ScenarioServer.Classes
{
    public class UserShip : Ship, IEntityContorller
    {
        public long RemoteConnectionID = long.MinValue;

        public List<ShipInboundMessage> InboundMessages = new List<ShipInboundMessage>();
        public List<ShipOutboundMessage> OutboundMessages = new List<ShipOutboundMessage>();

        public double LastPositionUpdate = double.MinValue;
 
        public UserShip()
        {
            Controller = this;

            SensorEntityUpdated += UserShip_SensorEntityUpdated;
            SensorEntityAppeared += UserShip_SensorEntityUpdated;

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
            if (count <= InboundMessages.Count)
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
                    case MessageCodes.SetCourse:
                        SetCourse(SetShipCourse.Unpack(msg.Payload));
                        break;
                } 
            }
        }

        protected void SetCourse(SetShipCourse msg)
        {
			SetCourse(msg.Velocity,msg.Orientation);
        }

        public void SetCourse(Vector3D newHeading, QuaternionD orientation)
        {
            Velocity = newHeading;

            if (Velocity.Length() > 100)
                Velocity = Vector3D.Normalize(Velocity) * 100;

            SendCourseAndPosition();
        }

        public void SendCourseAndPosition()
        {
			SetSelfPosition sm = new SetSelfPosition();
            sm.Position = Position;
            sm.Velocity = Velocity;
            sm.TimeStamp = Timer.Now;

            OutboundMessages.Add(sm);

            LastPositionUpdate = Timer.Now;
        }

        public void SendSensorEntityUpdate(KnownEntity ent)
        {
            if (ent.LastTrasmitUpdate < 0)
            {
                SensorEntityDetails sd = new SensorEntityDetails();

                sd.ID = ent.BaseEntity.ID;
                sd.Position = ent.LastPosition;
                sd.Velocity = ent.LastVelocity;
                sd.TimeStamp = ent.LastTimestamp;
                sd.Name = ent.BaseEntity.Name;
                sd.VisualGraphics = ent.BaseEntity.VisualGraphics;
            }
            else
            {
                SensorEntityUpdate sm = new SensorEntityUpdate();
                sm.ID = ent.BaseEntity.ID;
                sm.Position = ent.LastPosition;
                sm.Velocity = ent.LastVelocity;
                sm.TimeStamp = ent.LastTimestamp;

                OutboundMessages.Add(sm);
            }

            ent.LastTrasmitUpdate = Timer.Now;
        }
    }
}
