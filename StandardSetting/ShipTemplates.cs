using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Agrotera.Setting;
using Agrotera.Core;
using Agrotera.Core.Entities;
using Agrotera.Core.Types;

namespace AgroteraScripts.StandardCampaign
{
    public static class AssemblyFinder
    {
        public static System.Reflection.Assembly GetThisAssembly()
        {
            return System.Reflection.Assembly.GetExecutingAssembly();
        }
    }

    public class DefaultShipTemplates : Agrotera.Scripting.IScript
    {
		private bool UseWarp = true;

		public void InitAgroteraScript()
		{
			Campaign campaign = Campaign.New("Standard");

			campaign.Configuration.Add(new BooleanConfigruationItem("UseWarp", "Enable Warp Drive", "When selected, allows ships to have warp drive as an option instead of just JumpDrives", true));
       
			campaign.QueryShipForFaction += GetShipForFaction;
			campaign.QueryPlayerShipType += GetPlayerShipType;
			campaign.CampaignLoaded += campaign_CampaignLoaded;
		}

		void campaign_CampaignLoaded(object sender, Campaign.CampaignRuntimeEventInfo e)
		{
			Campaign campaign = e.CurrentCampaign;

			if(campaign.Configuration.Exists("UseWarp"))
				UseWarp = (campaign.Configuration.Find("UseWarp") as BooleanConfigruationItem).Value;

			// common stations
            Station.StationTemplate smallStation = campaign.NewTemplate<Station.StationTemplate>("Small Station");
			smallStation.Model = "space_staton_4";
			smallStation.Hull = 150;
			smallStation.Shields = 300;
			smallStation.Radius = 50;

            Station.StationTemplate mediumStation = campaign.NewTemplate<Station.StationTemplate>("Medium Station");
			mediumStation.Model = "space_station_3";
			mediumStation.Hull = 400;
			mediumStation.Shields = 800;
			mediumStation.Radius = 75;

            Station.StationTemplate largeStation = campaign.NewTemplate<Station.StationTemplate>("Large Station");
			largeStation.Model = "space_station_2";
			largeStation.Hull = 500;
			largeStation.Shields = 1000;
			largeStation.Radius = 100;

            Station.StationTemplate hugeStation = campaign.NewTemplate<Station.StationTemplate>("Huge Station");
			hugeStation.Model = "space_station_1";
			hugeStation.Hull = 800;
			hugeStation.Shields = 1200;
			hugeStation.Radius = 150;

			// player ships
			SetupPlayerCruiser(campaign.NewTemplate<Ship.ShipTemplate>("Player Cruiser"), campaign);
            SetupPlayerMissileCruiser(campaign.NewTemplate<Ship.ShipTemplate>("Player Missile Cruiser"), campaign);
            SetupPlayerFighter(campaign.NewTemplate<Ship.ShipTemplate>("Player Fighter"), campaign);

			// AI Ships
			// Merchant ships
            SetupTug(campaign.NewTemplate<Ship.ShipTemplate>("Tug"), campaign);

			// fighters
            SetupSmallFighter(campaign.NewTemplate<Ship.ShipTemplate>("Fighter"), campaign);

			// Fleet Ships
            SetupKarnackCruiserMk1(campaign.NewTemplate<Ship.ShipTemplate>("Cruiser"), campaign);
            SetupKarnackCruiserMk2(campaign.NewTemplate<Ship.ShipTemplate>("Upgraded Cruiser"), campaign);
            SetupPolarisCruiser(campaign.NewTemplate<Ship.ShipTemplate>("Missile Cruiser"), campaign);
            SetupGunship(campaign.NewTemplate<Ship.ShipTemplate>("Advanced Gunship"), campaign);
            SetupStrikeShip(campaign.NewTemplate<Ship.ShipTemplate>("Strikeship"), campaign);
            SetupAdvancedStrikeShip(campaign.NewTemplate<Ship.ShipTemplate>("Advanced Strikeship"), campaign);
            SetupDreadnought(campaign.NewTemplate<Ship.ShipTemplate>("Dreadnought"), campaign);
            SetupBattleStation(campaign.NewTemplate<Ship.ShipTemplate>("Battle Station"), campaign);
            SetupBlockadeRunner(campaign.NewTemplate<Ship.ShipTemplate>("Blockade Runner"), campaign);

			// Fleet Stations
			SetupWeaponsPlatform(campaign.NewTemplate<Station.StationTemplate>("Weapons Platform"), campaign);
		}

