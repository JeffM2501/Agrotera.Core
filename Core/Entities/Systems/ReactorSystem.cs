using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  Agrotera.Core.Entities.Systems
{
	public class ReactorSystem : VesselSystem
	{
		public ReactorSystem()
		{
			Name = "Reactor";
			UseDamage = true;
		}

		public double Fuel = 1000.0f;
		public double MaxFuel = 1000.0f;
		public double FuelEfficency = 100.0f; // power per fuel unit

		protected bool FuelStarved = false;

		public double AddFuel(double ammount)
		{
			Fuel += ammount;
			if(Fuel < MaxFuel)
				return 0;
			return Fuel - MaxFuel;
		}

		public override void Update(Tick tick)
		{
			double heatMultiplyer = 1.0f + (3 * HeatLevel);

			double fuelNeeded = ((Math.Abs(ActualPowerLevel) * tick.Delta) / FuelEfficency) *heatMultiplyer;
			if(Fuel < fuelNeeded)
				FuelStarved = true;
			else
				Fuel -= fuelNeeded;
		}

		public override double GetGeneratedPower()
		{
			if(!Generates() || FuelStarved)
				return 0;

			return -1.0f * NominalPowerUseFactor * DesiredPowerLevel * GetPerformanceDegredation();
		}
	}
}
