using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

using Lidgren.Network;

namespace Agrotera.ShipLink
{
	public static class MessageFactory
	{
		public static string ProtocolVersionString = "Agrotera Ship Link 0.0.1";

        private class MessageTypeInfo
        {
            public Type ClassType = null;
            public NetworkMessage.PackingTypes Packing = NetworkMessage.PackingTypes.Reflection;
        }

		private static Dictionary<int, MessageTypeInfo> MessageIDs = new Dictionary<int, MessageTypeInfo>();

		static MessageFactory()
		{
			LoadAssembly(Assembly.GetExecutingAssembly());
		}

		public static void LoadAssembly(Assembly assembly)
		{
			Type netMsgType = typeof(NetworkMessage);

			foreach (var t in assembly.GetTypes())
			{
				if (t.IsSubclassOf(netMsgType))
				{
					int id = t.GetHashCode();
					if (MessageIDs.ContainsKey(id))
					{
						MessageIDs.Remove(id);
					}

                    try
                    {
                        MessageTypeInfo info = new MessageTypeInfo();
                        info.ClassType = t;
                        NetworkMessage msg = Activator.CreateInstance(t) as NetworkMessage;
                        info.Packing = msg.Packing;

                        MessageIDs.Add(id, info);
                    }
                    catch (Exception /*ex*/) { }
        
				}
			}
		}

		private static BinaryFormatter BinFormater = new BinaryFormatter();

		public static NetworkMessage ParseMessage(NetIncomingMessage msg)
		{
			if(msg.LengthBytes < sizeof(int))
				return NetworkMessage.Empty;

			int id = msg.ReadInt32();

            MessageTypeInfo t = null;
			lock(MessageIDs)
			{
				if(MessageIDs.ContainsKey(id))
					t = MessageIDs[id];
			}

			if(t == null)
				return NetworkMessage.Empty;

            NetworkMessage outMsg = null;
            outMsg = (NetworkMessage)Activator.CreateInstance(t.ClassType);

			switch(t.Packing)
			{
				case NetworkMessage.PackingTypes.Reflection:
					msg.ReadAllFields(outMsg);
					break;

				case NetworkMessage.PackingTypes.Custom:
					outMsg.Unpack(msg);
					break;

				case NetworkMessage.PackingTypes.BinSer:
					{
						int size = msg.ReadUInt16();
						byte[] b = msg.ReadBytes(size);
						MemoryStream ms = new MemoryStream(b);
						outMsg = (NetworkMessage)BinFormater.Deserialize(ms);
						ms.Close();
					}
					break;
			}
			return outMsg;
		}

		public static NetOutgoingMessage PackMessage(NetOutgoingMessage outMsg, NetworkMessage msg)
		{
            outMsg.Write(msg.GetType().GetHashCode());

			switch(msg.Packing)
			{
				case NetworkMessage.PackingTypes.Reflection:
					outMsg.WriteAllFields(msg);
					break;

				case NetworkMessage.PackingTypes.Custom:
					outMsg = msg.Pack(outMsg);
					break;

				case NetworkMessage.PackingTypes.BinSer:
					{
						MemoryStream ms = new MemoryStream();

						BinFormater.Serialize(ms, msg);
						byte[] b = ms.GetBuffer();
						outMsg.Write((UInt16)b.Length);
						outMsg.Write(b);
						ms.Close();
					}
					break;
			}

            return outMsg;
		}
	}
}
