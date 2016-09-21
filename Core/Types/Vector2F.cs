using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Core.Types
{
    public class Vector2F
    {
        public double X = 0;
        public double Y = 0;

        public Vector2F() { }
        public Vector2F(double x, double y) { X = x; Y = y; }
        public Vector2F(Vector2F v) { X = v.X; Y = v.Y; }

        public Vector2F(Vector3F v) { X = v.X; Y = v.Y; }

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

		public static readonly Vector2F Zero = new Vector2F(0, 0);
        public static readonly Vector2F UnitX = new Vector2F(1, 0);
        public static readonly Vector2F UnitY = new Vector2F(0, 1);
        public static readonly Vector2F UnitXY = new Vector2F(1, 1);
        public static readonly Vector2F Invalid = new Vector2F(double.MinValue, double.MaxValue);

        public static Vector2F operator *(Vector2F v, double f)
        {
            return new Vector2F(v.X * f, v.Y * f);
        }

        public static Vector2F operator +(Vector2F v, double f)
        {
            return new Vector2F(v.X + f, v.Y + f);
        }

        public static Vector2F operator +(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2F operator -(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2F FromAngle(double angle, double radius)
        {
            return new Vector2F(Math.Cos(angle) * radius, Math.Sin(angle) * radius);
        }

        public static double Distance(Vector2F p1, Vector2F p2)
        {
            return (p1 - p2).Length();
        }

		public static bool TryParse(string text, out Vector2F vec)
		{
			vec = new Vector2F(0, 0);

			string[] bits = text.Split(",".ToCharArray(), 3);
			if(bits.Length != 3)
				return false;

			return double.TryParse(bits[0], out vec.X) && double.TryParse(bits[1], out vec.Y);
		}
	}
}
