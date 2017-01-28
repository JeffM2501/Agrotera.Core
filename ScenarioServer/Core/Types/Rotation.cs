using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Types
{
    public class Rotation
    {
        public double Angle = 0;

        public static readonly double RadCon = Math.PI / 180;
        public static readonly double DegCon = 1 / RadCon;

        protected static readonly double AngularTolerance = 2.6e-7;


        public Rotation() { }
        public Rotation( double angle) { Angle = angle; }

        public Rotation(Rotation r)
        {
            Angle = r.Angle;
        }

        public Rotation Clone()
        {
            return new Rotation(this);
        }

        public static double AngleBetween(Rotation lhs, Rotation rhs)
        {
            return Math.Abs(lhs.Angle-rhs.Angle);
        }

        public static Rotation Inverse(Rotation e)
        {
            return new Rotation(e.Angle * -1);
        }

        public static Rotation operator +(Rotation lhs, Rotation rhs)
        {
            return new Rotation(lhs.Angle+rhs.Angle);
        }

        public static Rotation operator -(Rotation lhs, Rotation rhs)
        {
            return new Rotation(lhs.Angle - rhs.Angle);
        }

        public static Rotation operator *(Rotation lhs, double rhs)
        {
            return new Rotation(lhs.Angle * rhs);
        }

        public static Rotation operator *(Rotation lhs, Rotation rhs)
        {
            return new Rotation(lhs.Angle - rhs.Angle);
        }

        public static Rotation operator /(Rotation lhs, double rhs)
        {
            return new Rotation(lhs.Angle/rhs);
        }

        public static Rotation operator /(Rotation lhs, Rotation rhs)
        {
            return new Rotation(lhs.Angle / rhs.Angle);
        }

        public Vector3D TransformVec(Vector3D v)
        {
            double cs = Math.Cos(RadCon * Angle);
            double sn = Math.Sin(RadCon * Angle);
            
            return new Vector3D(v.X * cs - v.Y * sn, v.X * sn + v.Y * cs,v.Z);
        }

        public Vector3D ToVector3D()
        {
            return new Vector3D(Math.Cos(RadCon * Angle), Math.Sin(RadCon * Angle), 0);
        }

        public static Rotation FromVector3D(Vector3D v)
        {
            var nv = Vector3D.Normalize(v);
            return new Rotation(DegCon * Math.Atan2(nv.Y, nv.X));
        }

        public static Rotation FromLocation(Location v)
        {
            var nv = Location.Normalize(v);
            return new Rotation(DegCon * Math.Atan2(nv.X, nv.Y));
        }

        private static double Round(double x)
        {
            return Math.Floor(x + 0.5);
        }

        private static double RoundTo(double x, double y)
        {
            return Round(x / y) * y;
        }

        private static double NormAngle(double angle)
        {
            //    return angle - Math.Floor(angle / 360.0) * 360.0;

            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;
            return angle;
        }

        public void Normailzie()
        {
            Angle = NormAngle(Angle);
        }

        public void Clamp(double maxAngle)
        {
            Angle = Math.Min(maxAngle,Math.Abs(Angle)) * Math.Sin(Angle);
        }

        public static Rotation ShortRotationTo(Rotation from, Rotation to)
        {
            double diff = (to.Angle - from.Angle + 180) % 360 - 180;
            return new Rotation(diff < -180 ? diff + 360 : diff);
        }

        public static readonly Rotation Zero = new Rotation(0);
        public static readonly Rotation Invalid = new Rotation(double.MinValue);
    }
}
