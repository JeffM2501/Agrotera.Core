using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core;
using Agrotera.Core.Entities;


namespace Agrotera.DefaultControllers.SubSystems
{
	public class PowerSystemController : SubSystemController
	{
		public PowerSystemController() : base()
		{
			Priority = SubSystemController.PowerSystemPriority;
		}

		public override void UpdateEntity(Tick tick, Entity entity)
		{
			Vessel vessel = entity as Vessel;
			if(vessel == null)
				return;

			UpdatePowerSystems(vessel, tick);
		}

		protected virtual void UpdatePowerSystems(Vessel vessel, Tick tick)
		{
			double desiredPower = 0;
			foreach(var s in vessel.Systems.Values)
			{
				if(s.Generates())
					vessel.PowerBuffer += tick.Delta * s.GetGeneratedPower();
				else
					desiredPower += tick.Delta * s.GetDesiredPowerUse();

				s.ActualPowerLevel = s.DesiredPowerLevel;
			}

			double actualPower = 0;
			double scale = 1;
			if(desiredPower < vessel.PowerBuffer)  // power starved, go an compute a scale factor
			{
				if(vessel.PowerBuffer <= 0)
					scale = 0;
				else
					scale = desiredPower / vessel.PowerBuffer;
			}

			foreach(var s in vessel.Systems.Values)
			{
				s.ActualPowerLevel = s.DesiredPowerLevel * scale;
				s.Update(tick);
				actualPower += s.GetActualPowerUse();
			}

			vessel.PowerBuffer -= actualPower;
			if(vessel.PowerBuffer < 0)
				vessel.PowerBuffer = 0;
			if(vessel.PowerBuffer > vessel.MaxPowerBuffer)
				vessel.PowerBuffer = vessel.MaxPowerBuffer;
		}
	}
}
