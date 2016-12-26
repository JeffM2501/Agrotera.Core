using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entities;
using Core.Types;
using NetworkMessages;
using NetworkMessages.ShipMessages;

namespace ShipClient
{
	public class UserShip : Entities.Classes.Ship
	{
		protected Vector3F LastUpdatePosition = Vector3F.Zero;
		protected Vector3F LastUpdateVelocity = Vector3F.Zero;
		public double LastPositionUpdate = double.MinValue;

		public List<ShipInboundMessage> InboundMessages = new List<ShipInboundMessage>();
		public List<ShipOutboundMessage> OutboundMessages = new List<ShipOutboundMessage>();

		
		protected ShipInboundMessage[] PopOffNInbound(int count)
		{
			if(count <= InboundMessages.Count)
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

		public void UpdatePositions()
		{
			Position += (Velocity * Timer.Delta);

			foreach(var item in KnownEntities.Values)
			{
				item.BaseEntity.Position = item.LastPosition + (item.LastVelocity * (Timer.Now - item.LastTrasmitUpdate));
				item.BaseEntity.Velocity = item.LastVelocity;
			}
		}

		public void ProcessMessages()
		{
			if(InboundMessages.Count == 0)
				return;

			foreach(var msg in PopOffNInbound(10))
			{
				if(msg.Code == MessageCodes.SetSelfPosition)
					UpdateSelfPosition(SetSelfPosition.Unpack(msg.Payload));
				else if(msg.Code == MessageCodes.UpdateEntityCode)
					UpdateSensorEntity(SensorEntityUpdate.Unpack(msg.Payload));
			}
		}

		protected void UpdateSelfPosition(SetSelfPosition sp)
		{
			LastPositionUpdate = sp.TimeStamp;

			LastUpdatePosition = sp.Position;
			LastUpdateVelocity = sp.Velocity;

			Position = LastUpdatePosition + (LastUpdateVelocity * (Timer.Now - LastPositionUpdate));
			Velocity = LastUpdateVelocity;
		}
	}
}
