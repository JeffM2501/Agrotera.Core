using System;
using System.Collections.Generic;

using Lidgren.Network;

namespace Agrotera.ShipLink
{
    public interface ClientHandler
    {
        void ClientConnected(NetClient client);

        void ClientReceiveData(NetIncomingMessage msg);
        void ClientDisconnected(string reason);

        void DisconnectClient(string reason);
    }

    public class Client
    {
		NetClient SocketClient;

        protected ClientHandler Handler = null;

        private int ConnectionPort = 0;
        protected string ConnectionHost = string.Empty;

		public int ConnectedPort
		{
			get { return ConnectionPort; }
		}

        public string ConnectedHost
        {
            get { return ConnectionHost; }
        }


        protected object Locker = new object();
		protected bool Connected = false;

		public bool IsConnected
		{
			get { lock(Locker) return Connected; }
		}

        public Client(ClientHandler handler)
        {
            Handler = handler;
        }

        public Client(ClientHandler handler, string host, int port)
        {
            Handler = handler;
        }

        public void Reconnect()
        {
            Connect(ConnectedHost, ConnectedPort);
        }

        public void Connect(string host, int port)
		{
            ConnectionHost = host;
            ConnectionPort = port;
			NetPeerConfiguration config = new NetPeerConfiguration(MessageFactory.ProtocolVersionString);
			config.AutoFlushSendQueue = true;
			SocketClient = new NetClient(config);

			SocketClient.RegisterReceivedCallback(new System.Threading.SendOrPostCallback(CheckMessages));
			SocketClient.Start();
			NetOutgoingMessage hail = SocketClient.CreateMessage(MessageFactory.ProtocolVersionString);
			SocketClient.Connect(host, port, hail);
		}

		public void Shutdown()
		{
			if(SocketClient != null)
			{
				SocketClient.Disconnect("Closing");
				SocketClient.FlushSendQueue();

			}
			SocketClient = null;
		}

		private void CheckMessages(object peer)
		{
			while(ProcessOneMessages()) ;
		}

		public event EventHandler HostConnected = null;
		public event EventHandler HostDisconnected = null;

		protected List<InboundNetworkMessage> PendingInboundMessages = new List<InboundNetworkMessage>();

		protected InboundNetworkMessage PopMessage()
		{
			lock(PendingInboundMessages)
			{
				if(PendingInboundMessages.Count == 0)
					return null;

				InboundNetworkMessage msg = PendingInboundMessages[0];
				PendingInboundMessages.RemoveAt(0);
				return msg;
			}
		}

		public void SendMessage(NetworkMessage msg)
		{
			SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);
		}

		public void SendMessage(NetworkMessage msg, NetDeliveryMethod method, int channel)
		{
			if(SocketClient == null)
				return;
			var packet = SocketClient.CreateMessage();
			packet.WriteTime(true);
			SocketClient.SendMessage(MessageFactory.PackMessage(packet, msg), method, channel);
		}

		private object ExitLocker = new object();
		private bool ExitFlag = false;

		protected bool ExitCheckThread()
		{
			lock(ExitLocker)
				return ExitFlag;
		}

		protected void TerminateCheckThread()
		{
			lock(ExitLocker)
				ExitFlag = true;
		}

		public bool ProcessOneMessages()
		{
			NetIncomingMessage im;
			if(SocketClient != null && (im = SocketClient.ReadMessage()) != null)
			{
				switch(im.MessageType)
				{
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.ErrorMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						/*AddLogLine(im.ReadString());*/
						break;

					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

						string reason = im.ReadString();
						//	AddLogLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

						if(status == NetConnectionStatus.Connected)
						{
							lock(Locker)
								Connected = true;

							// 							if(im.SenderConnection.RemoteHailMessage != null)
							// 								AddLogLine("Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString());
							if(HostConnected != null)
								HostConnected.Invoke(this, EventArgs.Empty);
						}
						else if(status == NetConnectionStatus.Disconnected)
						{
							lock(Locker)
								Connected = false;

							if(HostDisconnected != null)
								HostDisconnected.Invoke(this, EventArgs.Empty);
						}
						break;
					case NetIncomingMessageType.Data:
						{
							InboundNetworkMessage ibm = new InboundNetworkMessage(im.ReadTime(true), MessageFactory.ParseMessage(im));
							lock(PendingInboundMessages)
								PendingInboundMessages.Add(ibm);
						}
						break;
					default:
						//AddLogLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
						break;
				}
				if(SocketClient != null)
					SocketClient.Recycle(im);

				return true;
			}
			return false;
		}
	}
}

