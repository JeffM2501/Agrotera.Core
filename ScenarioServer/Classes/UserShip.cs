using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Types;

using Entities.Classes;
using Entities;

namespace ScenarioServer.Classes
{
    public class UserShip : Ship, IEntityContorller
    {
        public long RemoteConnectionID = long.MinValue;

        public List<ShipMessage> InboundMessages = new List<ShipMessage>();
        public List<ShipMessage> OutboundMessages = new List<ShipMessage>();

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

        protected ShipMessage[] PopOffNInbound(int count)
        {
            if (count <= InboundMessages.Count)
            {
                ShipMessage[] t = InboundMessages.ToArray();
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
                if (msg.Code == ShipMessage.SetCourseCode)
                    SetCourse(msg);
            }
        }

        protected void SetCourse(ShipMessage msg)
        {
            Vector3F newHeading = Vector3F.Zero;
            if (Vector3F.TryParse(msg.Payload, out newHeading))
                SetCourse(newHeading);
        }

        public void SetCourse(Vector3F newHeading)
        {
            Velocity = newHeading;

            if (Velocity.Length() > 100)
                Velocity = Vector3F.Normalize(Velocity) * 100;

            SendCourseAndPosition();
        }

        public void SendCourseAndPosition()
        {
            ShipMessage sm = new ShipMessage();
            sm.Code = ShipMessage.SetSelfPositionCode;
            sm.Payload += Position.ToString();
            sm.Payload += "|";
            sm.Payload += Velocity.ToString();
            sm.Payload += "|";
            sm.Payload += Timer.Now.ToString();

            OutboundMessages.Add(sm);

            LastPositionUpdate = Timer.Now;
        }

        public void SendSensorEntityUpdate(KnownEntity ent)
        {
            ShipMessage sm = new ShipMessage();
            sm.Code = ShipMessage.UpdateEntityCode;
            sm.Payload += ent.BaseEntity.ID.ToString();
            sm.Payload += "|";
            sm.Payload += ent.LastPosition.ToString();
            sm.Payload += "|";
            sm.Payload += ent.LastVelocity.ToString();
            sm.Payload += "|";
            sm.Payload += ent.LastTimestamp.ToString();

            OutboundMessages.Add(sm);

            ent.LastTrasmitUpdate = Timer.Now;
        }
    }
}
