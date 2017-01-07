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
        public bool Loop = false;

        public double MoveAcceleration = 0;
        public double MoveMaxSpeed = 0;

        public double DestinationDelay = 0;
        public double DestinationArivalRadius = 0;

        public class CargoHaulerDestinationData
        {
            public int NextDestination = -1;
            public double TimeWaited = 0;
        }

        public int InfoKey { get; protected set; }

        void IEntityContorller.AddEntity(Entity ent)
        {
            InfoKey = ent.SetParam("CargoHauler.Data", new CargoHaulerDestinationData());
        }

        void IEntityContorller.UpdateEntity(Entity ent)
        {
            if (Destinations.Count == 0)
                return;

            CargoHaulerDestinationData info = ent.GetParam(InfoKey) as CargoHaulerDestinationData;
            if (info == null)
                return;

        //    ent.Rotation = QuaternionD.FromAxisAngle(Vector3D.UnitZ, 10);

            if (info.NextDestination < 0)
                info.NextDestination = 0;

            if (info.NextDestination > Destinations.Count)
            {
                if (Loop)
                    info.NextDestination = 0;
                else
                    return;
            }

            double dist = Vector3D.Distance(ent.Position, Destinations[info.NextDestination].Position);

            ent.SetParam("CargoHauler.DistanceToDestination", dist);
            if (dist > DestinationArivalRadius)
            {
                Vector3D targetVector = Destinations[info.NextDestination].Position - ent.Position;
                targetVector.Normailize();

                double speed = ent.Velocity.Length();

                if ( speed < MoveMaxSpeed)
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
                    if (speed <= 0)
                        ent.Velocity = Vector3D.Zero;
                    else
                        ent.Velocity = Vector3D.Normalize(ent.Velocity) * speed;
                }
                else
                {
                    if (info.TimeWaited < DestinationDelay)
                    {
                        info.TimeWaited += Timer.Delta;
                    }
                    else // set off for the next location
                    {
                        info.TimeWaited = 0;
                        info.NextDestination++;
                        if (info.NextDestination >= Destinations.Count && Loop)
                            info.NextDestination = 0;
                    }
                }
            }
        }
    }
}
