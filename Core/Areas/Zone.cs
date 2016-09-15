using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core.Types;

namespace Agrotera.Core.Areas
{
    public class Zone
    {
        public string ID = string.Empty;
        public string Name = string.Empty;

        public Vector3F Bounds = Vector3F.Zero;

        public double Gravity = 0;
        public double ZFloor = double.MinValue;

        public override string ToString()
        {
            return ID + "_" + Name;
        }

        public List<Entity> StaticEntities = new List<Entity>();
        public List<Entity> DynamicEntities = new List<Entity>();

        public event EventHandler<Entity.EntityEventArgs> EntityAdded = null;

        public delegate Zone ZoneCreateCallback();

        public static ZoneCreateCallback CreateZone = null;

        public static Zone NewZone()
        {
            if (CreateZone == null)
                return new Zone();

            return CreateZone();
        }

        public virtual void Update(Tick tick)
        {
            foreach (var e in StaticEntities)
                e.Update(tick);

            foreach (var e in DynamicEntities)
                e.Update(tick);
        }

        public virtual Entity AddStaticItem(Entity.EntityTemplate template)
        {
            if (template == null)
                return null;

            var e = InitEntity(template);
            StaticEntities.Add(e);
            return e;
        }

        public virtual T AddStaticItem<T>(Entity.EntityTemplate template) where T : Entity
        {
            if (template == null)
                return null;

            T e = InitEntity<T>(template);
            if (e != null)
                StaticEntities.Add(e);
            return e;
        }

        public virtual Entity InitEntity(Entity.EntityTemplate template)
        {
            Entity e = template.Create(this);
            e.Init();

            if (EntityAdded != null)
                EntityAdded(this, e.Args);
            return e;
        }

        public T InitEntity<T>(Entity.EntityTemplate template) where T : Entity
        {
            if (template == null)
                return null;

            T s = template.Create(this) as T;
            if (s != null)
            {
                s.Init();
                if (EntityAdded != null)
                    EntityAdded(this, s.Args);
            }

            return s;
        }

        public virtual List<Entity> FindEntitiesInRadiusOf(Entity center, double radius)
        {
            //TODO, use an octree or ask unity?
            double dSquared = radius * radius;

            List<Entity> eList = new List<Entity>();

            foreach (var e in StaticEntities)
            {
                if (e == center)
                    continue;

                if (Vector3F.DistanceSquared(center.Position, e.Position) <= dSquared)
                    eList.Add(e);
            }

            foreach (var e in DynamicEntities)
            {
                if (e == center)
                    continue;
                if (Vector3F.DistanceSquared(center.Position, e.Position) <= dSquared)
                    eList.Add(e);
            }

            return eList;
        }
    }
}
