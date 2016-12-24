using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using ScenarioServer.Classes;
using Entities;
using NetworkMessages;
using NetworkMessages.ShipMessages;

namespace ScenarioServer
{
    public class ShipListener
    {
        public class Peer : EventArgs
        {
            public long ID = -1;
            public NetConnection SocketPeer = null;

            public int ShipID = -1;
			public List<string> DesiredShipAttributes = new List<string>();
            public UserShip Ship = null;

            public Peer(NetConnection p)
            {
                SocketPeer = p;
                ID = p.Peer.UniqueIdentifier;
            }
        }

        protected NetServer SocketHost = null;

        protected Dictionary<long,Peer> ConnectedPeers = new Dictionary<long,Peer>();

        public event EventHandler<Peer> PeerConnected = null;
        public event EventHandler<Peer> PeerDisconnected = null;
        public event EventHandler<Peer> PeerWantsNewShip = null;
        public event EventHandler<Peer> PeerWantsOldShip = null;

        public ShipListener(int port)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(ProtocollDefinition.ProtoString);
            config.AutoFlushSendQueue = true;
            config.MaximumConnections = 32;
            config.ConnectionTimeout = 100;
            config.Port = port;

            SocketHost = new NetServer(config);

            SocketHost.Start();
        }

        public void Shutdown()
        {
            SocketHost.Shutdown("Shutdown");
            SocketHost = null;
        }

        protected Peer FindPeer(long id)
        {
            if (ConnectedPeers.ContainsKey(id))
                return ConnectedPeers[id];
            return null;
        }

        protected void ForceDisconnect(Peer peer)
        {
            peer.SocketPeer.Disconnect("Force Disconnect");
            ConnectedPeers.Remove(peer.ID);
            if (PeerDisconnected != null)
                PeerDisconnected.Invoke(this, peer);
        }

        public void UpdateOutbound()
        {
            foreach(var p in ConnectedPeers.Values)
            {
                if (p.Ship == null || p.Ship.OutboundMessages.Count == 0)
                    continue;

                int count = p.Ship.OutboundMessages.Count;
                if (count > 10)
                    count = 10;

                for(int i = 0; i < count; i++)
                {
                    NetOutgoingMessage msg = p.SocketPeer.Peer.CreateMessage();
					p.Ship.OutboundMessages[i].Pack(msg);
                    p.SocketPeer.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
                }

                p.Ship.OutboundMessages.RemoveRange(0, count);
            }
        }

        List<NetIncomingMessage> PendingMessages = new List<NetIncomingMessage>();

        public void UpdateInbound(double timestamp)
        {
            SocketHost.ReadMessages(PendingMessages);

            int toProcess = PendingMessages.Count;
            if (toProcess > 10)
                toProcess = 10;
            if (toProcess == 0)
                return;

            for (int i = 0; i < toProcess; i++)
            {
                NetIncomingMessage im = PendingMessages[i];

                long id = long.MinValue;
                if (im != null && im.SenderConnection != null)
                    id = im.SenderConnection.RemoteUniqueIdentifier;

                Peer peer = FindPeer(im.SenderConnection.Peer.UniqueIdentifier);

                switch (im.MessageType)
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
              
                        if (status == NetConnectionStatus.Connected)
                        {
                            peer = new Peer(im.SenderConnection);
                            ConnectedPeers.Add(peer.ID, peer);
                            if (PeerConnected != null)
                                PeerConnected(this, peer);
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            if (peer != null)
                            {
                                ConnectedPeers.Remove(peer.ID);
                                if (PeerDisconnected != null)
                                    PeerDisconnected.Invoke(this, peer);
                            }
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        if (peer == null)
                            break;

						ShipInboundMessage msg = new ShipInboundMessage(im);

                        if (peer.Ship == null)
                        {
                            if (msg.Code == MessageCodes.NewShip)
                            {
								WantNewShip ns = WantNewShip.Unpack(im);
								peer.DesiredShipAttributes = ns.Requirements;

                                if (PeerWantsNewShip != null)
                                    PeerWantsNewShip.Invoke(this, peer);

                                if (peer.Ship == null)
                                    ForceDisconnect(peer);
                                else
                                {
                                    NetOutgoingMessage responce = peer.SocketPeer.Peer.CreateMessage();
									AssignShip s = new AssignShip();
									s.ShipID = peer.ShipID;
									s.Name = peer.Ship.Name;
									s.ClassName = peer.Ship.ClassName;
									s.Pack(responce);
                                    peer.SocketPeer.SendMessage(responce, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }
                            else if (msg.Code == MessageCodes.ExistingShip)
                            {
								WantExistingShip es = WantExistingShip.Unpack(im);

                                peer.ShipID = es.ShipID;
								peer.DesiredShipAttributes = es.Requirements;

								if (PeerWantsOldShip != null)
                                    PeerWantsOldShip.Invoke(this, peer);

                                if (peer.Ship == null)
                                    ForceDisconnect(peer);
                                else
                                {
                                    NetOutgoingMessage responce = peer.SocketPeer.Peer.CreateMessage();
									AssignShip s = new AssignShip();
									s.ShipID = peer.ShipID;
									s.Name = peer.Ship.Name;
									s.ClassName = peer.Ship.ClassName;
									s.Pack(responce);
									peer.SocketPeer.SendMessage(responce, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }
                        }
                        else
                        {
                            peer.Ship.InboundMessages.Add(new ShipInboundMessage(im));
                        }
                        break;
                }
            }
            PendingMessages.RemoveRange(0, toProcess);
        }
    }
}
