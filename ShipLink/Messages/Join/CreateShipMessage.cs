using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.ShipLink.Messages.Join
{
	public class CreateShipMessage : SimpleReflectionPackedNetworkMessage
	{
		public string ShipClass = string.Empty;
		public string ShipName = string.Empty;
	}
}
