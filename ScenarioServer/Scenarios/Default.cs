
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

        protected double RandomVectorParam()
        {
            return (RNG.NextDouble() - 0.5) * 2.0;
        }

        protected double RandomInRange( float max)
        {
            return (RNG.NextDouble() * max);
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
