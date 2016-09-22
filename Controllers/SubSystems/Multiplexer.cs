using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core;
using Agrotera.Core.Entities.Controllers;

namespace Agrotera.DefaultControllers.SubSystems
{
	public class SubSystemController : Default
	{
		public static readonly int FirstPriority = 0;
		public static readonly int PowerSystemPriority = 10;
		public static readonly int NormalPriority = 20;
		public static readonly int LastPriority = 100;

		public int Priority = LastPriority;
	}

	public class Multiplexer : Default
	{
		public Dictionary<int,List<SubSystemController>> SubControllers = new Dictionary<int, List<SubSystemController>>();

		protected void AddSubSystem(SubSystemController subSystem)
		{
			int priority = subSystem.Priority;
			if(!SubControllers.ContainsKey(priority))
				SubControllers.Add(priority, new List<SubSystemController>());

			SubControllers[priority].Add(subSystem);
		}

		public override void AddArgument(string arg, string value)
		{
			base.AddArgument(arg, value);

			foreach(var c in SubControllers)
			{
				foreach(var s in c.Value)
					s.AddArgument(arg, value);
			}
				
		}

		public override void EntityAdded(Entity entity)
		{
			foreach(var c in SubControllers)
			{
				foreach(var s in c.Value)
					s.AddEntity(entity);
			}
		}

		public override void EntityRemoved(Entity entity)
		{
			foreach(var c in SubControllers)
			{
				foreach(var s in c.Value)
					s.RemoveEntity(entity);
			}
		}

		public override void UpdateEntity(Tick tick, Entity entity)
		{
			base.UpdateEntity(tick, entity);

			foreach(var c in SubControllers)
			{
				foreach(var s in c.Value)
					s.UpdateEntity(tick, entity);
			}
		}
	}
}
