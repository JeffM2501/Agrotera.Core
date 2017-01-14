using Core.Types;
using Entities;
using Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioServer.Scenarios.Controller
{
    public class CargoHauler : IEntityContorller
    {
        public class DestinationInfo
        {
            public int Index = -1;
            public Entity TargetEnt = null;
            public double Delay = 0;
            public double ArivalRadius = 0;
            public double Jitter = 0;

            public Vector3D Offset = Vector3D.Zero;
        }
        public List<DestinationInfo> Destinations = new List<DestinationInfo>();

		protected Random RNG = new Random();

		public enum RepeatTypes
		{
			None,
			Loop,
			Reverse,
		}
        public RepeatTypes Repeat = RepeatTypes.None;

        public bool RadndomInitalDestination = false;


        public class CargoHaulerDestinationData
        {
            public enum States
            {
                TravelingTo,
                Offloading,
                Orienting,
                Idle,
            }

            public States State = States.TravelingTo;

            public DestinationInfo Destination = null;
            public double TimeWaited = 0;

			public bool Forward = true;

			public Location DestinationTarget = Location.Zero;

            public double TargetAngle = 0;
        }

        public int InfoKey { get; protected set; }

        void IEntityContorller.AddEntity(Entity ent)
        {
            InfoKey = ent.SetParam("CargoHauler.Data", new CargoHaulerDestinationData());

            if (RadndomInitalDestination)
                SetEntityDestination(ent, RNG.Next(Destinations.Count));
            else
                SetEntityDestination(ent, 0);
        }

        public int AddDesitnation(Entity ent)
        {
            return AddDesitnation(ent, 0, 0, 0, Vector3D.Zero);
        }

        public int AddDesitnation(Entity ent, double delay, double radius, double jitter)
        {
            return AddDesitnation(ent, delay, radius, jitter, Vector3D.Zero);
        }

        public int AddDesitnation(Entity ent, double delay, double radius, double jitter, Vector3D offset)
        {
            if (ent == null)
                return -1;

            DestinationInfo info = new DestinationInfo();
            info.Index = Destinations.Count;
            info.TargetEnt = ent;
            info.Delay = delay;
            info.ArivalRadius = radius;
            info.Jitter = jitter;
            Destinations.Add(info);
            return info.Index;
        }

        public void SetEntityDestination(Entity ent , int index)
        {
            CargoHaulerDestinationData info = ent.GetParam(InfoKey) as CargoHaulerDestinationData;
            if (info == null)
                return;

            info.Destination = Destinations[index];
            info.DestinationTarget = GetDestinationTarget(info.Destination);
        }

		Location GetDestinationTarget(DestinationInfo dest)
		{
			if(dest.Jitter <= 0)
				return dest.TargetEnt.Position + dest.Offset;

			return dest.TargetEnt.Position + dest.Offset +  new Location((RNG.NextDouble() * dest.Jitter * 2) - dest.Jitter, (RNG.NextDouble() * dest.Jitter * 2) - dest.Jitter, 0);
		}

        void IEntityContorller.UpdateEntity(Entity ent)
        {
            if (Destinations.Count == 0)
                return;

            Ship ship = ent as Ship;
            CargoHaulerDestinationData info = ent.GetParam(InfoKey) as CargoHaulerDestinationData;
            if (info == null || ship == null)
                return;

            bool needInit = info.Destination == null;
            if (info.State == CargoHaulerDestinationData.States.TravelingTo)
            {
                if (info.Destination == null)
                    SetEntityDestination(ent, 0);

				Location destPos = info.DestinationTarget;

				double dist = Location.Distance(ent.Position, destPos);

                if (dist > info.Destination.ArivalRadius)
                {
                    Vector3D targetVector =Location.VectorTo(ent.Position, destPos);
                    targetVector.Normailize();

					if (!needInit)
						ent.Orientation = Rotation.FromVector3D(targetVector);

					double speed = ent.Velocity.Length();

                    if (speed < ship.MoveMaxSpeed)
                    {
                        speed += (ship.MoveAcceleration * Timer.Delta);
                        if (speed > ship.MoveMaxSpeed)
                            speed = ship.MoveMaxSpeed;

                        ent.Velocity = targetVector * speed;
                    }
                }
                else
                {
                    double speed = ent.Velocity.Length();

                    if (speed > 0)
                    {
                        speed -= (ship.MoveAcceleration * Timer.Delta);
                        if (speed <= 0.01)
                        {
                            ent.Velocity = Vector3D.Zero;
                            speed = 0;
                        }
                          
                        else
                            ent.Velocity = Vector3D.Normalize(ent.Velocity) * speed;
                    }
                    
                    if (speed == 0)
                    {
                        info.State = CargoHaulerDestinationData.States.Offloading;
                        info.TimeWaited = 0;
                    } 
                } 
            }

            if (info.State == CargoHaulerDestinationData.States.Offloading)
            {
                if (info.TimeWaited >= info.Destination.Delay)
				{
					info.State = CargoHaulerDestinationData.States.Orienting;

                    int destIndex = info.Destination.Index;

                    destIndex += info.Forward ? 1 : -1;
					if(destIndex >= Destinations.Count || destIndex < 0)
					{
						if(Repeat == RepeatTypes.Loop && Destinations.Count > 1)
                            destIndex = 0;
						else if(Repeat == RepeatTypes.Reverse && Destinations.Count > 1)
						{
							info.Forward = !info.Forward;
							if(!info.Forward)
                                destIndex = Destinations.Count - 2;
							else
                                destIndex = 0;
						}
						else
						{
							info.State = CargoHaulerDestinationData.States.Idle;
							return;
                        }
                    }

                    SetEntityDestination(ent, destIndex);

                    Vector3D vecToTarget = Location.VectorTo(ent.Position, info.DestinationTarget);
                    Rotation targetRot = Rotation.FromVector3D(vecToTarget);
                    ent.AngularVelocity = new Rotation(ship.MaxTurnSpeed * Math.Sign(Rotation.ShortRotationTo(ent.Orientation, targetRot).Angle));

                    info.TargetAngle = targetRot.Angle;

                }
                else
                    info.TimeWaited += Timer.Delta;
            }

            if (info.State == CargoHaulerDestinationData.States.Orienting && info.Destination != null)
            {
                Rotation targetRot = new Rotation(info.TargetAngle);

				double delta = Rotation.ShortRotationTo(ent.Orientation,targetRot).Angle;

				if (Math.Abs(delta) <= (ship.MaxTurnSpeed * Timer.Delta * 2))
                {
                    ent.Orientation = targetRot;
                    ent.AngularVelocity = Rotation.Zero;
                    info.State = CargoHaulerDestinationData.States.TravelingTo;
                }
            }
        }
    }
}
