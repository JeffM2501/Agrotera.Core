
using System;
using System.Collections.Generic;

using Core.Types;
using Entities;
using Entities.Classes;
using Entities.Classes.Components;

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
                Traveling,
                Offloading,
                Idle,
            }

            public States State = States.Idle;

            public DestinationInfo Destination = null;
            public double TimeWaited = 0;

			public bool Forward = true;

			public Location DestinationTarget = Location.Zero;

            public double TargetAngle = 0;

            public int Runs = 0;
        }

        public int InfoKey { get; protected set; }

        void IEntityContorller.AddEntity(Entity ent)
        {
            Ship ship = ent as Ship;
            if (ship == null)
                return;

            SetupShip(ship);
        }

        protected void SetupShip(Ship ship)
        {
            InfoKey = ship.SetParam("CargoHauler.Data", new CargoHaulerDestinationData());

            ship.NaviComp.ArrivedAtWaypoint += NaviComp_ArrivedAtWaypoint;
            ship.NaviComp.CourseComplete += NaviComp_CourseComplete;
        }

        public void RedoCourse(Ship ship)
        {
            if (ship == null)
                return;

            CargoHaulerDestinationData info = ship.GetParam(InfoKey) as CargoHaulerDestinationData;
            if (info == null)
                SetupShip(ship);
            else
                info.State = CargoHaulerDestinationData.States.Idle;
        }

        private void NaviComp_CourseComplete(object sender, NavigationComputer.CourseWaypoint e)
        {
            Ship ship = sender as Ship;
            CargoHaulerDestinationData info = ship.GetParam(InfoKey) as CargoHaulerDestinationData;
            if (info == null || ship == null)
                return;

            info.Runs++;
            info.State = CargoHaulerDestinationData.States.Offloading;
			info.Destination = e.Tag as DestinationInfo;
            info.TimeWaited = 0;
            ship.NaviComp.AllStop();
        }

        private void NaviComp_ArrivedAtWaypoint(object sender, NavigationComputer.CourseWaypoint e)
        {
            Ship ship = sender as Ship;
            CargoHaulerDestinationData info = ship.GetParam(InfoKey) as CargoHaulerDestinationData;
            if (info == null || ship == null)
                return;

            info.State = CargoHaulerDestinationData.States.Offloading;
			info.Destination = e.Tag as DestinationInfo;
            info.TimeWaited = 0;
            ship.NaviComp.AllStop();
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

        public static bool UseCourses = true;

        void IEntityContorller.UpdateEntity(Entity ent)
        {
            if (!UseCourses)
            {
                UpdateEntityManual(ent);
                return;
            }

            Ship ship = ent as Ship;
            CargoHaulerDestinationData info = ent.GetParam(InfoKey) as CargoHaulerDestinationData;
            if (info == null || ship == null)
                return;

            if (info.State == CargoHaulerDestinationData.States.Idle)
            {
                info.State = CargoHaulerDestinationData.States.Traveling;   // it's ok to leave dead ended ships at traveling, they will stay at all stop until set to idle

                if (Destinations.Count == 0)
                {
                    ship.NaviComp.AllStop();
                    return;
                }

                if (ship.NaviComp.Waypoints.Count == 0 || ship.NaviComp.Mode != NavigationComputer.NavigationModes.Course)
                {
                    if (Repeat == RepeatTypes.None && info.Runs > 0)
                    {
                        ship.NaviComp.AllStop();
                        return;
                    }
                }

                // we need a course

                if (info.Runs > 0 && Repeat == RepeatTypes.Reverse)
                    info.Forward = !info.Forward;

                List<NavigationComputer.CourseWaypoint> waypoints = new List<NavigationComputer.CourseWaypoint>();

                int start = 0;
                if (info.Runs == 0 && RadndomInitalDestination)
                    start = RNG.Next(Destinations.Count);

                for (int i = start; i < Destinations.Count; i++)
                {
                    var dest = Destinations[i];
                    NavigationComputer.CourseWaypoint wp = new NavigationComputer.CourseWaypoint();
                    wp.AcceptableDistance = dest.ArivalRadius;
                    wp.TargetPosition = GetDestinationTarget(dest);
                    wp.Tag = dest;
                    waypoints.Add(wp);
                }

                if (!info.Forward)
                    waypoints.Reverse();

                ship.NaviComp.PlotCourse(waypoints, ship.MoveMaxSpeed, true);
				info.Destination = ship.NaviComp.Waypoints[0].Tag as DestinationInfo;
            }

            if (info.State == CargoHaulerDestinationData.States.Offloading)
            {
				if(ship.CurrentSpeed() > 0.01)
					return;

                info.TimeWaited += Timer.Delta;

                DestinationInfo dest = null;
               
                if (ship.NaviComp.Waypoints.Count > 0)
                    dest = ship.NaviComp.Waypoints[0].Tag as DestinationInfo;

                if (dest == null)
                    dest = info.Forward ? Destinations[Destinations.Count - 1] : Destinations[0];


                if (info.TimeWaited > info.Destination.Delay)
                {
                    if (ship.NaviComp.Waypoints.Count == 0)
                        info.State = CargoHaulerDestinationData.States.Idle;
                    else
                    {
                        info.State = CargoHaulerDestinationData.States.Traveling;
                        ship.NaviComp.ResumeCoursePlot();
						info.Destination = ship.NaviComp.Waypoints[0].Tag as DestinationInfo;
					}
                }
            }
        }

        protected void UpdateEntityManual(Entity ent)
        {
            if (Destinations.Count == 0)
                return;

            Ship ship = ent as Ship;
            CargoHaulerDestinationData info = ent.GetParam(InfoKey) as CargoHaulerDestinationData;
            if (info == null || ship == null)
                return;

            bool needInit = info.Destination == null;
            if (info.State == CargoHaulerDestinationData.States.Traveling)
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
					info.State = CargoHaulerDestinationData.States.Idle;

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

            if (info.State == CargoHaulerDestinationData.States.Idle && info.Destination != null)
            {
                Rotation targetRot = new Rotation(info.TargetAngle);

				double delta = Rotation.ShortRotationTo(ent.Orientation,targetRot).Angle;

				if (Math.Abs(delta) <= (ship.MaxTurnSpeed * Timer.Delta * 2))
                {
                    ent.Orientation = targetRot;
                    ent.AngularVelocity = Rotation.Zero;
                    info.State = CargoHaulerDestinationData.States.Traveling;
                }
            }
        }
    }
}
