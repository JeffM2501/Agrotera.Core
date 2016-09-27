using System;
using Agrotera.ShipLink;
using Agrotera.Core.Entities;

namespace SimulationServer.Peers
{
	public class ShipPeer : Peer
	{
		public int ShipID = -1;
		public string authenticationToken = string.Empty;

		public Ship LinkedShip = null;

		public NetworkShipController ShipController = new NetworkShipController();

		public class Message<M> where M : NetworkMessage
		{
			public M Msg = null;
			public double Timestamp = 0;
			public ShipPeer Ship = null;

			public Message(InboundNetworkMessage msg, Peer p)
			{
				Msg = msg.Message as M;
				Timestamp = msg.Timestamp;
				Ship = p as ShipPeer;
			}

			public bool Valid()
			{
				return Msg != null && Ship != null;
			}
		}
	}
}
