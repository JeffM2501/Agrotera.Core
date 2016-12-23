using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Types;

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

        protected ShipListener Listener = null;

        public ScenarioState(IScenarioController c)
        {
            Controller = c;
            Controller.Init(this);

            Listener = new ShipListener(2501);
            Listener.PeerDisconnected += Listener_PeerDisconnected;
            Listener.PeerWantsNewShip += Listener_PeerWantsNewShip;
            Listener.PeerWantsOldShip += Listener_PeerWantsOldShip;
        }

        private void Listener_PeerDisconnected(object sender, ShipListener.Peer e)
        {
            if (e.Ship != null)
            {
                e.Ship.RemoteDisconnect();
                e.ShipID = int.MinValue;
                e.Ship = null;
            }
        }

        private void Listener_PeerWantsOldShip(object sender, ShipListener.Peer e)
        {
            foreach(var s in PlayerShips)
            {
                if (s.ID == e.ShipID && s.RemoteConnectionID == long.MinValue)
                {
                    s.RemoteReconnect(e.ID);
                    e.Ship = s;
                    return;
                }
            }
        }

        private void Listener_PeerWantsNewShip(object sender, ShipListener.Peer e)
        {
            e.Ship = Controller.AddPlayerShip(e.ID, new List<string>()) as UserShip;
            e.ShipID = e.Ship.ID;
        }

        public void Startup()
        {
        }

        public void Update()
        {
            Timer.Advance();

 
            Controller.Update();

            MapItems.ThinkEntityControllers();
            MapItems.InterpMotion();

            ProcessShipSensors();
        }

        protected void ProcessShipSensors()
        {
            foreach(var ship in Ships)
            {
                foreach (var e in MapItems.GetInSphere(ship.Position,ship.SensorRadius()))
                    ship.UpdateEntity(e);
            }
        }

        public UserShip NewUserShip(long playerID)
        {
            var s = NewEntity<UserShip>();
            s.RemoteConnectionID = playerID;
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
