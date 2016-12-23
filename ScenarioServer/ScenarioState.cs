using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities;
using Entities.Classes;

using ScenarioServer.Classes;
using ScenarioServer.Interfaces;

namespace ScenarioServer
{
    public class ScenarioState
    {
        public IScenarioController Controller = null;

        public EntityDatabase MapItems = new EntityDatabase();
        public List<UserShip> PlayerShips = new List<UserShip>();
        public List<Ship> Ships = new List<Ship>();

        double LastTime = double.MinValue;

        public ScenarioState(IScenarioController c)
        {
            Controller = c;

            Controller.Init(this);
        }

        public void Startup(double time)
        {
            LastTime = time;
        }

        public void Update(double time)
        {
            double delta = time - LastTime;
            Controller.Update(delta);

            MapItems.ThinkEntityControllers(delta);
            MapItems.InterpMotion(delta);

            ProcessShipSensors();

            LastTime = time;
        }

        protected void ProcessShipSensors()
        {
            foreach(var ship in Ships)
            {

            }
        }

        public Ship NewUserShip(int playerID)
        {
            var s = NewEntity<UserShip>();
            s.ControllerConnection = playerID;
            PlayerShips.Add(s);
            return s;
        }

        public T NewEntity<T>() where T : Entity, new()
        {
            T i = MapItems.New<T>();
            if (i as Ship != null)
                Ships.Add(i as Ship);
            return i;
        }
    }
}
