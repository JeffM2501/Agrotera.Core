using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

using Core.Types;
using NetworkMessages;
using NetworkMessages.ConsoleMessages;

using Console.Client.Consoles;

namespace Console.Client
{
    public class ConsoleConnection
    {
        public List<ConsoleInboundMessage> InboundMessages = new List<ConsoleInboundMessage>();
        public List<ConsoleOutboundMessage> OutboundMessages = new List<ConsoleOutboundMessage>();

        protected NetClient SocketClient = null;

        public Dictionary<int, Console> Consoles = new Dictionary<int, Console>();

        public class ConnectedEventArgs : EventArgs
        {
            public List<string> ConsoleList = new List<string>();
        }

        public event EventHandler<ConnectedEventArgs> Connected = null;
        public event EventHandler Disconnected = null;

        public event EventHandler ConsolesAssigned = null;

        public ConsoleConnection(string host, int port)
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

            if (Consoles.Count > 0)
                UpdateConsoles();

            UpdateOutbound();
        }

        protected void UpdateConsoles()
        {
            foreach(var msg in InboundMessages)
            {
                List<Console> ConsolesToProcess = new List<Console>();
                if (msg.AffectedConsoles.Count == 0 || msg.AffectedConsoles[0] == -1)
                    ConsolesToProcess.AddRange(Consoles.Values);
                else
                {
                    foreach(int cID in msg.AffectedConsoles)
                    {
                        if (Consoles.ContainsKey(cID))
                            ConsolesToProcess.Add(Consoles[cID]);
                    }
                }

                foreach (var c in ConsolesToProcess)
                    c.ProcessMessage(msg);
            }
            InboundMessages.Clear();
        }

        protected void SetupConsoles(List<string> consoleNames)
        {
            Consoles.Clear();
            int id = 0;

            foreach (string name in consoleNames)
            {

                if (name == "Helm")
                    Consoles.Add(id,new HelmConsole(id));
                else
                    Consoles.Add(id,new Console());

                id++;
            }

            if (ConsolesAssigned != null)
                ConsolesAssigned.Invoke(this, EventArgs.Empty);
        }

        public void UpdateOutbound()
        {
            if (Consoles.Count == 0 || OutboundMessages.Count == 0)
                return;

            int count = OutboundMessages.Count;
            if (count > 10)
                count = 10;

            for (int i = 0; i < count; i++)
            {
                NetOutgoingMessage msg = SocketClient.CreateMessage();
                OutboundMessages[i].Pack(msg);
                SocketClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
            }
            SocketClient.FlushSendQueue();
            OutboundMessages.RemoveRange(0, count);
        }

        protected List<NetIncomingMessage> PendingMessages = new List<NetIncomingMessage>();

        public void UpdateInbound()
        {
            if (SocketClient == null)
                return;

            SocketClient.ReadMessages(PendingMessages);

            int toProcess = PendingMessages.Count;
            if (toProcess > 10)
                toProcess = 10;
            if (toProcess == 0)
                return;

            for (int i = 0; i < toProcess; i++)
            {
                NetIncomingMessage im = PendingMessages[i];

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
                            if (Connected != null)
                            {
                                ConnectedEventArgs args = new ConnectedEventArgs();
                                Connected(this, args);

                                NetOutgoingMessage hail = SocketClient.CreateMessage();

                                ConnectToShip cs = new ConnectToShip();
                                cs.Consoles = args.ConsoleList;
                                cs.Pack(hail);
                             
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
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            if (Disconnected != null)
                                Disconnected.Invoke(this, EventArgs.Empty);

                            SocketClient = null;
                            PendingMessages.Clear();
                            return;

                        }
                        break;

                    case NetIncomingMessageType.Data:
                        if (SocketClient == null)
                            break;

                        var msg = new ConsoleInboundMessage(im);

                        if (Consoles.Count == 0)
                        {
                            if (msg.Code == ConsoleMessageCodes.ConnectToShip)
                                SetupConsoles(ConnectToShip.Unpack(im).Consoles);
                        }
                        else
                        {
                            int count = im.ReadInt32();
                            for (int j = 0; j < count; j++)
                                msg.AffectedConsoles.Add(im.ReadInt32());

                            InboundMessages.Add(msg);
                        }
                        break;
                }
            }
            SocketClient.FlushSendQueue();
            PendingMessages.RemoveRange(0, toProcess);
        }
    }
}
