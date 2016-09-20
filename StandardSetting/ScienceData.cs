using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Agrotera.Core;
using Agrotera.Setting;

namespace AgroteraScripts.StandardCampaign
{
    public class DefaultScienceData : Agrotera.Scripting.IScript
    {
		public void InitAgroteraScript()
		{
            var campaign = Campaign.Get("Standard");
			campaign.CampaignLoaded += campaign_CampaignLoaded;
		}

		void campaign_CampaignLoaded(object sender, Campaign.CampaignRuntimeEventInfo e)
		{
			var campaign = e.CurrentCampaign;
			var item = campaign.ScienceDB.New("Natural", "Asteroid");
			item.AddDescriptionLine("Asteroids are minor planets, usually smaller than a few kilometers.");
			item.AddDescriptionLine("The larger variants are sometimes referred to as planetoids.");
			item.Known = true;

			item = campaign.ScienceDB.New("Natural", "Neblua");
			item.AddDescriptionLine("Neblua are the birthing place of new stars.");
			item.AddDescriptionLine("These gas fields, usually created by the death of an old star, slowly from new stars due to the gravitational pull of the gas molecules.");
			item.AddDescriptionLine("Due to the ever changing nature of gas nebulae, most radar and scanning technology is unable to detect objects that lie within.");
			item.AddDescriptionLine("Science officers are therefore advised to rely on visual observations.");
			item.Known = true;

			item = campaign.ScienceDB.New("Weapons", "Homing Missile");
			item.SetValue("Range", "5.4km");
			item.SetValue("Damage", "35");
			item.AddDescriptionLine("This target seeking missile is the work horse of many ships.");
			item.AddDescriptionLine("It's compact enough to be fitted on frigates and packs enough punch to be used on larger ships, albeit with more than a single missile tube.");
			item.Known = true;

			item = campaign.ScienceDB.New("Weapons", "Nuke");
			item.SetValue("Range", "5.4km");
			item.SetValue("Blast radius", "1km");
			item.SetValue("Damage at center", "160");
			item.SetValue("Damage at edge", "30");
			item.AddDescriptionLine("The nuclear missile is the same as the homing missile, but with a greatly increased (nuclear) payload.");
			item.AddDescriptionLine("It is capable of taking out multiple ships in a single shot.");
			item.AddDescriptionLine("Some captains question the use of these weapons as they have lead to 'fragging' or un-intentional friendly fire. ");
			item.AddDescriptionLine("The shielding of ships should protect the crew from any harmful radiation, but seeing that these weapons are often used in the thick of battle, there is no way of knowing if the hull plating or shield will provide enough protection.");
			item.Known = true;

			item = campaign.ScienceDB.New("Weapons", "Mine");
			item.SetValue("Drop distance", "1.2km");
			item.SetValue("Trigger distance", "600m");
			item.SetValue("Blast radius", "1km'");
			item.SetValue("Damage at center", "160");
			item.SetValue("Damage at edge", "30");
			item.AddDescriptionLine("Mines are often placed in a defensive perimeter around stations.");
			item.AddDescriptionLine("There are also old mine fields scattered around the universe from older wars.");
			item.AddDescriptionLine("Some fearless captains have used mines as offensive weapons, but this is with great risk.");
			item.Known = true;

			item = campaign.ScienceDB.New("Weapons", "EMP");
			item.SetValue("Range", "5.4km");
			item.SetValue("Blast radius", "1km");
			item.SetValue("Damage at center", "160");
			item.SetValue("Damage at edge", "30");
			item.AddDescriptionLine("The EMP is a shield-only damaging weapon It matches the heavy nuke in damage but does no hull damage.");
			item.AddDescriptionLine("The EMP missile is smaller and easier to storage then the heavy nuke.");
			item.AddDescriptionLine("Thus many captains prefer it's use over nukes.");	
		}
    }
}