using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

using Agrotera.Core.Types;

namespace Agrotera.ShipLink
{
	public static class NetworkVectorUtils
	{
		public static void Write(this NetOutgoingMessage msg, Vector3F vec)
		{
			msg.Write(vec.X);
			msg.Write(vec.Y);
			msg.Write(vec.Z);
		}

		public static void Write(this NetOutgoingMessage msg, Vector2F vec)
		{
			msg.Write(vec.X);
			msg.Write(vec.Y);
		}

		public static Vector3F ReadVector3F(this NetIncomingMessage msg)
		{
			return new Vector3F(msg.ReadDouble(), msg.ReadDouble(), msg.ReadDouble());
		}

		public static Vector2F ReadVector2F(this NetIncomingMessage msg)
		{
			return new Vector2F(msg.ReadDouble(), msg.ReadDouble());
		}
	}
}
