using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.DefaultControllers.Complex;

using Agrotera.ShipLink;
using Agrotera.ShipLink.Messages.ShipInfo;


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

            UpdateShipTemplateMessage msg = new UpdateShipTemplateMessage();
            msg.Template = NetworkPeer.LinkedShip.TemplateShip;

            NetworkPeer.SendMessage(msg);
		}


		public void ProcessMessage(NetworkMessage msg)
		{
			MessageCallbacks.CallProcessor(NetworkPeer, msg);
		}
	}
}
