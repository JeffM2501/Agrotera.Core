using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Core.Entities.Controllers
{

    public interface IEntityController
    {
        void AddEntity(Entity entity);

		void RemoveEntity(Entity entity);

        bool Update(Tick tick);

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

		private static List<IEntityController> MasterControllerList = new List<IEntityController>();

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

			IEntityController c = Activator.CreateInstance(t) as IEntityController;

			lock(MasterControllerList)
				MasterControllerList.Add(c);

			// TODO add it to a thread pool

			return c;
        }

		public static void UpdateControllers(Tick tick)
		{
			IEntityController[] l = new IEntityController[0];
			lock(MasterControllerList)
				l = MasterControllerList.ToArray();

			// toDO, add thread pools to have multiple updates happen at the same time
			List<IEntityController> toKill = new List<IEntityController>();
			foreach( var c in l)
			{
				if(c.Update(tick))
					toKill.Add(c);
			}

			lock(MasterControllerList)
				MasterControllerList.RemoveAll(x => toKill.Contains(x));
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
