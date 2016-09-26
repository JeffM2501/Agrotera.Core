using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Agrotera.Core;
using Agrotera.Core.Areas;
using Agrotera.Core.Entities;

namespace Agrotera.Setting
{
    public class Campaign
    {
        public static Dictionary<string, Campaign> CampaignList = new Dictionary<string, Campaign>();

        public static Campaign Get(string name)
        {
            string upperName = name.ToUpperInvariant();
            if (CampaignList.ContainsKey(upperName))
                return CampaignList[upperName];

            Campaign c = new Campaign();
            c.Name = name;
            CampaignList.Add(upperName, c);
            return c;
        }

        public static Campaign Find(string name)
        {
            string upperName = name.ToUpperInvariant();
            if (CampaignList.ContainsKey(upperName))
                return CampaignList[upperName];

            return null;
        }

		public static Campaign GetDefaultCampaign()
		{
			foreach(var c in CampaignList.Values)
				return c;

			return null;
		}

        public string Name = string.Empty;

        [XmlIgnore]
        public Dictionary<string, Faction> Factions = new Dictionary<string, Faction>();

        public ConfigurationData Configuration = new ConfigurationData();

        public Dictionary<string, object> GlobalDataCache = new Dictionary<string, object>();

        public Faction NewFaction(string name)
        {
            return NewFaction(name, Faction.Generalizations.Unclassified);
        }

        public Faction NewFaction(string name, Faction.Generalizations generalization)
        {
            string upperName = name.ToUpperInvariant();
            if (Factions.ContainsKey(upperName))
                return Factions[upperName];

            Faction faction = new Faction();
            faction.Name = name;
            faction.Generalization = generalization;
            Factions.Add(upperName, faction);
            return faction;
        }

        public Faction FindFaction(string name)
        {
            string upperName = name.ToUpperInvariant();
            if (Factions.ContainsKey(upperName))
                return Factions[upperName];

            return null;
        }

        public ScienceDatabase ScienceDB = null;
        internal Campaign()
        {
            ScienceDB = new ScienceDatabase(this);
        }

        public List<Entity.EntityTemplate> Templates = new List<Entity.EntityTemplate>();

        public Entity.EntityTemplate NewTemplate(string name)
        {
            Entity.EntityTemplate template = new Entity.EntityTemplate();
            template.EntityClass = Entity.EntityTemplate.EntityClasses.Basic;
            template.Name = name;
            Templates.Add(template);
            return template;
        }

        public T NewTemplate<T>(string name) where T : Entity.EntityTemplate, new()
        {
            T template = new T();
            template.Name = name;
            Templates.Add(template);
            return template;
        }

        public Entity.EntityTemplate FindTemplate(string name)
        {
            return Templates.Find(x => x.Name == name);
        }

        public class CampaignRuntimeEventInfo : EventArgs
        {
            public Campaign CurrentCampaign = null;
            public Scenario CurrentScenario = null;
            public Zone CurrentZone = null;
        }

        internal CampaignRuntimeEventInfo RuntimeEventArgs = new CampaignRuntimeEventInfo();

        public event EventHandler<CampaignRuntimeEventInfo> CampaignLoaded;
        public event EventHandler<CampaignRuntimeEventInfo> ScenarioStarted;
        public event EventHandler<CampaignRuntimeEventInfo> ZoneStarted;

        public void Load()
        {
            if (CampaignLoaded == null)
                return;

            CampaignLoaded(this, RuntimeEventArgs);
        }

		public void ScenarioStartup()
        {
            if (ScenarioStarted == null)
                return;

            ScenarioStarted(this, RuntimeEventArgs);
        }

		public void ZoneStartup()
        {
            if (ZoneStarted == null)
                return;

            ZoneStarted(this, RuntimeEventArgs);
        }

        public class CampaignTemplateQueryEventArgs : EventArgs
        {
            public string Query = string.Empty;
            public string[] Arguments = new string[0];

            public CampaignRuntimeEventInfo RuntimeEventArgs = null;

            public Entity.EntityTemplate ReturnedTemplate = null;
        }

        public event EventHandler<CampaignTemplateQueryEventArgs> QueryPlayerShipType;
        public event EventHandler<CampaignTemplateQueryEventArgs> QueryShipForFaction;

        public class CampaignStringQueryEventArgs : EventArgs
        {
            public string Query = string.Empty;
            public string[] Arguments = new string[0];
            public CampaignRuntimeEventInfo RuntimeEventArgs = null;

            public string ReturnedValue = null;
        }

        public event EventHandler<CampaignStringQueryEventArgs> QueryFactionFromGeneralization = null;

		public Ship.ShipTemplate GetPlayerShipType(string factionName, string classType)
		{
			if(QueryPlayerShipType == null)
				return null;

			CampaignTemplateQueryEventArgs args = new CampaignTemplateQueryEventArgs();

			args.Query = "GetPlayerShipType";
			args.Arguments = new string[]{ classType, factionName};

			args.RuntimeEventArgs = RuntimeEventArgs;
			args.ReturnedTemplate = null;

			QueryPlayerShipType(this, args);
			return args.ReturnedTemplate as Ship.ShipTemplate;
		}

		public Ship.ShipTemplate GetShipForFaction(Faction faction, string[] args)
        {
            if (QueryShipForFaction == null)
                return null;

            CampaignTemplateQueryEventArgs eventArgs = new CampaignTemplateQueryEventArgs();

            eventArgs.Query = faction.Name;
            eventArgs.RuntimeEventArgs = RuntimeEventArgs;
            eventArgs.ReturnedTemplate = null;
            eventArgs.Arguments = args;
            QueryShipForFaction(this, eventArgs);
            return eventArgs.ReturnedTemplate as Ship.ShipTemplate;
        }

        public Faction GetFactionByGeneralization(Faction.Generalizations generalization)
        {
            string desiredFactionName = string.Empty;

            if (QueryFactionFromGeneralization != null)
            {
                CampaignStringQueryEventArgs queryArgs = new CampaignStringQueryEventArgs();
                queryArgs.Query = generalization.ToString();
                queryArgs.RuntimeEventArgs = RuntimeEventArgs;
                QueryFactionFromGeneralization(this, queryArgs);
                desiredFactionName = queryArgs.ReturnedValue;
            }

            if (desiredFactionName != string.Empty && Factions.ContainsKey(desiredFactionName))
                return Factions[desiredFactionName];

            // nope, try and find one that matches
            foreach (Faction faction in Factions.Values)
            {
                if (faction.Generalization == generalization)
                    return faction;
            }
            return null;
        }

        public Faction GetFactionByGeneralization(Faction.Generalizations generalization, Faction butNot)
        {
            int count = 0;
            while (count < 10)
            {
                Faction faction = GetFactionByGeneralization(generalization);
                if (faction != butNot)
                    return faction;
            }
            return GetFactionByGeneralization(generalization);
        }
    }
}
