using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using Core.Types;

namespace NetworkMessages
{
	public static class VectorUtils
	{
		public static Vector3D ReadVector3D(this NetIncomingMessage msg)
		{
			return new Vector3D(msg.ReadDouble(), msg.ReadDouble(), msg.ReadDouble());
		}

		public static Vector2D ReadVector2D(this NetIncomingMessage msg)
		{
			return new Vector2D(msg.ReadDouble(), msg.ReadDouble());
		}

        public static QuaternionD ReadQuaternionD(this NetIncomingMessage msg)
        {
            return new QuaternionD(msg.ReadDouble(), msg.ReadDouble(), msg.ReadDouble(), msg.ReadDouble());
        }

        public static void Write(this NetOutgoingMessage msg, Vector3D vec)
		{
			msg.Write(vec.X);
			msg.Write(vec.Y);
			msg.Write(vec.Z);
		}

		public static void Write(this NetOutgoingMessage msg, Vector2D vec)
		{
			msg.Write(vec.X);
			msg.Write(vec.Y);
		}

        public static void Write(this NetOutgoingMessage msg, QuaternionD q)
        {
            msg.Write(q.XYZ);
            msg.Write(q.W);
        }
    }
}
