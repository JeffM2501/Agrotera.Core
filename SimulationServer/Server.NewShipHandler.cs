using System;
using System.Collections.Generic;

using Lidgren.Network;

using Agrotera.ShipLink;
using Agrotera.ShipLink.Messages.Join;
using Agrotera.Core.Entities;

using SimulationServer.Peers;

namespace SimulationServer
{
	internal partial class Server
	{
		void ProcessCreateShip(InboundNetworkMessage m, Peer peer)
		{
			var msg = new ShipPeer.Message<CreateShipMessage>(m, peer);
			if(!msg.Valid() || msg.Ship.LinkedShip != null)
				return;

			msg.Ship.LinkedShip = TheScenario.SpawnPlayerShip(msg.Msg.ShipClass, string.Empty);

			msg.Ship.LinkedShip.SetController(msg.Ship.ShipController);
			msg.Ship.ShipController.LinkToPeer(msg.Ship);

			msg.Ship.LinkedShip.Name = msg.Msg.ShipName;
			msg.Ship.LinkedShip.ShipServerOwnerID = Agrotera.Core.Utilities.RNG.Next().ToString() + Agrotera.Core.Utilities.RNG.Next().ToString() + msg.Ship.ShipID.ToString();

			ShipLinkedMessage responce = new ShipLinkedMessage();
			responce.ShipUID = msg.Ship.LinkedShip.ID;
			responce.ShipName = msg.Ship.LinkedShip.Name;
			responce.ShipRejoinToken = msg.Ship.LinkedShip.ShipServerOwnerID;

			peer.SendMessage(responce);
		}

		void ProcessRejoinShip(InboundNetworkMessage m, Peer peer)
		{
			var msg = new ShipPeer.Message<RejoinShipMessage>(m, peer);
			if(!msg.Valid() || msg.Ship.LinkedShip != null)
				return;

			Ship ship = null;
			foreach(var s in PlayerConnectedShips)
			{
				if(s.ShipServerOwnerID == msg.Msg.ShipRejoinKey)
				{
					ship = s;
					break;
				}
			}

			if(ship == null)
				TheScenario.SpawnPlayerShip(string.Empty, string.Empty);

			msg.Ship.LinkedShip = ship;
			msg.Ship.LinkedShip.SetController(msg.Ship.ShipController);
			msg.Ship.ShipController.LinkToPeer(msg.Ship);

			ShipLinkedMessage responce = new ShipLinkedMessage();
			responce.ShipUID = ship.ID;
			responce.ShipName = ship.Name;
			responce.ShipRejoinToken = ship.ShipServerOwnerID;

			peer.SendMessage(responce);
		}
	}
}
