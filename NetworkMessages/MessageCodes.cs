using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkMessages
{
	public static class ProtocollDefinition
	{
		public static readonly string ProtoString = "Simple Ship Net.0.0.1";
	}

	public enum MessageCodes
	{
		Unknown = 0,

		// setup messages
		NewShip,
		ExistingShip,
		AssignShip,

		// ship control messages
		SetCourse,

		// ship Update messages
		SetSelfPosition,
		ShipNavigationStatus,

		// sensor update messages
		UpdateEntity,
        UpdateEnityDetails,
	}
}
