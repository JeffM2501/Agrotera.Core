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
            else if (cosHalfAngle < 0.0f)
            {
                q2.XYZ = q2.XYZ * -1;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            double blendA;
            double blendB;
            if (cosHalfAngle < 0.999f)
            {
                // do proper slerp for big angles
                double halfAngle = (double)System.Math.Acos(cosHalfAngle);
                double sinHalfAngle = (double)System.Math.Sin(halfAngle);
                double oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (double)System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (double)System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
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

        public readonly static QuaternionD Identity = new QuaternionD(0, 0, 0, 1);
    }
}
