using System;

using Agrotera.DefaultControllers.SubSystems;
using Agrotera.DefaultControllers.Simple;

namespace Agrotera.DefaultControllers.Complex
{
	public class StationController : VesselController
	{
		public StationController() : base()
		{
			AddSubSystem(new Spinner());
		}
	}
}
