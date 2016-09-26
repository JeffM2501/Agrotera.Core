using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core;
using Agrotera.Core.Areas;
using Agrotera.Core.Types;
using Agrotera.Core.Entities;

namespace Agrotera.Setting
{
    public class Scenario
    {
        public static List<Scenario> Scenarios = new List<Scenario>();

        public static void Register(Scenario scenario)
        {
            Scenarios.Add(scenario);
        }

		public static Scenario GetAnyScenario()
		{
			if(Scenarios.Count == 0)
				return null;

			return Scenarios[0];
		}

		public static Scenario FindScenario(string name)
		{
			return Scenarios.Find(x=>x.Name == name);
		}

		public ConfigurationData Configuration = new ConfigurationData();

        public Zone Map = new Zone();
        public string Name = string.Empty;
        public string Description = string.Empty;

        public List<String> Campaigns = new List<string>(); // empty means campaign agnostic

        public Campaign BoundCampaign = new Campaign();

        // overrides

        public virtual void SetupZone()
        {

        }

        public void Load(Campaign campaign)
        {
            BoundCampaign = campaign;
            BoundCampaign.RuntimeEventArgs.CurrentCampaign = BoundCampaign;
            BoundCampaign.RuntimeEventArgs.CurrentScenario = this;

            Startup();
            BoundCampaign.ScenarioStartup();

            SetupZone();

            BoundCampaign.RuntimeEventArgs.CurrentZone = Map;
            BoundCampaign.ZoneStartup();
        }

        public void Stop()
        {
            Shutdown();
        }

        public virtual void Startup()
        {

        }

        public virtual void Shutdown()
        {
        }

        public virtual string GetName()
        {
            return string.Empty;
        }

        public virtual bool Update(Tick tick)
        {
            return false;
        }

        // Scenario API

        protected void SendGlobalMessage(string from, string message)
        {

        }

        protected void SetVictory(Faction winningFaction, string message)
        {
        }

        protected virtual Faction GetPlayerFaction()
        {
            Faction playerFaction = BoundCampaign.GetFactionByGeneralization(Faction.Generalizations.PlayerCompatible);
            if (playerFaction == null)
                return new Faction("Independent Mercenary #" + Utilities.RNG.Next().ToString());
            return playerFaction;
        }

        public virtual Ship GetPlayerShip(string classType, string faction)
        {
            Ship.ShipTemplate template = BoundCampaign.GetPlayerShipType(faction, classType);
            if (template == null)
                return new Ship(Map);

            Ship ship = Map.AddPlayableShip(template);
            if (ship != null)
                ship.Owner = GetPlayerFaction().Name;
            return ship;
        }
    }
}
