using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core.Entities;

using Lidgren.Network;

namespace Agrotera.ShipLink.Messages.ShipInfo
{
    public class UpdateShipTemplateMessage : BinarySerializedNetworkMessage
    {
        public Ship.ShipTemplate Template = null;
    }
}
