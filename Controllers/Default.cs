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

		protected List<Entity> ControlledEntities = new List<Entity>();

        public virtual void AddArgument(string arg, string value)
        {
            if (Arguments.ContainsKey(arg))
                Arguments[arg] = value;
            else
                Arguments.Add(arg, value);
        }

        public virtual void AddEntity(Entity entity)
        {
			lock(ControlledEntities)
				ControlledEntities.Add(entity);
		}

		public virtual void RemoveEntity(Entity entity)
		{
			lock(ControlledEntities)
				ControlledEntities.Remove(entity);
		}

		public virtual bool Update(Tick tick)
		{
			Entity[] entList = new Entity[0];
			lock(ControlledEntities)
				entList = ControlledEntities.ToArray();

			foreach(var e in entList)
				UpdateEntity(tick, e);

			return entList.Length == 0;
		}

		public virtual void UpdateEntity(Tick tick, Entity entity)
        {
        }
    }
}
