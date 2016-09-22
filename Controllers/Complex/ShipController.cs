using System;

using Agrotera.DefaultControllers.SubSystems;
using Agrotera.DefaultControllers.Simple;
using Agrotera.Core;
using Agrotera.Core.Entities;

namespace Agrotera.DefaultControllers.Complex
{
	public class ShipController : VesselController
	{
		public class ShipMover : SubSystemController
		{
			public ShipMover() : base()
			{
				Priority = SubSystemController.NormalPriority-1;
			}

			public override void UpdateEntity(Tick tick, Entity entity)
			{
				base.UpdateEntity(tick, entity);

				Ship ship = entity as Ship;
				if(ship == null)
					return;
			}

			protected virtual void ProcessHeadingChange(Ship ship, Tick tick)
			{
				if(ship.DesiredHeading != ship.Rotation)
				{
					double newHeading = Utilities.LinInterpValue(ship.Rotation, ship.DesiredHeading, ship.ManeuverSystem.GetEffectiveTurnSpeed(), tick.Delta);
					ship.Spin = (newHeading - ship.Rotation) / tick.Delta;
				}
				else
					ship.Spin = 0;
			}
		}

		public ShipController() : base()
		{
			AddSubSystem(new ShipMover());
		}
	}
}
