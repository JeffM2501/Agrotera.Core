using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region --- License ---
/*
Copyright (c) 2006 - 2008 The Open Toolkit library.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

namespace Core.Types
{
    public class QuaternionD
    {
        public Vector3D XYZ;
        public double W;

        public double X { get { return XYZ.X; } }
        public double Y { get { return XYZ.Y; } }
        public double Z { get { return XYZ.Z; } }


        public QuaternionD(Vector3D v, double w)
        {
            XYZ = new Vector3D(v);
            W = w;
        }

        public QuaternionD(QuaternionD q)
        {
            XYZ = new Vector3D(q.XYZ);
            W = q.W;
        }

        public QuaternionD(double x, double y, double z, double w)
        {
            XYZ = new Vector3D(x, y, z);
            W = w;
        }

        public double Length()
        {
            return System.Math.Sqrt(W * W + XYZ.LengthSquared());
        }

        public double LengthSquared()
        {
            return W * W + XYZ.LengthSquared();
        }

        public void Normalize()
        {
            double scale = 1.0f / Length();
            XYZ *= scale;
            W *= scale;
        }

        public void Conjugate()
        {
            XYZ = XYZ * -1;
        }

        public static QuaternionD operator * (QuaternionD lhs, QuaternionD rhs)
        {
            double x = lhs.X * rhs.W + lhs.Y * rhs.Z - lhs.Z * rhs.Y + lhs.W * rhs.X;
            double y = -lhs.X * rhs.Z + lhs.Y * rhs.W + lhs.Z * rhs.X + lhs.W * rhs.Y;
            double z = lhs.X * rhs.Y - lhs.Y * rhs.X + lhs.Z * rhs.W + lhs.W * rhs.Z;
            double w = -lhs.X * rhs.X - lhs.Y * rhs.Y - lhs.Z * rhs.Z + lhs.W * rhs.W;

            return new QuaternionD(x, y, z, w);
        }

        public static QuaternionD Invert(QuaternionD q)
        {
            QuaternionD result;
            double lengthSq = q.LengthSquared();
            if (lengthSq != 0.0)
            {
                double i = 1.0f / lengthSq;
                result = new QuaternionD(q.XYZ * -i, q.W * i);
            }
            else
            {
                result = q;
            }
            return result;
        }

        public void ToAxisAngle(out Vector3D axis, out double angle)
        {
            QuaternionD q = new QuaternionD(this);
            if (Math.Abs(q.W) > 1.0f)
                q.Normalize();


            angle = 2.0f * System.Math.Acos(q.W); // angle
            double den = System.Math.Sqrt(1.0 - q.W * q.W);
            if (den > 0.00001)
                axis = q.XYZ / den;
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                axis = Vector3D.UnitX;
            }
        }

        public static QuaternionD FromAxisAngle(Vector3D axis, double angle)
        {
            if (axis.LengthSquared() == 0.0f)
                return Identity;

            QuaternionD result = Identity;

            angle *= 0.5f;
            axis = Vector3D.Normalize(axis);
            result.XYZ = axis * System.Math.Sin(angle);
            result.W = System.Math.Cos(angle);

            result.Normalize();
            return result;
        }

        public static QuaternionD Slerp(QuaternionD q1, QuaternionD q2, double blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared() == 0.0f)
            {
                if (q2.LengthSquared() == 0.0f)
                {
                    return Identity;
                }
                return q2;
            }
            else if (q2.LengthSquared() == 0.0f)
            {
                return q1;
            }

            double cosHalfAngle = q1.W * q2.W + Vector3D.DotProduct(q1.XYZ, q2.XYZ);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return q1;
            }
            else if (cosHalfAngle < 0.0)
            {
                q2.XYZ = q2.XYZ * -1;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            double blendA;
            double blendB;
            if (cosHalfAngle < 0.9999)
            {
                // do proper slerp for big angles
                double halfAngle = Math.Acos(cosHalfAngle);
                double sinHalfAngle = Math.Sin(halfAngle);
                double oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0 - blend;
                blendB = blend;
            }

            QuaternionD result = new QuaternionD(blendA * q1.XYZ + blendB * q2.XYZ, blendA * q1.W + blendB * q2.W);
            if (result.LengthSquared() > 0.0f)
            {
                result.Normalize();
                return result;
            }
            else
                return Identity;
        }

        public static QuaternionD RotateTo(Vector3D from, Vector3D to)
        {
            Vector3D f = Vector3D.Normalize(from);
            Vector3D t = Vector3D.Normalize(to);

            Vector3D axis = Vector3D.CrossProduct(from, to);

            double angle = Math.Acos(Vector3D.DotProduct(from, to));

            return QuaternionD.FromAxisAngle(axis, angle);
        }

        public static Vector3D operator *(QuaternionD quat, Vector3D vec)
        {
            double num = quat.X * 2.0;
            double num2 = quat.Y * 2.0;
            double num3 = quat.Z * 2.0;
            double num4 = quat.X * num;
            double num5 = quat.Y * num2;
            double num6 = quat.Z * num3;
            double num7 = quat.X * num2;
            double num8 = quat.X * num3;
            double num9 = quat.Y * num3;
            double num10 = quat.W * num;
            double num11 = quat.W * num2;
            double num12 = quat.W * num3;

            Vector3D result = new Vector3D();
            result.X = (1.0 - (num5 + num6)) * vec.X + (num7 - num12) * vec.Y + (num8 + num11) * vec.Z;
            result.Y = (num7 + num12) * vec.X + (1.0 - (num4 + num6)) * vec.Y + (num9 - num10) * vec.Z;
            result.Z = (num8 - num11) * vec.X + (num9 + num10) * vec.Y + (1.0 - (num4 + num5)) * vec.Z;
            return result;
        }

        public static QuaternionD LookAt(Vector3D sourcePoint, Vector3D destPoint, Vector3D up)
        {
            Vector3D forwardVector = Vector3D.Normalize(destPoint - sourcePoint);

            double dot = Vector3D.DotProduct(Vector3D.UnitX, forwardVector);

            if (Math.Abs(dot - (-1.0)) < 0.000001)
            {
                return new QuaternionD(up.X, up.Y, up.Z, Math.PI);
            }
            if (Math.Abs(dot - (1.0)) < 0.000001)
            {
                return QuaternionD.Identity;
            }

            double rotAngle = Math.Acos(dot);
            Vector3D rotAxis = Vector3D.CrossProduct(Vector3D.UnitX, forwardVector);
            rotAxis = Vector3D.Normalize(rotAxis);
            return FromAxisAngle(rotAxis, rotAngle);
        }

        public readonly static QuaternionD Identity = new QuaternionD(0, 0, 0, 1);
    }
}
