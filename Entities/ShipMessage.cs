using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ShipMessage
    {
        public double Timestamp = double.MinValue;
        public int Code = int.MinValue;
        public string Payload = string.Empty;

        // setup messages
        public static readonly int NewShipCode = "WantNewShip".GetHashCode();
        public static readonly int ExistingShipCode = "WantOldShip".GetHashCode();


        // ship control messages
        public static readonly int SetCourseCode = "SetCourse".GetHashCode();

        // ship Update messages
        public static readonly int SetSelfPositionCode = "SetSelfPosition".GetHashCode();

        // sensor update messages
        public static readonly int UpdateEntityCode = "UpdateEntity".GetHashCode();

    }
}
