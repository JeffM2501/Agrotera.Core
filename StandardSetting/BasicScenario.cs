using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Agrotera.Setting;
using Agrotera.Core.Entities;
using Agrotera.Core;
using Agrotera.Core.Types;

namespace AgroteraScripts.StandardCampaign
{
	public class BasicScenario : Scenario, Agrotera.Scripting.IScript
	{
		protected bool DefaultShipCreated = false;

		public BasicScenario()
		{
			Name = "Basic Testing Scenario";
			Description = "A simple scenario with a few friendly stations, and ships of various factions. Terminates when all hostiles are removed";

			Configuration.Add(new NumberConfigurationItem("ShipCount", "Number of Ships In Scenario", "The number of enemy ships to spawn", 5,0,10, true));

			List<string> names = new List<string>();
			names.Add("Hermes Station");
			names.Add("Bigelow's Station");
			names.Add("New Horizon");
			names.Add("The Pit");

			ListConfigurationItem stationNames = new ListConfigurationItem("StationName","Friendly Station Name", "The name of the station that likes players",names.ToArray());
			Configuration.Add(stationNames);
		}

		public void InitAgroteraScript()
		{
			Scenario.Register(this);
		}

		public override void Startup()
		{
		
		}

        protected int PlayerSpawnCount = 0;

        public override Ship SpawnPlayerShip(string classType, string faction)
        {
            Ship.ShipTemplate template = BoundCampaign.GetPlayerShipType(faction, classType);
            if (template == null)
                return new Ship(Map);

            Ship ship = Map.AddPlayableShip(template);
            if (ship != null)
            {
                if (FriendlyStation == null)
                {
                    ship.Position = Utilities.RandomPostion(Map.Bounds);
                    ship.Rotation = Utilities.RandomAngle();
                }
                else
                {
                    ship.Position = new Vector3F(Utilities.RandomPostionInbetween(new Vector2F(FriendlyStation.Position), 300, 800));
                    ship.Rotation = Utilities.RandomAngle();
                }
  
                ship.Name = "USS Constitution " + PlayerSpawnCount.ToString();

                ship.Owner = GetPlayerFaction().Name;
            }

            return ship;
        }

        protected Station FriendlyStation = null;

        public override void SetupZone()
		{
			base.SetupZone();

			Map.Bounds = new Vector3F(10000, 10000, 10000);

            var playerFaction = GetPlayerFaction();
            // some backup
            FriendlyStation = Map.AddStaticItem<Station>(BoundCampaign.FindTemplate("Medium Station") as Station.StationTemplate);
            FriendlyStation.Owner = playerFaction.Name;
            FriendlyStation.Position = Utilities.RandomPostion(Map.Bounds);
            FriendlyStation.Position.Z = 0;
            FriendlyStation.Name = FriendlyStation.Owner + " Supply Depot";

			for (int i =0; i < Utilities.RNG.Next(10); i++)
			{
                Vessel friendlyShip = Map.AddAIShip(BoundCampaign.GetShipForFaction(playerFaction, null) as Ship.ShipTemplate);
				friendlyShip.Owner = FriendlyStation.Owner;
				friendlyShip.Position = new Vector3F(Utilities.RandomPostionInbetween(new Vector2F(FriendlyStation.Position),100,500));
                friendlyShip.Rotation = Utilities.RandomAngle();
                friendlyShip.Name = playerFaction.Name + " Patrol Ship " + (i + 1).ToString();
			}

			// some baddies
            var baddieFaction = BoundCampaign.GetFactionByGeneralization(Faction.Generalizations.Hostile);

			Station hostileStation = Map.AddStaticItem<Station>(BoundCampaign.FindTemplate("Small Station") as Station.StationTemplate);
            hostileStation.Owner = baddieFaction.Name;
			hostileStation.Position = new Vector3F(Utilities.RandomPostionFurtherThan(new Vector2F(Map.Bounds),new Vector2F(friendlyStation.Position),3000));
			hostileStation.Rotation = Utilities.RandomAngle();
            hostileStation.Name = baddieFaction.Name + " outpost";

			for (int i =0; i < Utilities.RNG.Next(5); i++)
			{
                Vessel hostileShip = Map.AddAIShip(BoundCampaign.GetShipForFaction(baddieFaction, null) as Ship.ShipTemplate);
				hostileShip.Owner = hostileStation.Owner;
				hostileShip.Position = new Vector3F(Utilities.RandomPostionInbetween(new Vector2F(hostileStation.Position),100,500));
				hostileShip.Rotation = Utilities.RandomAngle();
                hostileShip.Name = baddieFaction.Name + " Patrol Ship " + (i + 1).ToString();
			}

			// some friends
            var friendFaction = BoundCampaign.GetFactionByGeneralization(Faction.Generalizations.Cooperative);

            Station coopStation = Map.AddStaticItem<Station>(BoundCampaign.FindTemplate("Large Station") as Station.StationTemplate);
            coopStation.Owner = friendFaction.Name;
            coopStation.Name = "Friendly " + friendFaction.Name + " Trade Station";
			coopStation.Position = new Vector3F(Utilities.RandomPostionFurtherThan(new Vector2F(Map.Bounds), new Vector2F(hostileStation.Position), 3000));

			for(int i = 0; i < Utilities.RNG.Next(20); i++)
			{
                Vessel coopShip = Map.AddAIShip(BoundCampaign.GetShipForFaction(friendFaction, null) as Ship.ShipTemplate);
                coopShip.Owner = BoundCampaign.GetFactionByGeneralization(Faction.Generalizations.Cooperative, friendFaction).Name;
				coopShip.Position = new Vector3F(Utilities.RandomPostionInbetween(new Vector2F(coopStation.Position), 100, 500));
				coopShip.Rotation = Utilities.RandomAngle(); 
				coopShip.Name = coopShip.Owner + " Trade Ship " + (i + 1).ToString();
			}
		}
	}
}