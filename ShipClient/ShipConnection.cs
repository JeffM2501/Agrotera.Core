using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using Entities;
using Core.Types;
using NetworkMessages;
using NetworkMessages.ShipMessages;

namespace ShipClient
{
    public class ShipConnection
    {
		protected NetClient SocketClient = null;

		public UserShip PlayerShip = null;

		public class ConnectedEventArgs : EventArgs
		{
			public int ShipID = -1;
			public List<string> DesiredShipArguments = new List<string>();
		}

		public event EventHandler<ConnectedEventArgs> Connected = null;
		public event EventHandler Disconnected = null;

		public event EventHandler ShipAssigned = null;

		public ShipConnection(string host, int port)
		{
			NetPeerConfiguration config = new NetPeerConfiguration(ProtocollDefinition.ProtoString);
			config.AutoFlushSendQueue = true;
			SocketClient = new NetClient(config);

			SocketClient.Start();
			SocketClient.Connect(host, port);
		}

		public void Update()
		{
			Timer.Advance();
			UpdateInbound();
			if (PlayerShip != null)
			{
				PlayerShip.ProcessMessages();
                PlayerShip.UpdateSelfDR();
                PlayerShip.UpdatePositions();
			}

			UpdateOutbound();
		}

		public void UpdateOutbound()
		{
			if(PlayerShip == null || PlayerShip.OutboundMessages.Count == 0)
				return;

			int count = PlayerShip.OutboundMessages.Count;
			if(count > 10)
				count = 10;

			for(int i = 0; i < count; i++)
			{
				NetOutgoingMessage msg = SocketClient.CreateMessage();
				PlayerShip.OutboundMessages[i].Pack(msg);
				SocketClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
			SocketClient.FlushSendQueue();
			PlayerShip.OutboundMessages.RemoveRange(0, count);
		}

		protected List<NetIncomingMessage> PendingMessages = new List<NetIncomingMessage>();

		public void UpdateInbound()
		{
			if(SocketClient == null)
				return;

			SocketClient.ReadMessages(PendingMessages);

			int toProcess = PendingMessages.Count;
			if(toProcess > 10)
				toProcess = 10;
			if(toProcess == 0)
				return;

			for(int i = 0; i < toProcess; i++)
			{
				NetIncomingMessage im = PendingMessages[i];

				switch(im.MessageType)
				{
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.ErrorMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						/*  AddLogLine(im.ReadString());*/
						break;

					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

						string reason = im.ReadString();

						if(status == NetConnectionStatus.Connected)
						{
							if(Connected != null)
							{
								ConnectedEventArgs args = new ConnectedEventArgs();
								Connected(this, args);

								NetOutgoingMessage hail = SocketClient.CreateMessage();
								if (args.ShipID > 0)
								{
									WantExistingShip ws = new WantExistingShip();
									ws.ShipID = args.ShipID;
									ws.Requirements = args.DesiredShipArguments;
									ws.Pack(hail);
								}
								else
								{
									WantNewShip ns = new WantNewShip();
									ns.Requirements = args.DesiredShipArguments;
									ns.Pack(hail);
								}
								SocketClient.SendMessage(hail, NetDeliveryMethod.ReliableOrdered, 0);
								
							}
							else
							{
								SocketClient.Disconnect("Invalid Client");
								i = toProcess;
								PendingMessages.Clear();
								return;
							}
						}
						else if(status == NetConnectionStatus.Disconnected)
						{
							if(Disconnected != null)
								Disconnected.Invoke(this, EventArgs.Empty);

							SocketClient = null;
							PendingMessages.Clear();
							return;

						}
						break;

					case NetIncomingMessageType.Data:
						if(SocketClient == null)
							break;

						var msg = new ShipInboundMessage(im);

						if(PlayerShip == null)
						{
							if (msg.Code == MessageCodes.AssignShip)
							{
								AssignShip s = AssignShip.Unpack(im);
								PlayerShip = new UserShip();
								PlayerShip.ID = s.ShipID;
								PlayerShip.Name = s.Name;
								PlayerShip.ClassName = s.ClassName;

								if(ShipAssigned != null)
									ShipAssigned.Invoke(this, EventArgs.Empty);
							}
						}
						else
						{
							PlayerShip.InboundMessages.Add(msg);
						}
						break;
				}
			}
			SocketClient.FlushSendQueue();
			PendingMessages.RemoveRange(0, toProcess);
		}
    }
}
