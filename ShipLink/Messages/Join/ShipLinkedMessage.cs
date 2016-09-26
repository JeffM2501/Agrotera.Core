using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.ShipLink.Messages.Join
{
	public class ShipLinkedMessage : SimpleReflectionPackedNetworkMessage
	{
		public string ShipUID = string.Empty;
		public string ShipName = string.Empty;

		public string ShipRejoinToken = string.Empty;
	}
}
