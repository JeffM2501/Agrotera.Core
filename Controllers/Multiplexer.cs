using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core;
using Agrotera.Core.Entities.Controllers;

namespace Agrotera.DefaultControllers
{
    public class Multiplexer : Default
    {
        protected List<IEntityController> SubControllers = new List<IEntityController>();

        public override int Version { get { return 1; } }

        public override void AddArgument(string arg, string value)
        {
            base.AddArgument(arg, value);

            foreach (var s in SubControllers)
                s.AddArgument(arg, value);
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);

            foreach (var s in SubControllers)
                s.Init(entity);
        }

        public override void Update(Tick tick, Entity entity)
        {
            base.Init(entity);

            foreach (var s in SubControllers)
                s.Init(entity);
        }
    }
}
