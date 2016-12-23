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
        public int ControllerConnection = -1;
 
        public UserShip()
        {
            Controller = this;
        }

        void IEntityContorller.AddEntity(Entity ent)
        {
        }

        void IEntityContorller.UpdateEntity(Entity ent, double delta)
        {
            
        }
    }
}
