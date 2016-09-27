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
		public string Name = string.Empty;

		public class SystemDescriptor
		{
			public int ID = int.MinValue;
			public string Name = string.Empty;

			public double NominalPowerUseFactor = 1;
			public double HeatFactor = 1;
		}
		public List<SystemDescriptor> Systems = new List<SystemDescriptor>();

		public void SetFromShip ( Ship ship)
		{
			Template = ship.TemplateShip;
			Name = ship.Name;

			foreach (var s in ship.Systems.Values)
			{
				SystemDescriptor desc = new SystemDescriptor();
				desc.ID = s.ID;
				desc.Name = s.Name;
				desc.NominalPowerUseFactor = s.NominalPowerUseFactor;
				desc.HeatFactor = s.HeatFactor;

				Systems.Add(desc);
			}
		}
    }
}
