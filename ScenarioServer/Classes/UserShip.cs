using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Entities.Classes;

namespace ScenarioServer.Classes
{
    public class UserShip : Ship
    {
        public int ControllerConnection = -1;

        public Dictionary<int, double> LastEntityUpdates = new Dictionary<int, double>();
        public List<int> KnownEntities = new List<int>();
    }
}
