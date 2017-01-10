using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Types
{
    public class Location
    {
        public double X = 0;
        public double Y = 0;
        public double Z = 0;

        public Location() { }
        public Location(double x, double y, double z) { X = x; Y = y; Z = z; }
        public Location(Vector2D v) { X = v.X; Y = v.Y; }

        public Location(Vector3D v) { X = v.X; Y = v.Y; Z = v.Z; }

        public Location(Location v) { X = v.X; Y = v.Y; Z = v.Z; }

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
            if (l == 0)
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

        public static readonly Location Zero = new Location(0, 0, 0);
        public static readonly Location UnitX = new Location(1, 0, 0);
        public static readonly Location UnitY = new Location(0, 1, 0);
        public static readonly Location UnitZ = new Location(0, 0, 1);

        public static readonly Location UnitXY = new Location(1, 1, 0);
        public static readonly Location Invalid = new Location(double.MinValue, double.MaxValue, double.MaxValue);

        public static Location operator *(Location v, double f)
        {
            return new Location(v.X * f, v.Y * f, v.Z * f);
        }

        public static Location operator *(double f, Location v)
        {
            return new Location(v.X * f, v.Y * f, v.Z * f);
        }

        public static Location operator /(Location v, double f)
        {
            return new Location(v.X / f, v.Y / f, v.Z / f);
        }

        public static Location operator /(double f, Location v)
        {
            return new Location(v.X / f, v.Y / f, v.Z / f);
        }

        public static Location operator +(Location v, double f)
        {
            return new Location(v.X + f, v.Y + f, v.Z + f);
        }

        public static Location operator +(Location v1, Location v2)
        {
            return new Location(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Location operator +(Vector3D v1, Location v2)
        {
            return new Location(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Location operator +(Location v1, Vector3D v2)
        {
            return new Location(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Location operator -(Location v1, Location v2)
        {
            return new Location(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Location operator -(Location v1, Vector3D v2)
        {
            return new Location(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Location operator -(Vector3D v1, Location v2)
        {
            return new Location(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static double DotProduct(Location p1, Location p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
        }

        public static Location CrossProduct(Location p1, Location p2)
        {
            return new Location(p1.Y * p2.Z - p1.Z * p2.Y,
                p1.Z * p2.X - p1.X * p2.Z,
                p1.X * p2.Y - p1.Y * p2.X);
        }

        public static Location Normalize(Location p1)
        {
            Location v = new Location(p1);
            v.Normailize();
            return v;
        }

        public static double Distance(Location p1, Location p2)
        {
            return (p1 - p2).Length();
        }

        public static double DistanceSquared(Location p1, Location p2)
        {
            return (p1 - p2).LengthSquared();
        }

        public static bool ExactEqual(Location p1, Location p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
        }

        public static bool NearEqual(Location p1, Location p2, double tolerance)
        {
            return Math.Abs(p1.X - p2.X) <= tolerance && Math.Abs(p1.Y - p2.Y) <= tolerance && Math.Abs(p1.Z - p2.Z) <= tolerance;
        }

        public static bool TryParse(string text, out Location vec)
        {
            vec = new Location(0, 0, 0);

            string[] bits = text.Split(",".ToCharArray(), 3);
            if (bits.Length != 3)
                return false;

            return double.TryParse(bits[0], out vec.X) && double.TryParse(bits[1], out vec.Y) && double.TryParse(bits[2], out vec.Z);
        }

        public static Vector3F FromRelativeDobules(Location p1, Location o)
        {
            return new Vector3F((float)(p1.X - o.X), (float)(p1.Y - o.Y), (float)(p1.Z = o.Z));
        }

        public static Vector3D VectorTo(Location from, Location to)
        {
            return new Vector3D(to.X - from.X, to.Y - from.Y, to.Z - from.Z);
        }

        public Location Clone()
        {
            return new Location(this);
        }
    }
}
