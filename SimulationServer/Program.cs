using System;
using System.IO;
using System.Reflection;
using System.Threading;

using Agrotera.Scripting;
using Agrotera.Setting;

namespace SimulationServer
{
	class Program
	{
		public static bool Run = false;
		private static int SleepTime = 20;

		private static object Locker = new object();

		public static void SetSleepTime(int t)
		{
			lock(Locker)
				SleepTime = t;
		}

		static void Main(string[] args)
		{
			string file = string.Empty;
			if(args.Length > 0)
				file = args[0];

			if (!File.Exists(file))
				file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "simserverconfig.xml");

			ScenarioConfig config = GetConfig(file);

			ScriptEngine.ScanFolder(config.PluginsDir);

			ScriptEngine.Init();

			BindConfig(config);

			Server server = new Server();
			server.Startup(config);

			while(!Run)
			{
				server.Update();

				int t = 20;
				lock(Locker)
					t = SleepTime;

				Thread.Sleep(t);
			}

			server.Shutdown();
		}

		static ScenarioConfig GetConfig(string arg)
		{
			ScenarioConfig cfg = ScenarioConfig.IO.Read(arg);

			if (cfg.PluginsDir == string.Empty)
				cfg.PluginsDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "plugins");

			return cfg;
		}

		/// <summary>
		/// Makes sure the config is bound to a valid scenario and campaign
		/// </summary>
		/// <param name="config"></param>
		static void BindConfig(ScenarioConfig config)
		{
			// make sure we have some kind of campaign to run
			if(config.ScenarioName == string.Empty || Scenario.FindScenario(config.ScenarioName) == null)
			{
				var s = Scenario.GetAnyScenario();
				if(s == null)
				{
					Console.WriteLine("No Scenarios to run");
					Environment.Exit(0);
				}
				config.ScenarioName = s.Name;
			}

			var scenario = Scenario.FindScenario(config.ScenarioName);

			if(config.CampaignName == string.Empty) // the config did not specify a campaign
			{
				if(scenario.Campaigns.Count != 0)           // the scenario only works with some campaigns, so use the first one
					config.CampaignName = scenario.Campaigns[0];
			}
			else
			{
				if(scenario.Campaigns.Count != 0)       // verify that the campaign the config wants is valid for the scenario
				{
					if(!scenario.Campaigns.Contains(config.CampaignName))
						config.CampaignName = scenario.Campaigns[0];
				}
			}

			if(config.CampaignName == string.Empty || Campaign.Find(config.CampaignName) == null)   // if we cant' find the campaign, use a default
				config.CampaignName = Campaign.GetDefaultCampaign().Name;
		}
	}
}
