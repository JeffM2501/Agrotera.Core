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

        public double MoveAcceleration = 5;
        public double MoveMaxSpeed = 25;
        public double MaxTurnSpeed = 45;

        public class KnownEntity : EventArgs
        {
			public object Tag = null;

            public Entity BaseEntity = null;

            public Location LastPosition = Location.Zero;
            public Vector3D LastVelocity = Vector3D.Zero;
            public Rotation LastOrientation = Rotation.Zero;
            public Rotation LastAngularVelocity = Rotation.Zero;

            public double LastTimestamp = 0;
            public double LastTrasmitUpdate = double.MinValue;

            public KnownEntity(Entity ent)
            {
                BaseEntity = ent;
            }

            protected double SmallAngle = (Math.PI / 180.0) * 1;

            public bool Refresh(double timeStamp)
            {
                double delta = Rotation.AngleBetween(LastOrientation,BaseEntity.Orientation);

                if (Math.Acos(delta) > SmallAngle || Location.DistanceSquared(LastPosition, BaseEntity.Position) > 0.01 || Vector3D.DistanceSquared(LastVelocity, BaseEntity.Velocity) > 0.01)
                {
                    LastPosition = BaseEntity.Position.Clone();
                    LastVelocity = BaseEntity.Velocity.Clone();
                    LastOrientation = BaseEntity.Orientation.Clone();
                    LastAngularVelocity = BaseEntity.AngularVelocity.Clone();
                    LastTimestamp = timeStamp;
                    return true;
                }
                return false;
            }

			public bool Refresh(SensorEntityUpdate upd)
			{
				LastPosition = upd.Position.Clone();
				LastVelocity = upd.Velocity.Clone();
                LastOrientation = upd.Orientation.Clone();
                LastAngularVelocity = upd.Rotation.Clone();
                LastTimestamp = Timer.Now;
				return true;
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

			if (Location.Distance(ent.Position,Position) < SensorRadius())
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

				KnownEntities.Add(ent.ID, ke);
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

        public virtual void UpdateSensorEntity(SensorEntityDetails details)
        {
            if (!KnownEntities.ContainsKey(details.ID))
            {
                Entity ent = new Entity();
                ent.ID = details.ID;
                ent.Deleted += Entity_Deleted;
                var ke = NewSensorEnity(ent);

                KnownEntities.Add(ent.ID, ke);
                RefreshEntity(ke, details);

                ke.BaseEntity.Name = details.Name;
                ke.BaseEntity.VisualGraphics = details.VisualGraphics;
                ke.BaseEntity.Tag = null;

                if (SensorEntityAppeared != null)
                    SensorEntityAppeared.Invoke(this, ke);
            }
            else
            {
                var ke = KnownEntities[details.ID];

                RefreshEntity(ke, details);

                ke.BaseEntity.Name = details.Name;
                ke.BaseEntity.VisualGraphics = details.VisualGraphics;
                ke.BaseEntity.Tag = null;

                if (SensorEntityUpdated != null)
                    SensorEntityUpdated.Invoke(this, KnownEntities[details.ID]);
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
			return 5000;
		}
	}
}
