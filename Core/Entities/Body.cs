using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Core.Entities
{
    public class Body : Entity
    {
        public class BodyTemplate : Entity.EntityTemplate
        {
            public BodyTemplate()
                : base()
            {
                EntityClass = EntityClasses.Body;
            }


            public override Entity Create(Areas.Zone map)
            {
                Body b = new Body(map);
                b.Template = this;
                b.TemplateBody = this;
                return b;
            }
            public string Model = string.Empty;
            public string Composition = string.Empty;
        }

        public BodyTemplate TemplateBody = null;

        public Body(Areas.Zone map) : base(map)
        {
        }
    }
}
