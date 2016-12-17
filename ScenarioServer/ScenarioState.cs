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
        public List<Ship> AIShips = new List<Ship>();

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

            LastTime = time;
        }


        public Ship NewUserShip(int playerID)
        {
            var s =  MapItems.New<UserShip>();
            s.ControllerConnection = playerID;
            PlayerShips.Add(s);
            return s;
        }
    }
}
