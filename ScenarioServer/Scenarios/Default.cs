
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
using ScenarioServer.Classes.Templates;

namespace ScenarioServer.Scenarios
{
    public class Default : IScenarioController
    {
        public ScenarioState State = null;

        protected Vector3D ZoneSize = new Vector3D(10000, 10000, 10000);

        protected ShipTemplate CargoShipTemplate = null;

        public Default()
        {
            CargoShipTemplate = new ShipTemplate();
            CargoShipTemplate.MaxAcceleration = 50;
            CargoShipTemplate.MaxSpeed = 100;
            CargoShipTemplate.MaxTurn = 45;
            CargoShipTemplate.PerformanceRand = 0.75;
            CargoShipTemplate.DefaultGraphics = "Shuttle";
            CargoShipTemplate.ClassName = "Simple Cargo Hauler"; 
        }

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
            ControllerTools.SetBounds(ZoneSize.X, ZoneSize.Y, ZoneSize.Z);

            DefaultStation = AddEntity<Entity>(new Location(0, 0, 0));
			DefaultStation.Name = "Default Station";
            DefaultStation.VisualGraphics = "Station";
            DefaultStation.AngularVelocity = new Rotation(90);
            DefaultStation.SetController(Fixed.Default);

            var cargo = AddEntity<Entity>(new Location(800, 0, 0));
            cargo.Name = "Default Cargo Stack";
            cargo.VisualGraphics = "CargoStack";
			cargo.SetController(Fixed.Default);

            CargoHauler haulerRoute = new CargoHauler();

            haulerRoute.AddDesitnation(DefaultStation, 2, 25, 25,new Vector3D(0,100,0));
           // haulerRoute.AddDesitnation(DefaultStation, 0, 25, 25, new Vector3D(400, 200, 0));
            haulerRoute.AddDesitnation(cargo,5,50,25);
         //   haulerRoute.AddDesitnation(DefaultStation, 0, 25, 25, new Vector3D(400, -200, 0));
            haulerRoute.AddDesitnation(DefaultStation,2,25,25, new Vector3D(0, -100, 0));

            haulerRoute.Repeat = CargoHauler.RepeatTypes.Reverse;
            haulerRoute.RadndomInitalDestination = true;

            CargoShipTemplate.SetupShip(AddEntity<Ship>(ControllerTools.RandomPostionRelativeTo(cargo.Position, 50, 500)), "Cargo Transport 1").SetController(haulerRoute);
            CargoShipTemplate.SetupShip(AddEntity<Ship>(ControllerTools.RandomPostionRelativeTo(cargo.Position, 100, 500)), "Cargo Transport 2").SetController(haulerRoute);
            CargoShipTemplate.SetupShip(AddEntity<Ship>(ControllerTools.RandomPostionRelativeTo(DefaultStation.Position, 300, 1000)), "Cargo Transport 3").SetController(haulerRoute);
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

            ship.Position = ControllerTools.RandomPostionRelativeTo(DefaultStation.Position, 500, 800);
            ship.VisualGraphics = "PlayerShip";

            ship.UpdateSensorEntity(DefaultStation); // add known locations

            return ship;
        }

        protected T AddRandomEntity<T>() where T : Entity, new()
        {
            return AddEntity<T>(ControllerTools.RandomPostion());
        }

        protected T AddEntity<T>(Location pos) where T : Entity, new()
        {
            T e = State.NewEntity<T>();

            e.Position = pos;

            return e;
        }

        protected T AddRandomEntity<T>(double speed) where T : Entity, new()
        {
            T e = AddRandomEntity<T>();

            e.Velocity = ControllerTools.RandomVector(speed);

            return e;
        }
    }
}
