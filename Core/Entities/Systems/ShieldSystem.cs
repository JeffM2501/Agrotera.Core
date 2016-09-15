using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrotera.Core.Entities.Systems
{
    /*
     * Shields work like a damage buffer
     * They have a capacitor of power
     * This capacitor has a max charge, represented by Status.Max
     * The current charge of the capacitor is represented by Status.Current
     * Hits take power from the capacitor and add heat.
     * When the capacitor is at zero damage is not absorbed
     * When the shield is powered it will recharge at a rate determined by the available power
     * Available power is modified by the damage and heat
     * WHen not powered the capictor will discharge at a fixed rate
     */

    public class ShieldStatus
	{
        public double Power = 0;
		public double Current = 0;
        public double Max = 0;
	}

    public class ShieldSystem : VesselSystem
    {
        public ShieldStatus Status = new ShieldStatus();

        protected bool Active = false;

        public double DischargeRage = 10;
        public double RechargeRage = 10;

        public ShieldSystem( string name, double maxShields, double powerUse)
		{
            Name = name;
			UseDamage = true;
            NominalPowerUseFactor = powerUse;

            Status.Max = maxShields;
		}

        public void ActivateShields(bool status)
        {
            Active = status;
        }

        public bool Activated()
        {
            return Active && GetPerformanceFactor() > 0;
        }

        public override void Update(Tick tick)
        {
            base.Update(tick);

            double factor = GetPerformanceFactor();

            Status.Current += factor * RechargeRage * tick.Delta;

            if (Status.Current > Status.Max)
                Status.Current = Status.Max;
            
            if (factor == 0)
            {
                Status.Current -= DischargeRage * tick.Delta;
                if (Status.Current < 0)
                    Status.Current = 0;
            }
        }

        public virtual double AbsorbHit(double force)
        {
            if (!Active)
                return force;

            HeatLevel += ((force/Status.Max) * 0.1f);
            Status.Current -= force;
            if (Status.Current < 0)
            {
                force = Status.Current * -1;
                Status.Current = 0;
            }
            else
                force = 0;

            return force;
        }

        // Damaged shields make more heat
        public override double GetHeatingDelta()
        {
            if (Active)
                return base.GetHeatingDelta() * (1 + (Damage * 0.5f));
            else
                return base.GetHeatingDelta();
        }

        // inactive shields use less power
        public override double GetPowerUse()
        {
            return (double)Math.Min(0.0, (Active ? NominalPowerUseFactor : NominalPowerUseFactor * 0.25f) * ActualPowerLevel * (1 - Damage));
        }
    }
}
