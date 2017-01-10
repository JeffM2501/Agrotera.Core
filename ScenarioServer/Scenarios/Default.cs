
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entities;
using Entities.Classes;
using ScenarioServer.Interfaces;

using Core.Types;
using ScenarioServer.Scenarios.Controller;
using ScenarioServer.Classes;

namespace ScenarioServer.Scenarios
{
    public class Default : IScenarioController
    {
        public ScenarioState State = null;

        protected Vector3D ZoneSize = new Vector3D(10000, 10000, 10000);
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

        protected Entity DefaultStation = null;

        public void Init(ScenarioState state)
        {
            State = state;

            DefaultStation = State.MapItems.New<Entity>();
			DefaultStation.Position = new Location(0, 0, 0);

			DefaultStation.Name = "Default Station";
            DefaultStation.VisualGraphics = "Station";
            DefaultStation.AngularVelocity = new Rotation(90);
            DefaultStation.SetController(Fixed.Default);

            var cargo = State.MapItems.New<Entity>();
            cargo.Name = "Default Cargo Stack";
            cargo.VisualGraphics = "CargoStack";
			cargo.Position = new Location(800, 0, 0);
            cargo.SetController(Fixed.Default);

            CargoHauler haulerRoute = new CargoHauler();
            haulerRoute.Destinations.Add(cargo);
            haulerRoute.Destinations.Add(DefaultStation);
            haulerRoute.Repeat = CargoHauler.RepeatTypes.Loop;
			haulerRoute.DestinationJitter = 15;
            haulerRoute.MoveMaxSpeed = 50;
            haulerRoute.MoveAcceleration = 50;
			haulerRoute.MaxTurnSpeed = 180;

			haulerRoute.DestinationArivalRadius = 50;
            haulerRoute.DestinationDelay = 5;

            var cargoOne = AddRandomEntity<Ship>();
            cargoOne.ClassName = "Simple Cargo Hauler";
            cargoOne.Name = "Cargo Transport 1";
            cargoOne.VisualGraphics = "Shuttle";
            cargoOne.Position = RandomPostionRelativeTo(cargo.Position, 50, 1000);
            cargoOne.SetController(haulerRoute);

            var cargoTwo = AddRandomEntity<Ship>();
            cargoTwo.ClassName = cargoOne.ClassName;
            cargoTwo.Name = "Cargo Transport 2";
            cargoTwo.VisualGraphics = "Shuttle";
            cargoTwo.Position = RandomPostionRelativeTo(cargo.Position, 50, 1000);
            cargoTwo.SetController(haulerRoute);

            var cargoThree = AddRandomEntity<Ship>();
            cargoThree.ClassName = cargoOne.ClassName;
            cargoThree.Name = "Cargo Transport 2";
            cargoThree.VisualGraphics = "Shuttle";
            cargoThree.Position = RandomPostionRelativeTo(DefaultStation.Position, 50, 1000);

            CargoHauler.CargoHaulerDestinationData data = new CargoHauler.CargoHaulerDestinationData();
            data.Destination = 1;

            cargoThree.SetParam(haulerRoute.InfoKey, data); // you go to the station first
            cargoThree.SetController(haulerRoute);

        }

        protected Location RandomPostion()
        {
            return new Location(RandomVectorParam() * ZoneSize.X,
                                RandomVectorParam() * ZoneSize.Y, 0);
        }

        protected Vector3D RandomVector(double magitude)
        {
            return new Vector3D(RandomVectorParam() * magitude,
                                RandomVectorParam() * magitude,
                                0);
        }

        protected Location RandomPostionRelativeTo(Location position, double minDistance, double maxDistance)
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

        protected T AddRandomEntity<T>() where T: Entity, new()
        {
            T e = State.NewEntity<T>();

            e.Position = RandomPostion();

            return e;
        }

        protected T AddRandomEntity<T>( double speed ) where T : Entity, new()
        {
            T e = AddRandomEntity<T>();

            e.Velocity = RandomVector(speed);

            return e;
        }

        public void Shutdown()
        {
            
        }

        public void Update()
        {
        }

        public Ship AddPlayerShip(long playerID, List<string> requestParams)
        {
            UserShip ship = State.NewUserShip(playerID);

            ship.Position = RandomPostionRelativeTo(DefaultStation.Position, 500, 800);
            ship.VisualGraphics = "PlayerShip";

            ship.UpdateSensorEntity(DefaultStation); // add known locations

            return ship;
        }
    }
}
