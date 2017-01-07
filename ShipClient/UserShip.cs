using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entities;
using Core.Types;
using NetworkMessages;
using NetworkMessages.ShipMessages;
using Entities.Classes;

namespace ShipClient
{
	public class UserShip : Entities.Classes.Ship
	{
		protected Vector3D LastUpdatePosition = Vector3D.Zero;
		protected Vector3D LastUpdateVelocity = Vector3D.Zero;
		public double LastPositionUpdate = double.MinValue;

		public List<ShipInboundMessage> InboundMessages = new List<ShipInboundMessage>();
		public List<ShipOutboundMessage> OutboundMessages = new List<ShipOutboundMessage>();

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

		public void UpdatePositions()
		{
			Position += (Velocity * Timer.Delta);

			double visSquared = VisualRadius() * VisualRadius();

			foreach(var item in KnownEntities.Values)
			{
                double delta = (Timer.Now - item.LastTimestamp);

                item.BaseEntity.Position = item.LastPosition + (item.LastVelocity * delta);
				item.BaseEntity.Velocity = item.LastVelocity;

                item.BaseEntity.Orientation = item.LastOrientation + (item.LastRotation * delta);

				ShipCentricSensorEntity ent = item as ShipCentricSensorEntity;
				if(ent == null)
					continue;

				// for 32 bit rendering
				ent.ShipRelativePosition = Vector3F.FromRelativeDobules(ent.BaseEntity.Position, Position);
				ent.ShipRelativeVelocity = Vector3F.FromRelativeDobules(ent.BaseEntity.Velocity, Velocity);

                ent.Visible = ent.ShipRelativePosition.LengthSquared() <= visSquared;
			}
		}

		public void ProcessMessages()
		{
			if(InboundMessages.Count == 0)
				return;

			foreach(var msg in PopOffNInbound(10))
			{
				if(msg.Processed)
					continue;

				if(msg.Code == MessageCodes.SetSelfPosition)
					UpdateSelfPosition(SetSelfPosition.Unpack(msg.Payload));
				else if(msg.Code == MessageCodes.UpdateEntity)
					UpdateSensorEntity(SensorEntityUpdate.Unpack(msg.Payload));
                else if (msg.Code == MessageCodes.UpdateEnityDetails)
                    UpdateSensorEntity(SensorEntityDetails.UnpackDeets(msg.Payload));

				msg.Processed = true;
			}
		}

		public override void RefreshEntity(KnownEntity ke, SensorEntityUpdate update)
		{
			ke.Refresh(update);

			ShipCentricSensorEntity sce = ke as ShipCentricSensorEntity;
			if(sce == null)
				return;

			sce.ShipRelativePosition = Vector3F.FromRelativeDobules(sce.BaseEntity.Position, Position);
			sce.ShipRelativeVelocity = new Vector3F(sce.BaseEntity.Velocity);
		}

		protected override KnownEntity NewSensorEnity(Entity ent)
		{
			return new ShipCentricSensorEntity(ent);
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

        public void SetCourse(Vector3D velocity, EulerAnglesD orientation)
        {
			SetShipCourse sc = new SetShipCourse();
			sc.Velocity = velocity;
			sc.Orientation = orientation;
			Send(sc);
		}
	}
}
