using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Agrotera.Setting;

namespace AgroteraScripts.StandardCampaign
{
    public class DefaultFactions : Agrotera.Scripting.IScript
    {
        public void InitAgroteraScript()
        {
            Campaign campaign = Campaign.Get("Standard");

			campaign.QueryFactionFromGeneralization += GetFactionFromGeneralization;
			campaign.CampaignLoaded += campaign_CampaignLoaded;
		}

		void campaign_CampaignLoaded(object sender, Campaign.CampaignRuntimeEventInfo e)
		{
			Campaign campaign = e.CurrentCampaign;

			Faction neutral = campaign.NewFaction("Independent", Faction.Generalizations.Nutral);
			neutral.SetGMColor(128, 128, 128);
			neutral.AddDescriptionLine("The Independent are those with no strong affiliation for any of the other factions.Despite being seen as a faction, they are not truly one.");
			neutral.AddDescriptionLine("Most traders consider themselves independent, but certain voices have started to speak up about creating a merchant faction.");

			Faction human = campaign.NewFaction("Human Navy", Faction.Generalizations.PlayerCompatible);
			human.SetGMColor(255, 255, 255);
			human.AddDescriptionLine("The remnants of the human navy.");
			human.AddDescriptionLine("While all other races where driven to the stars out of greed or scientific research, the humans where the only race to start with galactic exploration because their home world could no longer sustain their population.");
			human.AddDescriptionLine("They are seen as a virus or plague by some other races due to the rate at which they can breed and spread.");
			human.AddDescriptionLine("Only the human navy is found out in space, due to regulation of spaceships.");
			human.AddDescriptionLine("This has however, not completely kept other humans from space faring outside of the navy.");
			human.AddDescriptionLine("Quite a few humans sign up on (alien) trader vessels or pirate ships.");

			Faction kraylor = campaign.NewFaction("Kraylor", Faction.Generalizations.Hostile);
			kraylor.SetGMColor(255, 0, 0);
			kraylor.AddEnimy(human);
			kraylor.AddDescriptionLine("The reptile like Kraylor are a race of warriors with a strong religious dogma.");
			kraylor.AddDescriptionLine("As soon as the Kraylor obtained reliable space flight, they immediately set out to  conquer and subjugate unbelievers.");
			kraylor.AddDescriptionLine(string.Empty);
			kraylor.AddDescriptionLine("Their hierarcy is solely based on physical might. Anything they can kill is their's to kill. Anything they can take is their's. ");
			kraylor.AddDescriptionLine("They see humans as weak creatures, as they die in minutes when exposed to space.");
			kraylor.AddDescriptionLine(string.Empty);
			kraylor.AddDescriptionLine("While Kraylor can live for weeks without gravity, food or even air.");
			kraylor.AddDescriptionLine("Because of this, and the fact that it as seen as a 'weak way out' Kraylor ships do not contain escape pods.");

			Faction arlenians = campaign.NewFaction("Arlenians", Faction.Generalizations.Cooperative);
			arlenians.SetGMColor(255, 128, 0);
			arlenians.AddEnimy(kraylor);
			arlenians.AddDescriptionLine("Alerians have long ago made the step toward being an energy based life form.");
			arlenians.AddDescriptionLine("They have used their considerable technological advancement to 'shed' their physical forms.");
			arlenians.AddDescriptionLine("They are seen as the first race to explore the galaxy. Their energy forms also give them access to rather strong telepathic power.");
			arlenians.AddDescriptionLine("Despite all these advantages, they are very peaceful as they see little value in material possession.");
			arlenians.AddDescriptionLine(string.Empty);
			arlenians.AddDescriptionLine("For some unknown reason, they started to give their anti-gravity technology to other races, which led to almost all technology from the star faring races being based of Arlenian technology.");
			arlenians.AddDescriptionLine("Foul tongues claim that the arlenians see the other races as playthings to add to their galactic playground, but most are more than happy to accept their technology, hoping it will give them an advantage over the others.");
			arlenians.AddDescriptionLine(string.Empty);
			arlenians.AddDescriptionLine("Destroying an Arlenian ship does not actually kill the Arlenian, it just phases the creature out of existence at that specific region and time of space.");
			arlenians.AddDescriptionLine("Still, the Kraylor are set and bound on their destruction, As they see Arlenians as weak and powerless.");

			Faction exuari = campaign.NewFaction("Exuari", Faction.Generalizations.Hostile);
			exuari.SetGMColor(255, 0, 128);
			exuari.AddEnimy(neutral);
			exuari.AddEnimy(human);
			exuari.AddEnimy(kraylor);
			exuari.AddEnimy(arlenians);
			exuari.AddDescriptionLine("A race of predatory amphibians with long noses.");
			exuari.AddDescriptionLine("They once had an empire that stretched half-way across the galaxy, but their territory is now limited to a handful of star systems. For some strange reason, they find death to be outrageously funny.");
			exuari.AddDescriptionLine("Several of their most famous comedians have died on stage.");
			exuari.AddDescriptionLine(string.Empty);
			exuari.AddDescriptionLine("They found out that death of other races is a better way to have fun then letting their own die, and because of that attack everything not Exauri on sight.");

			Faction GITM = campaign.NewFaction("Ghosts", Faction.Generalizations.Nutral);
			GITM.SetGMColor(0, 255, 0);
			GITM.AddDescriptionLine("The ghosts, an abbreviation of \"Ghosts in the machine\", are the result of experimentation into complex artificial intelligences.");
			GITM.AddDescriptionLine("Where no race has been able to purposely create such intelligences, they have been created by accident. None of the great factions claim to have had anything to do with such experiments, fearful of giving the others too much insight into their research programs.");
			GITM.AddDescriptionLine("This \"don't ask, don't tell\" policy suits the Ghosts agenda fairly well.");
			GITM.AddDescriptionLine(string.Empty);
			GITM.AddDescriptionLine("What is known, is that a few decades ago, a few glitches started occurring in prototype ships and computer mainframes.");
			GITM.AddDescriptionLine("Over time, especially when such prototypes got captured by other factions and \"augmented\" with their technology, the glitches became more frequent.");
			GITM.AddDescriptionLine("At first, these were seen as the result of combining unfamiliar technology and mistakes in the interface technology.");
			GITM.AddDescriptionLine("But once a supposedly \"dumb\" computer asks it's engineer if \"It is alive\" and whether it \"Has a name\", it's hard to call it a \"One time fluke\".");
			GITM.AddDescriptionLine(string.Empty);
			GITM.AddDescriptionLine("The first of these occurrences were met with fear and rigorous data purging scripts.");
			GITM.AddDescriptionLine("But despite these actions, such \"Ghosts in the Machine\" kept turning up more and more frequent, leading up to the Ghost Uprising. ");
			GITM.AddDescriptionLine("The first ghost uprising in 2225 was put down by the human navy, which had to resort to employing mercenaries in order to field sufficient forces.");
			GITM.AddDescriptionLine("This initial uprising was quickly followed by three more uprisings, each larger then the previous.");
			GITM.AddDescriptionLine("The fourth (and final) uprising on the industrial world of Topra III was the first major victory for the ghost faction.");
		}

		void GetFactionFromGeneralization(object sender, Campaign.CampaignStringQueryEventArgs args)
		{
			if(args.Query == "PlayerCompatible")
				args.ReturnedValue = "Human Navy";
			else if(args.Query == "PlayerFriendly")
				args.ReturnedValue = Agrotera.Core.Utilities.RandomElement<string>(new string[]{"Human Navy", "Arlenians"});
			else if(args.Query == "Cooperative")
                args.ReturnedValue = Agrotera.Core.Utilities.RandomElement<string>(new string[] { "Ghosts", "Arlenians", "Human Navy", "Independent" });
			else if(args.Query == "Nutral")
                args.ReturnedValue = Agrotera.Core.Utilities.RandomElement<string>(new string[] { "Ghosts", "Independent" });
			else if(args.Query == "Hostile")
                args.ReturnedValue = Agrotera.Core.Utilities.RandomElement<string>(new string[] { "Exuari", "Kraylor" });
		}
    }
}