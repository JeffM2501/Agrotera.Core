using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrotera.Core.Entities.Systems
{
    public class VesselSystem
    {
        public VesselSystem() { }
        public VesselSystem(string name, double powerUse)
        {
            Name = name;
            NominalPowerUseFactor = powerUse;
        }

		public int ID = -1;
		public string Name = string.Empty;
		public double NominalPowerUseFactor = 1;
		public double HeatFactor = 1;

		public double Damage = 0.0f;		//1.0-0.0, where 0.0 is fully broken.
		public double DesiredPowerLevel = 1.0f; //0.0-3.0, default 1.0
		public double ActualPowerLevel = 1.0f; //0.0-3.0, default 1.0
		public double HeatLevel  = 0.0f; //0.0-1.0, system will damage at 1.0
		public double CoolantLevel = 0;	//0.0-10.0

		protected bool UseDamage = false;

		public override string ToString()
		{
			return Name;
		}

		public virtual bool UsesDamage()
		{
			return UseDamage;
		}

		public virtual bool Generates()
		{
			return NominalPowerUseFactor < 0;
		}

        public virtual bool Destoryed()
        {
            return Damage >= 1;
        }

		public virtual void Update(Tick tick)
		{

		}

		public virtual double GetHeatingDelta()
		{
			return (double)Math.Pow(1.7, DesiredPowerLevel - 1.0f) - (1.01f + CoolantLevel * 0.1f);
		}

		public virtual double GetPowerUse()
		{
			if(UseDamage)
				return (double)Math.Min(0.0, NominalPowerUseFactor * ActualPowerLevel * (1 - Damage));

			return (double)Math.Min(0.0, NominalPowerUseFactor * ActualPowerLevel);
		}

		public virtual void AddHeat(double ammount)
		{
			HeatLevel = (double)Math.Min(1.0, HeatLevel + (ammount * HeatFactor));
		}

		public double GetPerformanceDegredation()
		{
            if (Destoryed())
                return 0;

			return (1.0f - HeatLevel) * (1.0f - Damage);
		}

		public double GetDesiredPowerUse()
		{
			return NominalPowerUseFactor * DesiredPowerLevel;
		}

		public double GetActualPowerUse()
		{
			return NominalPowerUseFactor * ActualPowerLevel;
		}

        public virtual double GetPerformanceFactor()
        {
            if (Destoryed())
                return 0;

            return (NominalPowerUseFactor / ActualPowerLevel) * GetPerformanceDegredation();
        }

		public virtual double GetGeneratedPower()
		{
			if(!Generates())
				return 0;

			return -1.0f * NominalPowerUseFactor * DesiredPowerLevel * GetPerformanceDegredation();
		}
    }
}
