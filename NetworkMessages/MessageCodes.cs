using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkMessages
{
	public static class ProtocollDefinition
	{
		public static readonly string ProtoString = "Simple Ship Net.0.0.1";

        public static readonly string ConsoleProtoString = "Simple Ship Console Net.0.0.1";
    }

	public enum ShipMessageCodes
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

    public enum ConsoleMessageCodes
    {
        Unknown = 0,

        // setup message
        ConnectToShip,
        SetupConsoles,
    }
}
