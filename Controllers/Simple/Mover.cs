using System;
using System.Collections.Generic;

using Agrotera.Core;
using Agrotera.Core.Types;

using Agrotera.DefaultControllers.SubSystems;

namespace Agrotera.DefaultControllers.Simple
{
	public class Mover : SubSystemController
	{
		public override int Version { get { return 1; } }

		public static readonly string SpeedCommand = "Speed";
		public static readonly string VectorCommand = "Vector";

		double Speed = 0;
		Vector3F Direction = Vector3F.Zero;

		public override void AddArgument(string arg, string value)
		{
			if(arg == SpeedCommand)
				double.TryParse(value, out Speed);
			else if(arg == VectorCommand)
			{
				if(Vector3F.TryParse(value, out Direction))
					Direction.Normailize();
			}
		}

		public override void UpdateEntity(Tick tick, Entity entity)
		{
			double thisDelta =  Speed * tick.Delta;
			if (thisDelta != 0)
				entity.Position += Direction * thisDelta;

			// TODO, some kind of collision detection

			entity.Update(tick);
		}
	}
}
