using System;

using Agrotera.DefaultControllers.SubSystems;

namespace Agrotera.DefaultControllers.Complex
{
	public class VesselController : Multiplexer
	{
		public VesselController() : base()
		{
			AddSubSystem(new PowerSystemController());
			AddSubSystem(new SensorSystemController());
		}
	}
}
