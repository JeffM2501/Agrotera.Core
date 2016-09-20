using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core.Types;
using Agrotera.Core.Entities.Systems;

namespace Agrotera.Core.Entities
{
    public class Station : Vessel
    {
        public class StationTemplate : VesselTemplate
        {
            public StationTemplate()
                : base()
            {
                EntityClass = EntityClasses.Station;
            }

            public float Shields = 0;

            public override Entity Create(Areas.Zone map)
            {
                return Setup(new Station(map));
            }

            public override void SetScienceItemFields(ScienceDatabaseItem item)
            {
                base.SetScienceItemFields(item);
                item.AddValue("Max Shield Strength", Shields.ToString());
            }

            protected override Entity Setup(Entity ent)
            {
                Station station = base.Setup(ent) as Station;
                if (station == null)
                    return ent;

                station.Shields = new Entities.Systems.ShieldSystem("Shields", 10, Shields);
                station.AddSystem(station.Shields);
                station.Reactor.NominalPowerUseFactor = -50;

                station.AddSystem(new Entities.Systems.ShipRefuelingPort());

                return station;
            }
        }

        public Station(Areas.Zone map) : base(map)
        {

        }

        public ShieldSystem Shields = null;

        public override void Init()
        {
            base.Init();
        }

        public override List<NamedFloatValue> GetScienceScanValues(double scanProgress)
        {
            List<NamedFloatValue> values = base.GetScienceScanValues(scanProgress);
            
            if (scanProgress > 0.8 && Shields != null)
                values.Add(new NamedFloatValue("Shield Power", Shields.Status.Current));

            if (scanProgress > 0.9 && Shields != null)
                values.Add(new NamedFloatValue("Max Shield Power", Shields.Status.Max));
            
            return values;
        }
    }
}