		void GetPlayerShipType(object sender, Campaign.CampaignTemplateQueryEventArgs args)
		{
			args.ReturnedTemplate = args.RuntimeEventArgs.CurrentCampaign.FindTemplate("Player Cruiser");
		}

		void GetShipForFaction(object sender, Campaign.CampaignTemplateQueryEventArgs args)
		{
			switch(args.Query)
			{
				case "Independent":
                    args.ReturnedTemplate = args.RuntimeEventArgs.CurrentCampaign.FindTemplate(Agrotera.Core.Utilities.RandomElement(new string[] { "Tug", "Blockade Runner" }));
					break;

				case "Human Navy":
                    args.ReturnedTemplate = args.RuntimeEventArgs.CurrentCampaign.FindTemplate(Agrotera.Core.Utilities.RandomElement(new string[] { "Cruiser", "Upgraded Cruiser", "Missile Cruiser" }));
					break;
				
				case "Kraylor":
                    args.ReturnedTemplate = args.RuntimeEventArgs.CurrentCampaign.FindTemplate(Agrotera.Core.Utilities.RandomElement(new string[] { "Cruiser", "Upgraded Cruiser", "Dreadnought" }));
					break;

				case "Arlenians":
                    args.ReturnedTemplate = args.RuntimeEventArgs.CurrentCampaign.FindTemplate(Agrotera.Core.Utilities.RandomElement(new string[] { "Cruiser", "Blockade Runner", "Upgraded Cruiser" }));
					break;

				case "Exuari":
                    args.ReturnedTemplate = args.RuntimeEventArgs.CurrentCampaign.FindTemplate(Agrotera.Core.Utilities.RandomElement(new string[] { "Cruiser", "Blockade Runner", "Strikeship" }));
					break;

				case "Ghosts":
					args.ReturnedTemplate = args.RuntimeEventArgs.CurrentCampaign.FindTemplate(Agrotera.Core.Utilities.RandomElement(new string[] {"Upgraded Cruiser", "Blockade Runner", "Dreadnought" }));
					break;
			}
		}

        void SetupWeaponsPlatform(Station.StationTemplate station, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Station", "Weapons Platform");
			item.AddDescriptionLine("The weapons-platform is a stationary platform with beam-weapons.");
			item.AddDescriptionLine("It's not maneuverable, but it's beam weapons deal huge amounts of damage from any angle");
			item.Known = true;

			station.ScienceDBID = item.ID;

			station.Model = "space_staton_4";
			
			station.NewBeamWeapon(30,   0, 4000.0f, 1.5f, 20);
			station.NewBeamWeapon(30,  60, 4000.0f, 1.5f, 20);
			station.NewBeamWeapon(30, 120, 4000.0f, 1.5f, 20);
			station.NewBeamWeapon(30, 180, 4000.0f, 1.5f, 20);
			station.NewBeamWeapon(30, 240, 4000.0f, 1.5f, 20);
			station.NewBeamWeapon(30, 300, 4000.0f, 1.5f, 20);

			station.Hull = 70;
			station.Shields = 120;

			station.DefaultBehavor = "battle_station_AI";
			station.SetScienceItemFields(item);
		}

