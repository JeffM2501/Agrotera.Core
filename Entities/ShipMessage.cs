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


        public static readonly int NewShipCode = "NewShip".GetHashCode();
        public static readonly int ExistingShipCode = "OldShip".GetHashCode();
    }
}
