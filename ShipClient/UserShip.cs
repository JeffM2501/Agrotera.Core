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
		public List<ShipInboundMessage> InboundMessages = new List<ShipInboundMessage>();
		public List<ShipOutboundMessage> OutboundMessages = new List<ShipOutboundMessage>();

		public double LastPositionUpdate = double.MinValue;

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

			Position = sp.Position;
			Velocity = sp.Velocity;
		}
	}
}
