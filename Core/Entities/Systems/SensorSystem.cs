using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrotera.Core.Entities.Systems
{
    public class SensorSystem : VesselSystem
	{
        public SensorSystem()
		{
			Name = "Sensors";
			NominalPowerUseFactor = 1;
		}

        public double NominalRange = 100;
        public double NominalResolutionTime = 3;

		public double GetEffectiveRange()
		{
			return (double)Math.Max(10, NominalRange * ActualPowerLevel * GetPerformanceDegredation());
		}
	}
}
