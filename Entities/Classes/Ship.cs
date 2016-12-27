using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetworkMessages.ShipMessages;
using Entities;
using Core.Types;

namespace Entities.Classes
{
    public class Ship : Entity
    {
        public string ClassName = string.Empty;

        public class KnownEntity : EventArgs
        {
			public object Tag = null;

            public Entity BaseEntity = null;
            public Vector3D LastPosition = Vector3D.Zero;
            public Vector3D LastVelocity = Vector3D.Zero;

            public double LastTimestamp = 0;
            public double LastTrasmitUpdate = 0;

            public KnownEntity(Entity ent)
            {
                BaseEntity = ent;
            }

            public bool Refresh(double timeStamp)
            {
                if (Vector3D.DistanceSquared(LastPosition, BaseEntity.Position) > 0.01 || Vector3D.DistanceSquared(LastVelocity, BaseEntity.Velocity) > 0.01)
                {
                    LastPosition = new Vector3D(BaseEntity.Position);
                    LastVelocity = new Vector3D(BaseEntity.Velocity);
                    LastTimestamp = timeStamp;
                    return true;
                }
                return false;
            }

			public bool Refresh(SensorEntityUpdate upd)
			{
				if(Vector3D.DistanceSquared(LastPosition, upd.Position) > 0.01 || Vector3D.DistanceSquared(LastVelocity, upd.Velocity) > 0.01)
				{
					LastPosition = new Vector3D(upd.Position);
					LastVelocity = new Vector3D(upd.Velocity);
					LastTimestamp = upd.TimeStamp;
					return true;
				}
				return false;
			}
		}

        public event EventHandler<KnownEntity> SensorEntityAppeared = null;
        public event EventHandler<KnownEntity> SensorEntityUpdated = null;
        public event EventHandler<KnownEntity> SensorEntityRemoved = null;

        public Dictionary<int, KnownEntity> KnownEntities = new Dictionary<int, KnownEntity>();

        public void UpdateSensorEntity(Entity ent)
        {
            if (!KnownEntities.ContainsKey(ent.ID))
            {
                ent.Deleted += Entity_Deleted;
                var ke = NewSensorEnity(ent);

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

			if (Vector3D.Distance(ent.Position,Position) < SensorRadius())
			{
				// detailed sensor info
				
			}
        }

		public virtual void RefreshEntity(KnownEntity ke, SensorEntityUpdate update)
		{
			ke.Refresh(update);
		}

		public virtual void UpdateSensorEntity(SensorEntityUpdate update)
		{
			if(!KnownEntities.ContainsKey(update.ID))
			{
				Entity ent = new Entity();
				ent.ID = update.ID;
				ent.Deleted += Entity_Deleted;
				var ke = NewSensorEnity(ent);

				KnownEntities.Add(ent.ID, new KnownEntity(ent));
				RefreshEntity(ke, update);

				if(SensorEntityAppeared != null)
					SensorEntityAppeared.Invoke(this, ke);
			}
			else
			{
				RefreshEntity(KnownEntities[update.ID], update);

				if(SensorEntityUpdated != null)
					SensorEntityUpdated.Invoke(this, KnownEntities[update.ID]);
			}
		}

		protected virtual KnownEntity NewSensorEnity(Entity ent)
		{
			return new KnownEntity(ent);
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

		public virtual double VisualRadius()
		{
			return 1000;
		}
	}
}
