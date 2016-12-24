using Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public Entity GetByName(string name)
        {
            foreach(var e in Ents.Values)
            {
                if (e.Name == name)
                    return e;
            }

            return null;
        }

        public List<Entity> GetInSphere(Vector3F center, double radius)
        {
            List<Entity> inRad = new List<Entity>();

            double rad2 = radius * radius;
            foreach(var e in Ents.Values)
            {
                if (Vector3F.DistanceSquared(center, e.Position) <= rad2)
                    inRad.Add(e);
            }

            return inRad;
        }

        public void Remove(int id)
        {
            if (Ents.ContainsKey(id))
            {
                var ent = Ents[id];
                ent.Delete();
                Ents.Remove(id);
            }
        }

        public void InterpMotion()
        {
            double delta = Timer.Delta;

            foreach (var entity in Ents.Values)
			{
				entity.Position += entity.Velocity * delta;
			}
        }

        public void ThinkEntityControllers()
        {
            foreach (var entity in Ents.Values)
                entity.UpdateController();
        }
    }
}
