using Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioServer.Scenarios.Controller
{
    public static class ControllerTools
    {
        private static Random RNG = new Random();

        private static Vector3D ZoneSize = new Vector3D();

        public static void SetBounds(double x, double y, double Z)
        {
            ZoneSize.X = x;
            ZoneSize.Y = y;
            ZoneSize.Z = Z;
        }

        public static Location RandomPostion()
        {
            return new Location(RandomVectorParam() * ZoneSize.X,
                                RandomVectorParam() * ZoneSize.Y, 0);
        }

        public static Vector3D RandomVector(double magitude)
        {
            return new Vector3D(RandomVectorParam() * magitude,
                                RandomVectorParam() * magitude,
                                0);
        }

        public static Location RandomPostionRelativeTo(Location position, double minDistance, double maxDistance)
        {
            return position + RandomVector(RandomInRange(minDistance, maxDistance));
        }

        public static double RandomVectorParam()
        {
            return (RNG.NextDouble() - 0.5) * 2.0;
        }

        public static double RandomInRange(double max)
        {
            return (RNG.NextDouble() * max);
        }

        public static double RandomInRange(double min, double max)
        {
            return min + RandomInRange(max - min);
        }
    }
}
