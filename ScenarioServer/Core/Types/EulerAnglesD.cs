using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Types
{
    public class EulerAnglesD
    {
        public double Pitch = 0;
        public double Roll = 0;
        public double Yaw = 0;

        public EulerAnglesD() { }
        public EulerAnglesD(double p, double r, double y) { Pitch = p; Roll = r; Yaw = y; Normalize(); }
        public EulerAnglesD(EulerAnglesD v) { Pitch = v.Pitch; Roll = v.Roll; Yaw = v.Yaw; Normalize(); }

        public EulerAnglesD(QuaternionD q)
        {
            QuaternionD.ToEuler(q, out Roll, out Pitch, out Yaw);

            Roll *= DegCon;
            Pitch *= DegCon;
            Yaw *= DegCon;
            Normalize();
        }

        protected static readonly double RadCon = Math.PI / 180;
        protected static readonly double DegCon = 1/ RadCon;

        public QuaternionD ToQuaterneonD()
        {
            return QuaternionD.FromEuler(Pitch * RadCon, Roll * RadCon, Yaw * RadCon);
        }

        private double NormAngle(double n)
        {
            if (n > 180)
                return n - 360;
            if (n < -180)
                return n + 360;
            return n;
        }

        public void Normalize()
        {
            Pitch = NormAngle(Pitch);
            Roll = NormAngle(Roll);
            Yaw = NormAngle(Yaw);
        }

        public Vector3D TransformVec(Vector3D v)
        {
            return ToQuaterneonD() * v;
        }

        public static double AngleBetween(EulerAnglesD lhs, EulerAnglesD rhs)
        {
            return Math.Acos((lhs.ToQuaterneonD() * QuaternionD.Invert(rhs.ToQuaterneonD())).W);
        }

        public static EulerAnglesD Inverse(EulerAnglesD e)
        {
            return new EulerAnglesD(QuaternionD.Invert(e.ToQuaterneonD()));
        }

        public static EulerAnglesD operator +(EulerAnglesD lhs, EulerAnglesD rhs)
        {
            return new EulerAnglesD(lhs.Pitch + rhs.Pitch, lhs.Roll + rhs.Roll, lhs.Yaw + rhs.Yaw);
        }

        public static EulerAnglesD operator -(EulerAnglesD lhs, EulerAnglesD rhs)
        {
            return new EulerAnglesD(lhs.Pitch - rhs.Pitch, lhs.Roll - rhs.Roll, lhs.Yaw - rhs.Yaw);
        }

        public static EulerAnglesD operator *(EulerAnglesD lhs, double rhs)
        {
            return new EulerAnglesD(lhs.Pitch * rhs, lhs.Roll * rhs, lhs.Yaw * rhs);
        }

        public static EulerAnglesD operator *(EulerAnglesD lhs, EulerAnglesD rhs)
        {
            return new EulerAnglesD(lhs.Pitch * rhs.Pitch, lhs.Roll * rhs.Roll, lhs.Yaw * rhs.Yaw);
        }

        public static EulerAnglesD operator /(EulerAnglesD lhs, double rhs)
        {
            return new EulerAnglesD(lhs.Pitch / rhs, lhs.Roll / rhs, lhs.Yaw / rhs);
        }

        public static EulerAnglesD operator /(EulerAnglesD lhs, EulerAnglesD rhs)
        {
            return new EulerAnglesD(lhs.Pitch / rhs.Pitch, lhs.Roll / rhs.Roll, lhs.Yaw / rhs.Yaw);
        }

        public static EulerAnglesD FromQuaternion(QuaternionD q)
        {
            EulerAnglesD e = new EulerAnglesD();
            QuaternionD.ToEuler(q, out e.Roll, out e.Pitch, out e.Yaw);

            e.Roll *= DegCon;
            e.Pitch *= DegCon;
            e.Yaw *= DegCon;

            return e;
        }

        public static readonly EulerAnglesD Zero = new EulerAnglesD(0, 0, 0);
    }
}
