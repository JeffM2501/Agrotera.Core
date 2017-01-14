using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioServer.Classes.Templates
{
    public class ShipTemplate
    {
        public string ClassName = string.Empty;
        public string DefaultGraphics = string.Empty;

        public double MaxAcceleration = 0;
        public double MaxSpeed = 0;
        public double MaxTurn = 0;

        public double PerformanceRand = 0;

        private static Random RNG = new Random();

        public Entities.Classes.Ship SetupShip(Entities.Classes.Ship ship, string name)
        {
            if (ship != null)
            {
                ship.ClassName = ClassName;
                ship.VisualGraphics = DefaultGraphics;
                ship.Name = name;
                ship.MaxTurnSpeed = MaxTurn;
                ship.MoveAcceleration = MaxAcceleration;
                ship.MoveMaxSpeed = MaxSpeed;

                if (PerformanceRand != 0)
                {
                    ship.MaxTurnSpeed *= (1.0 - (RNG.NextDouble() * PerformanceRand));
                    ship.MoveAcceleration *= (1.0 - (RNG.NextDouble() * PerformanceRand));
                    ship.MoveMaxSpeed *= (1.0 - (RNG.NextDouble() * PerformanceRand));
                }
            }
            return ship;
        }
    }
}
