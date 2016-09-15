using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Core.Entities
{
    public class Planet : Entity
    {
        public class PlanetTemplate : Entity.EntityTemplate
        {
            public PlanetTemplate()
                : base()
            {
                EntityClass = EntityClasses.Planet;
            }

            public override Entity Create(Areas.Zone map)
            {
                Planet b = new Planet(map);
                b.Template = this;
                b.TemplatePlanet = this;
                return b;
            }

            public string SurfaceImage = string.Empty;
            public string AtmosphereShader = string.Empty;

            public override void SetScienceItemFields(ScienceDatabaseItem item)
            {
                base.SetScienceItemFields(item);
                item.AddValue("Radius", Radius.ToString());
            }
        }

        public PlanetTemplate TemplatePlanet = null;

        public Planet(Areas.Zone map)
            : base(map)
        {
        }
    }
}
