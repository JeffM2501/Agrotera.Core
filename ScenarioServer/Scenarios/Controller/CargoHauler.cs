using Core.Types;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int AtDestKey { get; protected set; }
        public int TimeWaitedKey { get; protected set; }

        void IEntityContorller.AddEntity(Entity ent)
        {
            AtDestKey = ent.SetParam("CargoHauler.DestinationIndex", -1);
            TimeWaitedKey = ent.SetParam("CargoHauler.TimeAtDestination", 0);
        }

        void IEntityContorller.UpdateEntity(Entity ent, double delta)
        {
            if (Destinations.Count == 0)
                return;

            int destIndex = (int)ent.GetParam(AtDestKey);
            if (destIndex < 0)
                destIndex = 0;

            if (destIndex > Destinations.Count)
            {
                if (Loop)
                    destIndex = 0;
                else
                    return;
            }

            ent.SetParam(AtDestKey, destIndex);

            double dist = Vector3F.Distance(ent.Position, Destinations[destIndex].Position);

            ent.SetParam("CargoHauler.DistanceToDestination", dist);
            if (dist > DestinationArivalRadius)
            {
                Vector3F targetVector = Destinations[destIndex].Position - ent.Position;
                targetVector.Normailize();

                double speed = ent.Velocity.Length();

                if ( speed < MoveMaxSpeed)
                {
                    speed += (MoveAcceleration * delta);
                    if (speed > MoveMaxSpeed)
                        speed = MoveMaxSpeed;

                    ent.Velocity = targetVector * speed;
                }
            }
            else
            {
                double speed = ent.Velocity.Length();

                if (speed > 0)
                {
                    speed -= (MoveAcceleration * delta);
                    if (speed <= 0)
                        ent.Velocity = Vector3F.Zero;
                    else
                        ent.Velocity = Vector3F.Normalize(ent.Velocity) * speed;
                }
                else
                {
                    double timeWaited = ent.GetParam(TimeWaitedKey);
                    if (timeWaited < DestinationDelay)
                    {
                        ent.SetParam(TimeWaitedKey, timeWaited + delta);
                    }
                    else // set off for the next location
                    {
                        ent.SetParam(TimeWaitedKey, 0);
                        destIndex++;
                        if (destIndex >= Destinations.Count && Loop)
                            destIndex = 0;

                        ent.SetParam(AtDestKey, destIndex); 
                    }
                }
            }
        }
    }
}
