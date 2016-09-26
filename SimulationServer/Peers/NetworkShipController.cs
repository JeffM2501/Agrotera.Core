using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.DefaultControllers.Complex;

using Agrotera.ShipLink;


namespace SimulationServer.Peers
{
	public class NetworkShipController : ShipController
	{
		public ShipPeer NetworkPeer = null;

		protected MessageProcessor MessageCallbacks = new MessageProcessor();

		public NetworkShipController()
		{
			//MessageCallbacks.CallProcessor()
		}


		public void LinkToPeer(ShipPeer peer)
		{
			NetworkPeer = peer;

			// TODO, DUMP data to the peer about our wonderfull ship
		}

		public void ProcessMessage(NetworkMessage msg)
		{
			MessageCallbacks.CallProcessor(NetworkPeer, msg);
		}
	}
}
