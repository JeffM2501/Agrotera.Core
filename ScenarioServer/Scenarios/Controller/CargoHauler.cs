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

            if (info.State == CargoHaulerDestinationData.States.TravelingTo)
            {
                if (info.Destination < 0)
                    info.Destination = 0;

                if (info.Destination > Destinations.Count)
                {
                    if (Loop)
                        info.Destination = 0;
                    else
                        return;
                }

                double dist = Location.Distance(ent.Position, Destinations[info.Destination].Position);

                if (dist > DestinationArivalRadius)
                {
                    Vector3D targetVector =Location.VectorTo(ent.Position, Destinations[info.Destination].Position);
                    targetVector.Normailize();

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
                    info.State = CargoHaulerDestinationData.States.Orienting;
                else
                    info.TimeWaited += Timer.Delta;
            }

            if (info.State == CargoHaulerDestinationData.States.Orienting)
            {
                info.Destination++;
                if (info.Destination >= Destinations.Count)
                {
                    if (Loop)
                        info.Destination = 0;
                    else
                    {
                        info.State = CargoHaulerDestinationData.States.Idle;
                        return;
                    }
                        
                }

                Vector3D vecToTarget = Location.VectorTo(ent.Position,Destinations[info.Destination].Position);
                Rotation targetRot = Rotation.FromVector3D(vecToTarget);
                double traverseAngle = Rotation.AngleBetween(targetRot, ent.Orientation);

                if (traverseAngle <= (MaxTurnSpeed * Timer.Delta * 2))
                {
                    ent.Orientation = targetRot;
                    ent.AngularVelocity = Rotation.Zero;
                    info.State = CargoHaulerDestinationData.States.TravelingTo;
                    return;
                }
                else
                {
                    ent.AngularVelocity = Rotation.ShortRotationTo(ent.Orientation, targetRot);
                }
            }
        }
    }
}
