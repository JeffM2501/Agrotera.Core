using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SimulationServer
{
	public class ScenarioConfig
	{
		[XmlIgnore]
		public string ConfigLocation = string.Empty;

		public string PluginsDir = string.Empty;

		public string CampaignName = string.Empty;
		public string ScenarioName = string.Empty;

		public int Port = 1700;

		public static class IO
		{
			public static ScenarioConfig Read(string path)
			{
				ScenarioConfig cfg = null;

				FileInfo file = new FileInfo(path);
				if(file.Exists)
				{
					try
					{
						XmlSerializer xml = new XmlSerializer(typeof(ScenarioConfig));
						StreamReader sr = file.OpenText();
						cfg = xml.Deserialize(sr) as ScenarioConfig;
						sr.Close();
					}
					catch (System.Exception /*ex*/)
					{
						cfg = null;
					}
				}

				if(cfg == null)
					cfg = new ScenarioConfig();

				cfg.ConfigLocation = file.FullName;
				return cfg;
			}

			public static bool Write(ScenarioConfig config)
			{
				if(config.ConfigLocation == string.Empty)
					return false;

				FileInfo file = new FileInfo(config.ConfigLocation);
				if(file.Exists)
					file.Delete();

				try
				{
					XmlSerializer xml = new XmlSerializer(typeof(ScenarioConfig));
					var sw = file.OpenWrite();
					xml.Serialize(sw, config);
					sw.Close();
				}
				catch(System.Exception /*ex*/)
				{
					return false;
				}

				return true;
			}
		}
	}
}
