using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities;

namespace ScenarioServer.Scenarios.Controller
{
    public class Fixed : IEntityContorller
    {
        void IEntityContorller.AddEntity(Entity ent)
        {
            
        }

        void IEntityContorller.UpdateEntity(Entity ent, double delta)
        {
   
        }

        public static readonly Fixed Default = new Fixed();
    }
}
