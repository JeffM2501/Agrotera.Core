using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lidgren.Network;

namespace Agrotera.ShipLink
{
    public class NetworkMessage
    {
        public static readonly NetworkMessage Empty = new NetworkMessage();

		public enum PackingTypes
		{
			Reflection,
			BinSer,
			Custom
		}

        public virtual PackingTypes Packing { get { return PackingTypes.Reflection; } }

        public virtual NetOutgoingMessage Pack(NetOutgoingMessage msg) { return null;}
        public virtual void Unpack(NetIncomingMessage msg) {}
    }

	public class SimpleReflectionPackedNetworkMessage : NetworkMessage
	{
		public override PackingTypes Packing { get { return PackingTypes.Reflection; } }
	}

	public class BinarySerializedNetworkMessage : NetworkMessage
    {
		public override PackingTypes Packing { get { return PackingTypes.BinSer; } }
    }

    public class CustomPackedNetworkMessage : NetworkMessage
    {
		public override PackingTypes Packing { get { return PackingTypes.Custom; } }
    }
}
