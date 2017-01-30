using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ShipClient;
using Lidgren.Network;
using Entities;
using NetworkMessages;
using NetworkMessages.ConsoleMessages;

namespace ConsoleServer
{
    public class ConsoleListener
    {
        public class Peer : EventArgs
        {
            public long ID = -1;
            public NetConnection SocketPeer = null;

            public List<string> ConnectedConsoles = new List<string>();

            public Peer(NetConnection p)
            {
                SocketPeer = p;
                ID = p.Peer.UniqueIdentifier;
            }

            public List<ConsoleInboundMessage> InboundMessages = new List<ConsoleInboundMessage>();
            public List<ConsoleOutboundMessage> OutboundMessages = new List<ConsoleOutboundMessage>();
        }

        public UserShip Ship = null;

        protected NetServer SocketHost = null;

        protected Dictionary<long, Peer> ConnectedPeers = new Dictionary<long, Peer>();

        public event EventHandler<Peer> PeerConnected = null;
        public event EventHandler<Peer> PeerDisconnected = null;
        public event EventHandler<Peer> PeerWantsConsoles = null;

        public ConsoleListener(int port, UserShip ship)
        {
            Ship = ship;
            Ship.MessagesProcessed += Ship_MessagesProcessed;
            Ship.PostionsUpdated += Ship_PostionsUpdated;

            NetPeerConfiguration config = new NetPeerConfiguration(ProtocollDefinition.ConsoleProtoString);
            config.AutoFlushSendQueue = true;
            config.MaximumConnections = 32;
            config.ConnectionTimeout = 100;
            config.Port = port;

            SocketHost = new NetServer(config);

            SocketHost.Start();
        }

        private void Ship_PostionsUpdated(object sender, EventArgs e)
        {
            // generate notifications

            UpdateOutbound();
        }

        private void Ship_MessagesProcessed(object sender, EventArgs e)
        {
            UpdateInbound();

            // process events
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
            foreach (var p in ConnectedPeers.Values)
            {
                if (p == null || p.OutboundMessages.Count == 0)
                    continue;

                int count = p.OutboundMessages.Count;
                if (count > 10)
                    count = 10;

                for (int i = 0; i < count; i++)
                {
                    NetOutgoingMessage msg = p.SocketPeer.Peer.CreateMessage();
                    p.OutboundMessages[i].Pack(msg);
                    p.SocketPeer.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
                }

                p.OutboundMessages.RemoveRange(0, count);
            }
        }

        List<NetIncomingMessage> PendingMessages = new List<NetIncomingMessage>();

        public void UpdateInbound()
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

                Peer peer = null;
                if (im.SenderConnection != null)
                    peer = FindPeer(im.SenderConnection.Peer.UniqueIdentifier);

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
                            if (ConnectedPeers.ContainsKey(peer.ID))
                                ConnectedPeers.Remove(peer.ID);

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

                        ConsoleInboundMessage msg = new ConsoleInboundMessage(im);

                        if (peer.ConnectedConsoles.Count == 0)
                        {
                            if (msg.Code == ConsoleMessageCodes.ConnectToShip)
                            {
                                ConnectToShip cs = ConnectToShip.Unpack(im);
                                peer.ConnectedConsoles = cs.Consoles;

                                if (PeerWantsConsoles != null)
                                    PeerWantsConsoles.Invoke(this, peer);


                                NetOutgoingMessage responce = peer.SocketPeer.Peer.CreateMessage();
                                cs.Pack(responce);
                                peer.SocketPeer.SendMessage(responce, NetDeliveryMethod.ReliableOrdered, 0);

                            }
                        }
                        else
                        {
                            peer.InboundMessages.Add(msg);
                        }
                        break;
                }
            }
            PendingMessages.RemoveRange(0, toProcess);
        }
    }
}
