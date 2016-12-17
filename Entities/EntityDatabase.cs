using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EntityDatabase
    {
        public Dictionary<int, Entity> Ents = new Dictionary<int, Entity>();

        protected int LastID = 1;

        public T New<T>() where T : Entity, new()
        {
            T i = new T();
            i.ID = LastID;
            LastID++;

            Ents.Add(i.ID, i);
            return i;
        }

        public Entity Add(Entity item)
        {
            if (Ents.ContainsKey(item.ID))
                Ents[item.ID] = item;
            else
                Ents.Add(item.ID, item);

            return item;
        }

        public void Remove(int id)
        {
            if (Ents.ContainsKey(id))
                Ents.Remove(id);
        }

        public void InterpMotion(double delta)
        {
			foreach(var entity in Ents.Values)
			{
				entity.Position += entity.Velocity * delta;
			}
        }

        public void ThinkEntityControllers(double delta)
        {
            foreach (var entity in Ents.Values)
            {
                if (entity.Controller != null)
                    entity.Controller.Update(entity, delta);
            }
        }
    }
}
