using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core;
using Agrotera.Core.Entities.Controllers;

namespace Agrotera.DefaultControllers
{
    public class Default : IEntityController
    {
        public virtual int Version { get { return 1; } }

        protected Dictionary<string, string> Arguments = new Dictionary<string, string>();

        public virtual void AddArgument(string arg, string value)
        {
            if (Arguments.ContainsKey(arg))
                Arguments[arg] = value;
            else
                Arguments.Add(arg, value);
        }

        public virtual void Init(Entity entity)
        {
        }

        public virtual void Update(Tick tick, Entity entity)
        {
        }
    }
}
