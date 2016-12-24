using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entities;

namespace ScenarioServer.Scenarios.Controller
{
    public class Fixed : IEntityContorller
    {
        void IEntityContorller.AddEntity(Entity ent)
        {
            
        }

        void IEntityContorller.UpdateEntity(Entity ent)
        {
   
        }

        public static readonly Fixed Default = new Fixed();
    }
}