        void SetupBlockadeRunner(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Blockade Runner");
			item.AddDescriptionLine("Fabricated by: Zoop");
			item.AddDescriptionLine("Blockade runner is a reasonably fast, high shield, slow on weapons ship designed to break trough defense lines and deliver/smuggle goods.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "battleship_destroyer_3_upgraded";
			ship.NewBeamWeapon(50, -15, 1000.0f, 6.0f, 6);
			ship.NewBeamWeapon(50, 15, 1000.0f, 6.0f, 6);

			ship.NewBeamWeapon(60, -15, 1000.0f, 6.0f, 8);
			ship.NewBeamWeapon(60,  15, 1000.0f, 6.0f, 8);
			ship.NewBeamWeapon(25,  170, 1000.0f, 6.0f, 8);
			ship.NewBeamWeapon(25, 190, 1000.0f, 6.0f, 8);

			ship.Hull = 70;
			ship.FrontShields = 100;
			ship.RearShields = 150;

			ship.SublightSpeed = 60; ship.RotatonSpeed = 15; ship.SublightAcceleration = 25;

            ship.FTLType = Ship.FTLTypes.None;

			ship.AddStores("Trade Goods", 250);
			ship.AddWeaponStores(MissileWeaponTypes.Homing, 25);
			ship.AddWeaponStores(MissileWeaponTypes.Nuke, 25);
			ship.AddWeaponStores(MissileWeaponTypes.Mine, 50);
			ship.AddWeaponStores(MissileWeaponTypes.EMP, 50);

			ship.DefaultBehavor = "runner_AI";
			ship.SetScienceItemFields(item);
		}


        void SetupBattleStation(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Battle Station");
			item.AddDescriptionLine("The battle station is a huge ship with many defensive features.");
			item.AddDescriptionLine("More station than ship, it can provide docking facilities for smaller ships.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "Ender Battlecruiser";

			ship.NewBeamWeapon(120, -90, 2500.0f, 6.1f, 4);
			ship.NewBeamWeapon(120, -90, 2500.0f, 6.0f, 4);
			ship.NewBeamWeapon(120, 90, 2500.0f, 6.1f, 4);
			ship.NewBeamWeapon(120, 90, 2500.0f, 6.0f, 4);
			ship.NewBeamWeapon(120, -90, 2500.0f, 5.9f, 4);
			ship.NewBeamWeapon(120, -90, 2500.0f, 6.2f, 4);
			ship.NewBeamWeapon(120, 90, 2500.0f, 5.9f, 4);
			ship.NewBeamWeapon(120, 90, 2500.0f, 6.2f, 4);
			ship.NewBeamWeapon(120, -90, 2500.0f, 6.1f, 4);
			ship.NewBeamWeapon(120, -90, 2500.0f, 6.0f, 4);
			ship.NewBeamWeapon(120, 90, 2500.0f, 6.1f, 4);
			ship.NewBeamWeapon(120, 90, 2500.0f, 6.0f, 4);

			ship.Hull = 120;
			ship.FrontShields = 1500;
			ship.RearShields = 1500;

			ship.SublightSpeed = 20; ship.RotatonSpeed = 1.5f; ship.SublightAcceleration = 3;
            ship.FTLType = Ship.FTLTypes.JumpDrive;
			ship.FTLLimit = 100;

			ship.DefaultBehavor = "battle_cruiser_AI";
			ship.SetScienceItemFields(item);
		}

        void SetupDreadnought(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Dreadnought");
			item.AddDescriptionLine("The Dreadnought is a flying fortress, it's slow, slow to turn, but packs a huge amount of beam weapons in the front.");
			item.AddDescriptionLine("Attacking this type of ship head-on is suicide for most vessels.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "battleship_destroyer_1_upgraded";

			ship.NewBeamWeapon(90, -25, 1500.0f, 6.0f, 8);
			ship.NewBeamWeapon(90,  25, 1500.0f, 6.0f, 8);
			ship.NewBeamWeapon(100, -60, 1000.0f, 6.0f, 8);
			ship.NewBeamWeapon(100,  60, 1000.0f, 6.0f, 8);
			ship.NewBeamWeapon(30,   0, 2000.0f, 6.0f, 8);
			ship.NewBeamWeapon(100, 180, 1200.0f, 6.0f, 8);

			ship.Hull = 120;
			ship.FrontShields = 300;
			ship.RearShields = 300;

			ship.SublightSpeed = 30; ship.RotatonSpeed = 1.5f; ship.SublightAcceleration = 5;
            ship.FTLType = Ship.FTLTypes.JumpDrive;
			ship.FTLLimit = 50;

			ship.DefaultBehavor = "battle_cruiser_AI";
			ship.SetScienceItemFields(item);
		}

        void SetupAdvancedStrikeShip(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Advanced Strikeship");
			item.AddDescriptionLine("Fabricated by: Void-Tech LLC");
			item.AddDescriptionLine("A competitor to the strikeship, equipped with a jump drive instead of traditional warp capability");
			item.AddDescriptionLine("Built for fast attack assaults, it focuses on offensive capabilities at the expense of defenses.");
			item.AddDescriptionLine("While not quite as tough as a traditional strikeship, the jump drive allows it arrive on site much faster.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "dark_fighter_6";
			ship.NewBeamWeapon(50, -15, 1000.0f, 6.0f, 6);
			ship.NewBeamWeapon(50, 15, 1000.0f, 6.0f, 6);

			ship.Hull = 70;
			ship.FrontShields = 50;
			ship.RearShields = 30;

			ship.SublightSpeed = 45; ship.RotatonSpeed = 12; ship.SublightAcceleration = 15;
            ship.FTLType = Ship.FTLTypes.JumpDrive;
			ship.FTLLimit = 1000;

			ship.DefaultBehavor = "strike_cruiser_AI";
			ship.SetScienceItemFields(item);
		}

        void SetupStrikeShip(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Strikeship");
			item.AddDescriptionLine("Fabricated by: Insight Corporation.");
			item.AddDescriptionLine("An upgraded and warp capable agile fighter.");
			item.AddDescriptionLine("Built for fast attack assaults, it focuses on offensive capabilities at the expense of defenses.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "dark_fighter_6";
			ship.NewBeamWeapon(40, -5, 1000.0f, 6.0f, 6);
			ship.NewBeamWeapon(40, 5, 1000.0f, 6.0f, 6);

			ship.Hull = 100;
			ship.FrontShields = 100;
			ship.RearShields = 30;

			ship.SublightSpeed = 70; ship.RotatonSpeed = 12; ship.SublightAcceleration = 11;
            ship.FTLType = UseWarp ? Ship.FTLTypes.WarpDrive : Ship.FTLTypes.JumpDrive;
			ship.FTLLimit = 1000;

			ship.DefaultBehavor = "strike_cruiser_AI";
			ship.SetScienceItemFields(item);
		}

        void SetupGunship(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Advanced Gunship");
			item.AddDescriptionLine("Fabricated by: Insight Corporation.");
			item.AddDescriptionLine("The advanced gunship is quipped with 2 homing missiles to do initial damage and 2 front firing beams to complete the kill.");
			item.AddDescriptionLine("It's designed to quickly take out the enemies weaker then itself.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "battleship_destroyer_4_upgraded";
			ship.NewBeamWeapon(50, -15, 1000.0f, 6.0f, 6);
			ship.NewBeamWeapon(50, 15, 1000.0f, 6.0f, 6);

			ship.AddMissileTubes(2, 8);

			ship.Hull = 100;
			ship.FrontShields = 100;
			ship.RearShields = 80;

			ship.SublightSpeed = 60; ship.RotatonSpeed = 5; ship.SublightAcceleration = 10;

			ship.AddWeaponStores(MissileWeaponTypes.Homing, 8);
            ship.FTLType = Ship.FTLTypes.None;

			ship.DefaultBehavor = "strike_cruiser_AI";
			ship.SetScienceItemFields(item);
		}

        void SetupPolarisCruiser(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Polaris Missile Cruiser Mark 1");
			item.AddDescriptionLine("Fabricated by: Repulse shipyards.");
			item.AddDescriptionLine("The missile cruiser is a long range missile firing platform employed by numerous governments.");
			item.AddDescriptionLine("It cannot handle much punishment, but can deal massive damage if not dealt with properly.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "space_cruiser_4";
			ship.AddMissileTubes(4, 25.0f);

			ship.Hull = 40;
			ship.FrontShields = 50;
			ship.RearShields = 50;

			ship.SublightSpeed = 45; ship.RotatonSpeed = 3; ship.SublightAcceleration = 10;
            ship.FTLType = Ship.FTLTypes.None;

			ship.AddWeaponStores(MissileWeaponTypes.Homing, 50);

			ship.DefaultBehavor = "fighter_AI";
			ship.SetScienceItemFields(item);
		}

        void SetupKarnackCruiserMk1(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Karnack Cruiser Mark I");
			item.AddDescriptionLine("Fabricated by: Repulse shipyards.");
			item.AddDescriptionLine("Due to it's versatility, this ship has found wide adaptation in numerous governments.");
			item.AddDescriptionLine("Most vessels have been extensively retrofitted to suit their desired combat doctrines.");
			item.AddDescriptionLine("Because it's an older model, most factions have been selling stripped versions.");
			item.AddDescriptionLine("This practice has made the class a favorite with smugglers and other mercenary groups.");
			item.AddDescriptionLine("Many have used the ship's adaptable nature to facilitate re-fits incorporate illegal weaponry.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "space_frigate_6";
			ship.NewBeamWeapon(75, -15, 1000.0f, 6.0f, 6);
			ship.NewBeamWeapon(75, 15, 1000.0f, 6.0f, 6);

			ship.Hull = 70;
			ship.FrontShields = 40;
			ship.RearShields = 40;

			ship.SublightSpeed = 60; ship.RotatonSpeed = 6; ship.SublightAcceleration = 10;
            ship.FTLType = Ship.FTLTypes.None;

			ship.DefaultBehavor = "cruiser_AI";
			ship.SetScienceItemFields(item);
		}

        void SetupKarnackCruiserMk2(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Karnack Cruiser Mark II");
			item.AddDescriptionLine("Fabricated by: Repulse shipyards.");
			item.AddDescriptionLine("The successor to the wildly successfullyMark I cruiser. ");
			item.AddDescriptionLine("This ship has several notable improvements over the original model, including hull reinforcements, improved weaponry and increased performance.");
			item.AddDescriptionLine("These improvements were mainly requested by customers once they realized that their old surplus mark I ships were being used against them by pirates and smugglers.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "space_frigate_6";
			ship.NewBeamWeapon(90, -15, 1000.0f, 6.0f, 8);
			ship.NewBeamWeapon(90, 15, 1000.0f, 6.0f, 8);
			ship.AddMissileTubes(1, 15.0f);

			ship.AddWeaponStores(MissileWeaponTypes.Homing, 5);
			ship.AddStores("Trade Goods", 25);

			ship.Hull = 80;
			ship.FrontShields = 60;
			ship.RearShields = 60;

			ship.SublightSpeed = 65; ship.RotatonSpeed = 8; ship.SublightAcceleration = 12;
            ship.FTLType = Ship.FTLTypes.None;

			ship.DefaultBehavor = "cruiser_AI";
			ship.SetScienceItemFields(item);
		}

        void SetupSmallFighter(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Snub Fighter");
			item.AddDescriptionLine("Fabricated by: Various.");
			item.AddDescriptionLine("A small fighter class ship, generally attached to a larger ship or station.");
			item.AddDescriptionLine("Lightweight and poorly armored, these small ships rely on maneuverability to deliver weapon payloads to strategic locations on larger ships.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "small_fighter_1";
			ship.NewBeamWeapon(60, -0, 1000.0f, 4.0f, 4);

			ship.Hull = 30;
			ship.FrontShields = 30;
			ship.RearShields = 30;

			ship.SublightSpeed = 120; ship.RotatonSpeed = 30; ship.SublightAcceleration = 25;
            ship.FTLType = Ship.FTLTypes.None;

			ship.DefaultBehavor = "cruiser_AI";
			ship.SetScienceItemFields(item);
		}

        void SetupTug(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Tug");
			item.AddDescriptionLine("Fabricated by: Various.");
			item.AddDescriptionLine("A small cargo/utility ship used for various means of commerce");
			item.AddDescriptionLine("Slow and unarmored these commercial vehicles can be found anywhere there is space-based infrastructure.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "space_tug";
			ship.Hull = 50;
			ship.FrontShields = 20;
			ship.RearShields = 20;

			ship.SublightSpeed = 90; ship.RotatonSpeed = 6; ship.SublightAcceleration = 10;

            ship.FTLType = Ship.FTLTypes.None;

			ship.AddWeaponStores(MissileWeaponTypes.Homing, 5);
			ship.AddWeaponStores(MissileWeaponTypes.Nuke, 1);
			ship.AddWeaponStores(MissileWeaponTypes.Mine, 3);
			ship.AddWeaponStores(MissileWeaponTypes.EMP, 2);
			ship.AddStores("Trade Goods", 100);
			ship.SetScienceItemFields(item);
		}

        void SetupPlayerCruiser(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Cruiser");
			item.AddDescriptionLine("Fabricated by: Various Faction Navies");
			item.AddDescriptionLine("The cruiser is a long range patrol ship employed by numerous governments.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "battleship_destroyer_5_upgraded";
			ship.NewBeamWeapon(90, -15, 1000.0f, 6.0f, 10);
			ship.NewBeamWeapon(90, 15, 1000.0f, 6.0f, 10);
			ship.AddMissileTubes(2, 8.0f);

			ship.Hull = 200;
			ship.FrontShields = 80;
			ship.RearShields = 80;

			ship.SublightSpeed = 90; ship.RotatonSpeed = 10; ship.SublightAcceleration = 20;

            ship.FTLType = Ship.FTLTypes.JumpDrive;
			ship.FTLLimit = 50;
			ship.CanCloak = false;

			ship.AddWeaponStores(MissileWeaponTypes.Homing, 12);
			ship.AddWeaponStores(MissileWeaponTypes.Nuke, 4);
			ship.AddWeaponStores(MissileWeaponTypes.Mine, 8);
			ship.AddWeaponStores(MissileWeaponTypes.EMP, 6);

            ship.NewRoomSystem(new Vector2F(1, 0), new Vector2F(2, 1), Ship.SystemTypes.Maneuver);
            ship.NewRoomSystem(new Vector2F(1, 1), new Vector2F(2, 1), Ship.SystemTypes.BeamWeapons);
			ship.NewRoom(new Vector2F(2, 2), new Vector2F(2, 1));

            ship.NewRoomSystem(new Vector2F(0, 3), new Vector2F(1, 2), Ship.SystemTypes.RearShield);
            ship.NewRoomSystem(new Vector2F(1, 3), new Vector2F(2, 2), Ship.SystemTypes.Reactor);
            ship.NewRoomSystem(new Vector2F(3, 3), new Vector2F(2, 2), Ship.SystemTypes.WarpDrive);
            ship.NewRoomSystem(new Vector2F(5, 3), new Vector2F(1, 2), Ship.SystemTypes.JumpDrive);
			ship.NewRoom(new Vector2F(6, 3), new Vector2F(2, 1));	   
			ship.NewRoom(new Vector2F(6, 4), new Vector2F(2, 1));
            ship.NewRoomSystem(new Vector2F(8, 3), new Vector2F(1, 2), Ship.SystemTypes.FrontShield);


			ship.NewRoom(new Vector2F(2, 5), new Vector2F(2, 1));
            ship.NewRoomSystem(new Vector2F(1, 6), new Vector2F(2, 1), Ship.SystemTypes.MissileSystem);
            ship.NewRoomSystem(new Vector2F(1, 7), new Vector2F(2, 1), Ship.SystemTypes.Impulse);

			ship.NewDoor(new Vector2F(1, 1), true);
			ship.NewDoor(new Vector2F(2, 2), true);
			ship.NewDoor(new Vector2F(3, 3), true);
			ship.NewDoor(new Vector2F(1, 3), false);
			ship.NewDoor(new Vector2F(3, 4), false);
			ship.NewDoor(new Vector2F(3, 5), true);
			ship.NewDoor(new Vector2F(2, 6), true);
			ship.NewDoor(new Vector2F(1, 7), true);
			ship.NewDoor(new Vector2F(5, 3), false);
			ship.NewDoor(new Vector2F(6, 3), false);
			ship.NewDoor(new Vector2F(6, 4), false);
			ship.NewDoor(new Vector2F(8, 3), false);
			ship.NewDoor(new Vector2F(8, 4), false);

			ship.SetScienceItemFields(item);
		}

        void SetupPlayerMissileCruiser(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Missile Cruiser");
			item.AddDescriptionLine("Fabricated by: Various Faction Navies");
			item.AddDescriptionLine("The missile cruiser is a patrol ship employed by numerous governments.");
			item.AddDescriptionLine("The ships are generally outfitted with a larger compliment of missile weapons for long range operations");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "space_cruiser_4";
			ship.AddMissileTubes(4, 8.0f);

			ship.Hull = 200;
			ship.FrontShields = 110;
			ship.RearShields = 70;

			ship.SublightSpeed = 60; ship.RotatonSpeed = 8; ship.SublightAcceleration = 15;

            ship.FTLType = UseWarp ? Ship.FTLTypes.WarpDrive : Ship.FTLTypes.JumpDrive;
			ship.FTLLimit = 800;
			ship.CanCloak = false;

			ship.AddWeaponStores(MissileWeaponTypes.Homing, 30);
			ship.AddWeaponStores(MissileWeaponTypes.Nuke, 8);
			ship.AddWeaponStores(MissileWeaponTypes.Mine, 12);
			ship.AddWeaponStores(MissileWeaponTypes.EMP, 10);

            ship.NewRoomSystem(new Vector2F(1, 0), new Vector2F(2, 1), Ship.SystemTypes.Maneuver);
            ship.NewRoomSystem(new Vector2F(1, 1), new Vector2F(2, 1), Ship.SystemTypes.BeamWeapons);
			ship.NewRoom(new Vector2F(2, 2), new Vector2F(2, 1));

            ship.NewRoomSystem(new Vector2F(0, 3), new Vector2F(1, 2), Ship.SystemTypes.RearShield);
            ship.NewRoomSystem(new Vector2F(1, 3), new Vector2F(2, 2), Ship.SystemTypes.Reactor);
            ship.NewRoomSystem(new Vector2F(3, 3), new Vector2F(2, 2), Ship.SystemTypes.WarpDrive);
            ship.NewRoomSystem(new Vector2F(5, 3), new Vector2F(1, 2), Ship.SystemTypes.JumpDrive);
			ship.NewRoom(new Vector2F(6, 3), new Vector2F(2, 1));
			ship.NewRoom(new Vector2F(6, 4), new Vector2F(2, 1));
            ship.NewRoomSystem(new Vector2F(8, 3), new Vector2F(1, 2), Ship.SystemTypes.FrontShield);


			ship.NewRoom(new Vector2F(2, 5), new Vector2F(2, 1));
            ship.NewRoomSystem(new Vector2F(1, 6), new Vector2F(2, 1), Ship.SystemTypes.MissileSystem);
            ship.NewRoomSystem(new Vector2F(1, 7), new Vector2F(2, 1), Ship.SystemTypes.Impulse);

			ship.NewDoor(new Vector2F(1, 1), true);
			ship.NewDoor(new Vector2F(2, 2), true);
			ship.NewDoor(new Vector2F(3, 3), true);
			ship.NewDoor(new Vector2F(1, 3), false);
			ship.NewDoor(new Vector2F(3, 4), false);
			ship.NewDoor(new Vector2F(3, 5), true);
			ship.NewDoor(new Vector2F(2, 6), true);
			ship.NewDoor(new Vector2F(1, 7), true);
			ship.NewDoor(new Vector2F(5, 3), false);
			ship.NewDoor(new Vector2F(6, 3), false);
			ship.NewDoor(new Vector2F(6, 4), false);
			ship.NewDoor(new Vector2F(8, 3), false);
			ship.NewDoor(new Vector2F(8, 4), false);

			ship.SetScienceItemFields(item);
		}

        void SetupPlayerFighter(Ship.ShipTemplate ship, Campaign campaign)
		{
			var item = campaign.ScienceDB.New("Ships", "Snub Fighter");
			item.AddDescriptionLine("Fabricated by: Various.");
			item.AddDescriptionLine("A small fighter class ship, generally attached to a larger ship or station.");
			item.AddDescriptionLine("Lightweight and poorly armored, these small ships rely on maneuverability to deliver weapon payloads to strategic locations on larger ships.");
			item.Known = true;

			ship.ScienceDBID = item.ID;

			ship.Model = "small_fighter_1";
			ship.NewBeamWeapon(40, -10, 1000.0f, 6.0f, 8);
			ship.NewBeamWeapon(40, 10, 1000.0f, 6.0f, 8);
			ship.AddMissileTubes(1, 10.0f);

			ship.Hull = 60;
			ship.FrontShields = 40;
			ship.RearShields = 40;

			ship.SublightSpeed = 110; ship.RotatonSpeed = 20; ship.SublightAcceleration = 40;

            ship.FTLType = Ship.FTLTypes.None;
			ship.FTLLimit = 0;
			ship.CanCloak = false;

			ship.AddWeaponStores(MissileWeaponTypes.Homing, 4);

            ship.NewRoomSystem(new Vector2F(3, 0), new Vector2F(1, 1), Ship.SystemTypes.Maneuver);
            ship.NewRoomSystem(new Vector2F(1, 0), new Vector2F(2, 1), Ship.SystemTypes.BeamWeapons);

            ship.NewRoomSystem(new Vector2F(0, 1), new Vector2F(1, 2), Ship.SystemTypes.RearShield);
            ship.NewRoomSystem(new Vector2F(1, 1), new Vector2F(2, 2), Ship.SystemTypes.Reactor);
            ship.NewRoomSystem(new Vector2F(3, 1), new Vector2F(2, 1), Ship.SystemTypes.WarpDrive);
            ship.NewRoomSystem(new Vector2F(3, 2), new Vector2F(2, 1), Ship.SystemTypes.JumpDrive);
            ship.NewRoomSystem(new Vector2F(5, 1), new Vector2F(1, 2), Ship.SystemTypes.FrontShield);

            ship.NewRoomSystem(new Vector2F(1, 3), new Vector2F(2, 1), Ship.SystemTypes.MissileSystem);
            ship.NewRoomSystem(new Vector2F(3, 3), new Vector2F(1, 1), Ship.SystemTypes.Impulse);

			ship.NewDoor(new Vector2F(2, 1), true);
			ship.NewDoor(new Vector2F(3, 1), true);
			ship.NewDoor(new Vector2F(1, 1), false);
			ship.NewDoor(new Vector2F(3, 1), false);
			ship.NewDoor(new Vector2F(3, 2), false);
			ship.NewDoor(new Vector2F(3, 3), true);
			ship.NewDoor(new Vector2F(2, 3), true);
			ship.NewDoor(new Vector2F(5, 1), false);
			ship.NewDoor(new Vector2F(5, 2), false);

			ship.SetScienceItemFields(item);
		}
    }
}