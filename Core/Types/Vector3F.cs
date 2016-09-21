using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Core.Types
{
    public class Vector3F
    {
        public double X = 0;
        public double Y = 0;
        public double Z = 0;

        public Vector3F() { }
        public Vector3F(double x, double y, double z) { X = x; Y = y; Z = z; }
        public Vector3F(Vector2F v) { X = v.X; Y = v.Y; }
        public Vector3F(Vector3F v) { X = v.X; Y = v.Y; Z = v.Z; }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double LengthSquared()
        {
            return (X * X + Y * Y + Z * Z);
        }

		public bool Normailize()
		{
			double l = Length();
			if(l == 0)
				return false;

			X /= l;
			Y /= l;
			Z /= l;
			return true;
		}

        public override string ToString()
        {
            return X.ToString("F0") + "," + Y.ToString("F0") + "," + Z.ToString("F0");
        }

        public static readonly Vector3F Zero = new Vector3F(0, 0, 0);
        public static readonly Vector3F UnitX = new Vector3F(1, 0, 0);
        public static readonly Vector3F UnitY = new Vector3F(0, 1, 0);
        public static readonly Vector3F UnitZ = new Vector3F(0, 0, 1);

        public static readonly Vector3F UnitXY = new Vector3F(1, 1, 0);
        public static readonly Vector3F Invalid = new Vector3F(double.MinValue, double.MaxValue, double.MaxValue);

        public static Vector3F operator *(Vector3F v, double f)
        {
            return new Vector3F(v.X * f, v.Y * f, v.Z * f);
        }

        public static Vector3F operator +(Vector3F v, double f)
        {
            return new Vector3F(v.X + f, v.Y + f, v.Z + f);
        }

        public static Vector3F operator +(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3F operator -(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static double Distance(Vector3F p1, Vector3F p2)
        {
            return (p1 - p2).Length();
        }

        public static double DistanceSquared(Vector3F p1, Vector3F p2)
        {
            return (p1 - p2).LengthSquared();
        }

        public static bool ExactEqual(Vector3F p1, Vector3F p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
        }

        public static bool NearEqual(Vector3F p1, Vector3F p2, double tolerance)
        {
            return Math.Abs(p1.X - p2.X) <= tolerance && Math.Abs(p1.Y - p2.Y) <= tolerance && Math.Abs(p1.Z - p2.Z) <= tolerance;
        }

		public static bool TryParse(string text, out Vector3F vec)
		{
			vec = new Vector3F(0, 0, 0);

			string[] bits = text.Split(",".ToCharArray(),3);
			if(bits.Length != 3)
				return false;

			return double.TryParse(bits[0], out vec.X) && double.TryParse(bits[1], out vec.Y) && double.TryParse(bits[2], out vec.Z);
		}
    }
}
