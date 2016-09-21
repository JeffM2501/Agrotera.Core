using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Core.Entities.Controllers
{

    public interface IEntityController
    {
        void Init(Entity entity);

        void Update(Tick tick, Entity entity);

        void AddArgument(string arg, string value);
        int Version { get; }
    }

    public static class ControllerCache
    {
        private class ControllerFactory
        {
            public string Name = string.Empty;
            public Type ControllerType = null;
            public int Version = -1;
        }

        private static Dictionary<string, ControllerFactory> Controllers = new Dictionary<string, ControllerFactory>();

        public static IEntityController CreateController(string name)
        {
            Type t = null;

            string n = name.ToUpperInvariant();
            if (Controllers.ContainsKey(n))
                t = Controllers[n].ControllerType;
            else if (Controllers.ContainsKey("DEFAULT"))
                t = Controllers["DEFAULT"].ControllerType;
            else
                return null;

            return Activator.CreateInstance(t) as IEntityController;
        }

        public static bool RegisterController(Type t)
        {
            if (t.GetInterface(typeof(IEntityController).Name) == null)
                return false;

            string n = t.Name.ToUpper();
            if (Controllers.ContainsKey(n))
            {
                var e = Controllers[n];

                ControllerFactory f = new ControllerFactory();
                f.Name = n;
                f.ControllerType = t;
                IEntityController c = Activator.CreateInstance(t) as IEntityController;
                if (c == null)
                    return false;

                if (c.Version > e.Version)
                {
                    e.Version = c.Version;
                    e.ControllerType = t;
                    return true;
                }
                return false;
            }
            else
            {
                ControllerFactory f = new ControllerFactory();
                f.Name = n;
                f.ControllerType = t;
                IEntityController c = Activator.CreateInstance(t) as IEntityController;
                if (c == null)
                    return false;

                f.Version = c.Version;
                Controllers.Add(n, f);
            }
          
            return true;
        }
    }
}
