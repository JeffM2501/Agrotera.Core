using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Types
{
    public class Vector2D
    {
        public double X = 0;
        public double Y = 0;

        public Vector2D() { }
        public Vector2D(double x, double y) { X = x; Y = y; }
        public Vector2D(Vector2D v) { X = v.X; Y = v.Y; }

        public Vector2D(Vector3D v) { X = v.X; Y = v.Y; }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double LengthSquared()
        {
            return (X * X + Y * Y);
        }

		public override string ToString()
		{
			return X.ToString("F0") + "," + Y.ToString("F0");
		}

		public bool Normailize()
		{
			double l = Length();
			if(l == 0)
				return false;

			X /= l;
			Y /= l;
			return true;
		}

		public static readonly Vector2D Zero = new Vector2D(0, 0);
        public static readonly Vector2D UnitX = new Vector2D(1, 0);
        public static readonly Vector2D UnitY = new Vector2D(0, 1);
        public static readonly Vector2D UnitXY = new Vector2D(1, 1);
        public static readonly Vector2D Invalid = new Vector2D(double.MinValue, double.MaxValue);

        public static Vector2D operator *(Vector2D v, double f)
        {
            return new Vector2D(v.X * f, v.Y * f);
        }

        public static Vector2D operator +(Vector2D v, double f)
        {
            return new Vector2D(v.X + f, v.Y + f);
        }

        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2D FromAngle(double angle, double radius)
        {
            return new Vector2D(Math.Cos(angle) * radius, Math.Sin(angle) * radius);
        }

        public static double Distance(Vector2D p1, Vector2D p2)
        {
            return (p1 - p2).Length();
        }

		public static bool TryParse(string text, out Vector2D vec)
		{
			vec = new Vector2D(0, 0);

			string[] bits = text.Split(",".ToCharArray(), 3);
			if(bits.Length != 3)
				return false;

			return double.TryParse(bits[0], out vec.X) && double.TryParse(bits[1], out vec.Y);
		}
	}
}
