using Core.Types;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioServer.Scenarios.Controller
{
    public class CargoHauler : IEntityContorller
    {
        public List<Entity> Destinations = new List<Entity>();

		protected Random RNG = new Random();

		public enum RepeatTypes
		{
			None,
			Loop,
			Reverse,
		}
        public RepeatTypes Repeat = RepeatTypes.None;

		public double DestinationJitter = 0;

        public double MoveAcceleration = 0;
        public double MoveMaxSpeed = 0;

        public double MaxTurnSpeed = 0;

        public double DestinationDelay = 0;
        public double DestinationArivalRadius = 0;

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

            public int Destination = -1;
            public double TimeWaited = 0;

			public bool Forward = true;

			public Location DestinationOffset = Location.Zero;

			public bool Inited = false;
        }

        public int InfoKey { get; protected set; }

        void IEntityContorller.AddEntity(Entity ent)
        {
            InfoKey = ent.SetParam("CargoHauler.Data", new CargoHaulerDestinationData());
        }

		Location GetDestinationOffset(int destIndex)
		{
			if(DestinationJitter <= 0)
				return Location.Zero;

			return new Location((RNG.NextDouble() * DestinationJitter * 2) - DestinationJitter, (RNG.NextDouble() * DestinationJitter * 2) - DestinationJitter, 0);
		}

        void IEntityContorller.UpdateEntity(Entity ent)
        {
            if (Destinations.Count == 0)
                return;

            CargoHaulerDestinationData info = ent.GetParam(InfoKey) as CargoHaulerDestinationData;
            if (info == null)
                return;

            if (info.State == CargoHaulerDestinationData.States.TravelingTo)
            {
                if (info.Destination < 0)
					info.Destination = 0;

				if (info.Destination > Destinations.Count)
                {
					if(Repeat == RepeatTypes.Loop && Destinations.Count > 1)
						info.Destination = 0;
					else if (Repeat == RepeatTypes.Reverse && Destinations.Count > 1)
					{
						info.Forward = false;
						info.Destination = Destinations.Count - 2;
					}
                    else
                        return;
                }
				if(!info.Inited)
					info.DestinationOffset = GetDestinationOffset(info.Destination);

				Location destPos = Destinations[info.Destination].Position + info.DestinationOffset;

				double dist = Location.Distance(ent.Position, destPos);

                if (dist > DestinationArivalRadius)
                {
                    Vector3D targetVector =Location.VectorTo(ent.Position, destPos);
                    targetVector.Normailize();

					if (!info.Inited)
						ent.Orientation = Rotation.FromVector3D(targetVector);

					info.Inited = true;

					double speed = ent.Velocity.Length();

                    if (speed < MoveMaxSpeed)
                    {
                        speed += (MoveAcceleration * Timer.Delta);
                        if (speed > MoveMaxSpeed)
                            speed = MoveMaxSpeed;

                        ent.Velocity = targetVector * speed;
                    }

                    //    ent.Orientation = QuaternionD.LookAt(targetVector, Vector3D.UnitZ);
                }
                else
                {

                    double speed = ent.Velocity.Length();

                    if (speed > 0)
                    {
                        speed -= (MoveAcceleration * Timer.Delta);
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
                if (info.TimeWaited >= DestinationDelay)
				{
					info.State = CargoHaulerDestinationData.States.Orienting;

					info.Destination += info.Forward ? 1 : -1;
					if(info.Destination >= Destinations.Count || info.Destination < 0)
					{
						if(Repeat == RepeatTypes.Loop && Destinations.Count > 1)
							info.Destination = 0;
						else if(Repeat == RepeatTypes.Reverse && Destinations.Count > 1)
						{
							info.Forward = !info.Forward;
							if(!info.Forward)
								info.Destination = Destinations.Count - 2;
							else
								info.Destination = 1;
						}
						else
						{
							info.State = CargoHaulerDestinationData.States.Idle;
							return;
						}
					}

					info.DestinationOffset = GetDestinationOffset(info.Destination);
				}
                else
                    info.TimeWaited += Timer.Delta;
            }

            if (info.State == CargoHaulerDestinationData.States.Orienting)
            {
				Location destPos = Destinations[info.Destination].Position + info.DestinationOffset;

				Vector3D vecToTarget = Location.VectorTo(ent.Position, destPos);
                Rotation targetRot = Rotation.FromVector3D(vecToTarget);

				ent.Orientation.Normailzie();

				double delta = targetRot.Angle - ent.Orientation.Angle;
				if(delta > 180)
					delta = 360 - delta;
				if(delta < -180)
					delta = 360 + delta;

				ent.AngularVelocity.Angle = delta;
				ent.AngularVelocity.Clamp(MaxTurnSpeed);

				if (Math.Abs(delta) <= (MaxTurnSpeed * Timer.Delta * 2))
                {
                    ent.Orientation = targetRot;
                    ent.AngularVelocity = Rotation.Zero;
                    info.State = CargoHaulerDestinationData.States.TravelingTo;
                }
            }
        }
    }
}
