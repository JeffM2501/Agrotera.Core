using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ScenarioServer.Interfaces
{
    public class ScenarioControllerLoader
    {
        public List<IScenarioController> Controllers = new List<IScenarioController>();

        public void Scan(Assembly module)
        {
            foreach(var t in module.GetTypes())
            {
                if (t.GetInterface(typeof(IScenarioController).Name) != null)
                {
                    Controllers.Add(Activator.CreateInstance(t) as IScenarioController);
                }
            }
        }

        public IScenarioController FindByName(string name)
        {
            string l = name.ToLowerInvariant();
            return Controllers.Find(x => x.Name.ToLowerInvariant() == l);
        }

        public IScenarioController GetDefaultScenario()
        {
            var i =  Controllers.Find(x => x.Defaultable);
            if (i == null)
                i = new Scenarios.Default();
            return i;
        }
    }
}
