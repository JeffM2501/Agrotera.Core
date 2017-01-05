using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ShipClient;

namespace ClientTester
{
	class Program
	{
		static void Main(string[] args)
		{
			ShipClient.ShipConnection connection = new ShipClient.ShipConnection("127.0.0.1", 2501);

			connection.Connected += Connection_Connected;
			connection.ShipAssigned += Connection_ShipAssigned;
			while(true)
			{
				connection.Update();

				if (connection.PlayerShip != null)
				{
					if (connection.PlayerShip.KnownEntities.Count > 0)
					{
						ShipClient.UserShip.ShipCentricSensorEntity sensorItem = connection.PlayerShip.KnownEntities[3] as ShipClient.UserShip.ShipCentricSensorEntity;

						Console.WriteLine(string.Format("Item {0}", sensorItem.BaseEntity.Name));
						Console.WriteLine(string.Format("Abs Position {0} {1} {2}", sensorItem.LastPosition.X, sensorItem.LastPosition.Y, sensorItem.LastPosition.Z));
						Console.WriteLine(string.Format("Rell Position {0} {1} {2}", sensorItem.ShipRelativePosition.X, sensorItem.ShipRelativePosition.Y, sensorItem.ShipRelativePosition.Z));

					}

				}
				System.Threading.Thread.Sleep(100);
			}
		}

		private static void Connection_ShipAssigned(object sender, EventArgs e)
		{
			Console.WriteLine("Ship assigned ");
		}

		private static void Connection_Connected(object sender, ShipConnection.ConnectedEventArgs e)
		{
			Console.WriteLine("connected");
			e.DesiredShipArguments.Add("Default");
		}
	}
}
