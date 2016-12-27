using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Types
{
    public class Vector3D
    {
        public double X = 0;
        public double Y = 0;
        public double Z = 0;

        public Vector3D() { }
        public Vector3D(double x, double y, double z) { X = x; Y = y; Z = z; }
        public Vector3D(Vector2D v) { X = v.X; Y = v.Y; }
        public Vector3D(Vector3D v) { X = v.X; Y = v.Y; Z = v.Z; }

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

        public static readonly Vector3D Zero = new Vector3D(0, 0, 0);
        public static readonly Vector3D UnitX = new Vector3D(1, 0, 0);
        public static readonly Vector3D UnitY = new Vector3D(0, 1, 0);
        public static readonly Vector3D UnitZ = new Vector3D(0, 0, 1);

        public static readonly Vector3D UnitXY = new Vector3D(1, 1, 0);
        public static readonly Vector3D Invalid = new Vector3D(double.MinValue, double.MaxValue, double.MaxValue);

        public static Vector3D operator *(Vector3D v, double f)
        {
            return new Vector3D(v.X * f, v.Y * f, v.Z * f);
        }

        public static Vector3D operator *(double f, Vector3D v)
        {
            return new Vector3D(v.X * f, v.Y * f, v.Z * f);
        }

        public static Vector3D operator /(Vector3D v, double f)
        {
            return new Vector3D(v.X / f, v.Y / f, v.Z / f);
        }

        public static Vector3D operator /(double f,Vector3D v)
        {
            return new Vector3D(v.X / f, v.Y / f, v.Z / f);
        }

        public static Vector3D operator +(Vector3D v, double f)
        {
            return new Vector3D(v.X + f, v.Y + f, v.Z + f);
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static double DotProduct(Vector3D p1, Vector3D p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
        }

        public static Vector3D CrossProduct(Vector3D p1, Vector3D p2)
        {
            return new Vector3D(p1.Y * p2.Z - p1.Z * p2.Y,
                p1.Z * p2.X - p1.X * p2.Z,
                p1.X * p2.Y - p1.Y * p2.X);
        }

        public static Vector3D Normalize(Vector3D p1)
        {
            Vector3D v = new Vector3D(p1);
            v.Normailize();
            return v;
        }

        public static double Distance(Vector3D p1, Vector3D p2)
        {
            return (p1 - p2).Length();
        }

        public static double DistanceSquared(Vector3D p1, Vector3D p2)
        {
            return (p1 - p2).LengthSquared();
        }

        public static bool ExactEqual(Vector3D p1, Vector3D p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
        }

        public static bool NearEqual(Vector3D p1, Vector3D p2, double tolerance)
        {
            return Math.Abs(p1.X - p2.X) <= tolerance && Math.Abs(p1.Y - p2.Y) <= tolerance && Math.Abs(p1.Z - p2.Z) <= tolerance;
        }

		public static bool TryParse(string text, out Vector3D vec)
		{
			vec = new Vector3D(0, 0, 0);

			string[] bits = text.Split(",".ToCharArray(),3);
			if(bits.Length != 3)
				return false;

			return double.TryParse(bits[0], out vec.X) && double.TryParse(bits[1], out vec.Y) && double.TryParse(bits[2], out vec.Z);
		}

    }
}
