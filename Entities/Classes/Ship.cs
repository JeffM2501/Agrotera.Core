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

        public class KnownEntity
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

            public void Refresh(double timeStamp)
            {
                LastPosition = new Vector3F(BaseEntity.Position);
                LastVelocity = new Vector3F(BaseEntity.Velocity);

                LastTimestamp = timeStamp;
            }
        }

        public Dictionary<int, KnownEntity> KnownEntities = new Dictionary<int, KnownEntity>();

        public void UpdateEntity(Entity ent, double timeStamp)
        {
            if (!KnownEntities.ContainsKey(ent.ID))
            {
                ent.Deleted += Entity_Deleted;
                KnownEntities.Add(ent.ID, new KnownEntity(ent));
            }
            KnownEntities[ent.ID].Refresh(timeStamp);
        }

        private void Entity_Deleted(object sender, EventArgs e)
        {
            Entity ent = sender as Entity;

            if (ent == null || !KnownEntities.ContainsKey(ent.ID))
                return;

            KnownEntities.Remove(ent.ID);
        }
    }
}
