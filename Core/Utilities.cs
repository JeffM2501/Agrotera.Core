using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Agrotera.Core.Types;

namespace Agrotera.Core
{
    public static class Utilities
    {
        public static Random RNG = new Random((int)DateTime.Now.Ticks);

        public static double LinInterpValue(double value, double target, double speed, double dt)
        {
            double delta = target - value;

            double maxIncrement = speed * dt;
            if (Math.Abs(delta) >= maxIncrement)
                return target;

            return value + (Math.Sign(delta) * maxIncrement);
        }

        public static double NormalizeAngle(double angle)
        {
            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;

            return angle;
        }

        public static double RandomAngle()
        {
            return RandomPosNegParam() * 180f;
        }

        public static double RandomPosNegParam()
        {
            return (double)((RNG.NextDouble() - 0.5) * 2);
        }

        public static UInt64 RandomUInt64()
        {
            byte[] b = new byte[sizeof(UInt64)];

            RNG.NextBytes(b);
            return BitConverter.ToUInt64(b, 0);
        }

        public static T RandomElement<T>(T[] vals)
        {
            return vals[RNG.Next(0, vals.Length)];
        }

        public static Vector2F RandomPostion(Vector2F size)
        {
            return new Vector2F(RNG.Next((int)(size.X * -0.5), (int)(size.X - 0.5)), RNG.Next((int)(size.Y * -0.5), (int)(size.Y - 0.5)));
        }

        public static Vector3F RandomPostion(Vector3F size)
        {
            return new Vector3F(RNG.Next((int)(size.X * -0.5), (int)(size.X - 0.5)), RNG.Next((int)(size.Y * -0.5), (int)(size.Y - 0.5)), RNG.Next((int)(size.Z * -0.5), (int)(size.Z - 0.5)));
        }

        public static Vector2F RandomPostionFurtherThan(Vector2F size, Vector2F postion, double minDistance)
        {
            Vector2F posible = RandomPostion(size);
            double distance = Vector2F.Distance(postion, posible);
            while (distance >= minDistance)
            {
                posible = RandomPostion(size);
                distance = Vector2F.Distance(postion, posible);
            }
            return posible;
        }

        public static Vector2F RandomPostionInbetween(Vector2F postion, double minDistance, double maxDistance)
        {
            double angle = RNG.NextDouble() * Math.PI * 2;
            double distance = RNG.NextDouble() * (maxDistance - minDistance);
            distance += minDistance;

            return Vector2F.FromAngle(angle, distance);
        }

        public static Vector3F RandomPostionFurtherThan(Vector3F size, Vector3F postion, double minDistance)
        {
            Vector3F posible = RandomPostion(size);
            double distance = Vector3F.Distance(postion, posible);
            while (distance >= minDistance)
            {
                posible = RandomPostion(size);
                distance = Vector3F.Distance(postion, posible);
            }
            return posible;
        }
    }
}
