using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Entities;
using Core.Types;

namespace Entities.Classes
{
    public class Ship : Entity
    {
        public string ClassName = string.Empty;

        public class KnownEntity : EventArgs
        {
            public Entity BaseEntity = null;
            public Vector3F LastPosition = Vector3F.Zero;
            public Vector3F LastVelocity = Vector3F.Zero;

            public double LastTimestamp = 0;
            public double LastTrasmitUpdate = 0;

            public KnownEntity(Entity ent)
            {
                BaseEntity = ent;
            }

            public bool Refresh(double timeStamp)
            {
                if (Vector3F.DistanceSquared(LastPosition, BaseEntity.Position) > 0.01 || Vector3F.DistanceSquared(LastVelocity, BaseEntity.Velocity) > 0.01)
                {
                    LastPosition = new Vector3F(BaseEntity.Position);
                    LastVelocity = new Vector3F(BaseEntity.Velocity);
                    LastTimestamp = timeStamp;
                    return true;
                }
                return false;
            }
        }

        public event EventHandler<KnownEntity> SensorEntityAppeared = null;
        public event EventHandler<KnownEntity> SensorEntityUpdated = null;
        public event EventHandler<KnownEntity> SensorEntityRemoved = null;

        public Dictionary<int, KnownEntity> KnownEntities = new Dictionary<int, KnownEntity>();

        public void UpdateEntity(Entity ent)
        {
            if (!KnownEntities.ContainsKey(ent.ID))
            {
                ent.Deleted += Entity_Deleted;
                var ke = new KnownEntity(ent);

                KnownEntities.Add(ent.ID, new KnownEntity(ent));
                ke.Refresh(Timer.Now);

                if (SensorEntityAppeared != null)
                    SensorEntityAppeared.Invoke(this, ke);
            }
            else
            {
                if (KnownEntities[ent.ID].Refresh(Timer.Now) && SensorEntityUpdated != null)
                    SensorEntityUpdated.Invoke(this, KnownEntities[ent.ID]);
            }
        }

        private void Entity_Deleted(object sender, EventArgs e)
        {
            Entity ent = sender as Entity;

            if (ent == null || !KnownEntities.ContainsKey(ent.ID))
                return;

            var ke = KnownEntities[ent.ID];
            KnownEntities.Remove(ent.ID);

            if (SensorEntityRemoved != null)
                SensorEntityRemoved.Invoke(this, ke);
        }

        public virtual double SensorRadius()
        {
            return 100;
        }
    }
}
