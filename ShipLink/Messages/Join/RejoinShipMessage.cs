using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.ShipLink.Messages.Join
{
	public class RejoinShipMessage : SimpleReflectionPackedNetworkMessage
	{
		public string ShipRejoinKey = string.Empty;
	}
}
