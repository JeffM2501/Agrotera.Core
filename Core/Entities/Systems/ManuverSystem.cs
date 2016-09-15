using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrotera.Core.Entities.Systems
{
	public class ManuverSystem: VesselSystem
	{
		public ManuverSystem()
		{
			Name = "Maneuvering";
			UseDamage = true;
			NominalPowerUseFactor = 2;
		}

		public double NominalTurnSpeed = 90;

		public virtual double GetEffectiveTurnSpeed()
		{
			return (double)Math.Max(10, NominalTurnSpeed * ActualPowerLevel * GetPerformanceDegredation());
		}

	}
}
