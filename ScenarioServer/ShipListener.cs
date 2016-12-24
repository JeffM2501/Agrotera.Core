using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using ScenarioServer.Classes;
using Entities;

namespace ScenarioServer
{
    public class ShipListener
    {
        public class Peer : EventArgs
        {
            public long ID = -1;
            public NetConnection SocketPeer = null;

            public int ShipID = -1;
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
            NetPeerConfiguration config = new NetPeerConfiguration("Simple Ship Host.0.0.1");
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
                    msg.Write(p.Ship.OutboundMessages[i].Code);
                    msg.Write(p.Ship.OutboundMessages[i].Payload);
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

                        int code = im.ReadInt32();

                        if (peer.Ship == null)
                        {
                            if (code == ShipMessage.NewShipCode)
                            {
                                if (PeerWantsNewShip != null)
                                    PeerWantsNewShip.Invoke(this, peer);

                                if (peer.Ship == null)
                                    ForceDisconnect(peer);
                                else
                                {
                                    NetOutgoingMessage responce = peer.SocketPeer.Peer.CreateMessage();
                                    responce.Write(ShipMessage.NewShipCode);
                                    responce.Write(peer.ShipID);
                                    peer.SocketPeer.SendMessage(responce, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }
                            else if (code == ShipMessage.ExistingShipCode)
                            {
                                peer.ShipID = im.ReadInt32();

                                if (PeerWantsOldShip != null)
                                    PeerWantsOldShip.Invoke(this, peer);

                                if (peer.Ship == null)
                                    ForceDisconnect(peer);
                                else
                                {
                                    NetOutgoingMessage responce = peer.SocketPeer.Peer.CreateMessage();
                                    responce.Write(ShipMessage.NewShipCode);
                                    responce.Write(peer.ShipID);
                                    peer.SocketPeer.SendMessage(responce, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }
                        }
                        else
                        {
                            ShipMessage msg = new ShipMessage();
                            msg.Timestamp = timestamp;
                            msg.Code = code;
                            msg.Payload = im.ReadString();
                            peer.Ship.InboundMessages.Add(msg);
                        }
                        break;
                }
            }
            PendingMessages.RemoveRange(0, toProcess);
        }
    }
}
