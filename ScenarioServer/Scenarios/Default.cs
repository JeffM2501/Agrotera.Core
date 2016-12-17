
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities;
using Entities.Classes;
using ScenarioServer.Interfaces;

using Core.Types;


namespace ScenarioServer.Scenarios
{
    public class Default : IScenarioController
    {
        public ScenarioState State = null;

        protected Vector3F ZoneSize = new Vector3F(1000, 1000, 1000);

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

        protected Entity AddRandomeEntity<T>() where T: Entity, new()
        {
            T e = State.MapItems.New<T>();

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
