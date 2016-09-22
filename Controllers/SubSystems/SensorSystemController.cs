using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core;
using Agrotera.Core.Entities;

namespace Agrotera.DefaultControllers.SubSystems
{
	public class SensorSystemController : SubSystemController
	{
		public SensorSystemController() : base()
		{
			Priority = SubSystemController.NormalPriority;
		}

		public override void UpdateEntity(Tick tick, Entity entity)
		{
			Vessel vessel = entity as Vessel;
			if(vessel == null)
				return;

			double range = vessel.GetEffectiveSensorRange();
			if(vessel.Map == null)
				return;

			foreach(var e in vessel.Map.FindEntitiesInRadiusOf(entity, vessel.GetEffectiveSensorRange()))
			{
				if(!vessel.MappedItems.ContainsKey(e.ID))
					vessel.AddMappedItem(e);
			}

			if(vessel.ActiveScanTarget != null)
			{
				if(vessel.ActiveScanTarget.ScanProgress < 1)
				{
					double resolutionTime = vessel.Sensors.NominalResolutionTime * vessel.Sensors.ActualPowerLevel;
					if(resolutionTime > 0)
					{
						double paramPerSec = 1.0f / resolutionTime;
						double lastUpdateDelta = tick.Now - vessel.ActiveScanTarget.LastUpdate;
						if(lastUpdateDelta > vessel.ScanUpdateGranularity)
						{
							vessel.ActiveScanTarget.ScanProgress += paramPerSec * lastUpdateDelta;
							if(vessel.ActiveScanTarget.ScanProgress > 1)
								vessel.ActiveScanTarget.ScanProgress = 1;

							vessel.ActiveScanTarget.LastUpdate = tick.Now;
							vessel.ActiveScanTarget.ScienceScanValues = vessel.ActiveScanTarget.WorldEntity.GetScienceScanValues(1);

							if(vessel.ActiveScanTarget.WorldEntity as Vessel != null && vessel.ActiveScanTarget.ScanProgress > 0.75f)
								vessel.ActiveScanTarget.IdentifiedFaction = (vessel.ActiveScanTarget.WorldEntity as Vessel).Owner;

							if(vessel.ActiveScanTarget.ScanProgress >= 1 && !vessel.ScienceDB.ContainsKey(vessel.ActiveScanTarget.ScienceID))
							{
								// if the ship doesn't know about the thing they finished scanning, update the database
								vessel.AddScienceItem(vessel.ScienceDB[vessel.ActiveScanTarget.ScienceID]);
							}

							vessel.UpdateMapedItem(vessel.ActiveScanTarget);
						}
					}
				}
				else
				{
					if(tick.Now - vessel.ActiveScanTarget.LastUpdate > vessel.ActiveTargetRescanTime)
					{
						vessel.ActiveScanTarget.LastUpdate = tick.Now;
						vessel.ActiveScanTarget.ScienceScanValues = vessel.ActiveScanTarget.WorldEntity.GetScienceScanValues(1);

						vessel.UpdateMapedItem(vessel.ActiveScanTarget);
					}
				}
			}
		}
	}
}
