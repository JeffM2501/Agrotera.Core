using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrotera.Core.Entities.Systems
{
	public class ShipRefuelingPort : VesselSystem
	{
		public double Fuel = 1000000;
		public double MaxFuelPerSecond = 10;

		public List<Vessel> DockedVessels = new List<Vessel>();
		public ShipRefuelingPort()
		{
			NominalPowerUseFactor = 10;
			Name = "Refueling Port";
		}

		public void DockVessel(Vessel vessel)
		{
			DockedVessels.Add(vessel);
		}
		public void UndockVessel(Vessel vessel)
		{
			DockedVessels.Remove(vessel);
		}

		public override void Update(Tick tick)
		{
			base.Update(tick);
			TransferFuel(tick);
		}

		public void TransferFuel(Tick tick)
		{
			if(DockedVessels.Count == 0)
				return;

			double param = 1.0f / DockedVessels.Count;
			double power = GetPowerUse() * param * tick.Delta;

			double fuelTransfer = MaxFuelPerSecond * param * NominalPowerUseFactor * tick.Delta;

			foreach (Vessel vessel in DockedVessels)
			{
				double canTransfer = fuelTransfer;
				if(canTransfer > Fuel)
					canTransfer = Fuel;

				Fuel -= canTransfer;

				if (canTransfer > 0)
					Fuel += vessel.Refuel(canTransfer, power);
			}
		}
	}
}
