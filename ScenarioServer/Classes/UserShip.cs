using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Types;

using Entities.Classes;
using Entities;

namespace ScenarioServer.Classes
{
    public class UserShip : Ship, IEntityContorller
    {
        public long RemoteConnectionID = long.MinValue;

        public List<ShipMessage> InboundMessages = new List<ShipMessage>();
        public List<ShipMessage> OutboundMessages = new List<ShipMessage>();
 
        public UserShip()
        {
            Controller = this;
        }

        public virtual void RemoteDisconnect()
        {
            RemoteConnectionID = long.MinValue;
        }

        public virtual void RemoteReconnect(long id)
        {
            RemoteConnectionID = id;
        }

        void IEntityContorller.AddEntity(Entity ent)
        {
        }

        void IEntityContorller.UpdateEntity(Entity ent, double delta)
        {
            
        }
    }
}
