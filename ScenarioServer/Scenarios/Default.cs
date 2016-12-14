
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScenarioServer.Classes;
using ScenarioServer.Interfaces;

namespace ScenarioServer.Scenarios
{
    public class Default : IScenarioController
    {
        public ScenarioState State = null;

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
        }

        public void Shutdown()
        {
            
        }

        public void Update(double delta)
        {
           
        }

        public UserShip AddPlayerShip(int playerID, List<string> requestParams)
        {
            UserShip s = State.MapItems.New<UserShip>();
            s.ControllerConnection = playerID;
            return s;
        }
    }
}
