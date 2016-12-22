
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities;
using Entities.Classes;
using ScenarioServer.Interfaces;

using Core.Types;
using ScenarioServer.Scenarios.Controller;


namespace ScenarioServer.Scenarios
{
    public class Default : IScenarioController
    {
        public ScenarioState State = null;

        protected Vector3F ZoneSize = new Vector3F(1000, 1000, 1000);
        protected Random RNG = new Random();

        public bool Defaultable
        {
            get
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                return "Default";
            }
        }

        public void Init(ScenarioState state)
        {
            State = state;

            var station = AddRandomEntity<Entity>();
            station.Name = "Default Station";
            station.SetController(Fixed.Default);

            var cargo = AddRandomEntity<Entity>();
            cargo.Name = "Default Cargo Stack";
            cargo.Position = RandomPostionRelativeTo(station.Position, 100, 1000);
            cargo.SetController(Fixed.Default);

            CargoHauler haulerRoute = new CargoHauler();
            haulerRoute.Destinations.Add(cargo);
            haulerRoute.Destinations.Add(station);
            haulerRoute.MoveMaxSpeed = 10;
            haulerRoute.MoveAcceleration = 5;
            haulerRoute.DestinationArivalRadius = 20;
            haulerRoute.DestinationDelay = 30;

            var cargoOne = AddRandomEntity<Entity>();
            cargoOne.Name = "Cargo Transport 1";
            cargoOne.Position = RandomPostionRelativeTo(cargo.Position, 50, 1000);
            cargoOne.SetController(haulerRoute);

            var cargoTwo = AddRandomEntity<Entity>();
            cargoTwo.Name = "Cargo Transport 2";
            cargoTwo.Position = RandomPostionRelativeTo(cargo.Position, 50, 1000);
            cargoTwo.SetController(haulerRoute);

            var cargoThree = AddRandomEntity<Entity>();
            cargoThree.Name = "Cargo Transport 2";
            cargoThree.Position = RandomPostionRelativeTo(station.Position, 50, 1000);
            cargoThree.SetParam(haulerRoute.AtDestKey, 1); // you go to the station first
            cargoThree.SetController(haulerRoute);

        }

        protected Vector3F RandomPostion()
        {
            return new Vector3F(RandomVectorParam() * ZoneSize.X,
                                RandomVectorParam() * ZoneSize.Y,
                                RandomVectorParam() * ZoneSize.Z);
        }

        protected Vector3F RandomVector(double magitude)
        {
            return new Vector3F(RandomVectorParam() * magitude,
                                RandomVectorParam() * magitude,
                                RandomVectorParam() * magitude);
        }

        protected Vector3F RandomPostionRelativeTo(Vector3F position, double minDistance, double maxDistance)
        {
            return  position +  RandomVector(RandomInRange(minDistance,maxDistance));
        }

        protected double RandomVectorParam()
        {
            return (RNG.NextDouble() - 0.5) * 2.0;
        }

        protected double RandomInRange( double max)
        {
            return (RNG.NextDouble() * max);
        }

        protected double RandomInRange(double min, double max)
        {
            return min + RandomInRange(max - min);
        }

        protected Entity AddRandomEntity<T>() where T: Entity, new()
        {
            T e = State.MapItems.New<T>();

            e.Position = RandomPostion();

            return e;
        }

        protected Entity AddRandomEntity<T>( double speed ) where T : Entity, new()
        {
            Entity e = AddRandomEntity<T>();

            e.Velocity = RandomVector(speed);

            return e;
        }

        public void Shutdown()
        {
            
        }

        public void Update(double delta)
        {
        }

        public Ship AddPlayerShip(int playerID, List<string> requestParams)
        {
            return State.NewUserShip(playerID);
        }
    }
}
