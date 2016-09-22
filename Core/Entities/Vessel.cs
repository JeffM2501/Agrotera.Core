using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core;
using Agrotera.Core.Types;
using Agrotera.Core.Entities.Systems;

namespace Agrotera.Core.Entities
{
    public class BeamWeaponDescriptor
    {
        public int ID = -1;
        public double Direction = 0;
        public double Arc = 90;
        public double MaxRange = 0;
        public double CycleTime = 0;
        public double Damage = 0;
    }

    public enum MissileWeaponTypes
    {
        None,
        Homing,
        Nuke,
        Mine,
        EMP,
        Count
    }

    public class MissileWeaponDescriptor
    {
        public int ID = -1;
        public double CycleTime = 0;
    }

    public class Vessel : Entity
    {
        public class VesselTemplate : EntityTemplate
        {
            public VesselTemplate()
                : base()
            {
            }

            public string Model = string.Empty;
            public string FactionName = string.Empty;

            public double Hull = 0;
            public double NominalSensorRange = 100;
            public double NominalSensorResolutionTime = 3;

            public double RactorPower = 25;

            public string DefaultBehavor = string.Empty;

            public List<BeamWeaponDescriptor> Beams = new List<BeamWeaponDescriptor>();

            public List<MissileWeaponDescriptor> MissileTubes = new List<MissileWeaponDescriptor>();

            public Dictionary<MissileWeaponTypes, int> WeaponStores = new Dictionary<MissileWeaponTypes, int>();
            public Dictionary<string, int> Stores = new Dictionary<string, int>();

            public override void SetScienceItemFields(ScienceDatabaseItem item)
            {
                base.SetScienceItemFields(item);
                item.AddValue("Max Hull Strength", Hull.ToString());
                item.AddValue("Nominal Sensor Range", NominalSensorRange.ToString());
                item.AddValue("Nominal Sensor Speed", NominalSensorResolutionTime.ToString());
                item.AddValue("Power Generation", RactorPower.ToString());
                item.AddValue("Beam Projectors", Beams.Count.ToString());
                item.AddValue("Missile Tubes", MissileTubes.Count.ToString());
            }

            public BeamWeaponDescriptor NewBeamWeapon(double dir, double arc, double range, double cycle, double damage)
            {
                BeamWeaponDescriptor beam = new BeamWeaponDescriptor();
                beam.ID = Beams.Count;
                beam.Direction = dir;
                beam.Arc = arc;
                beam.MaxRange = range;
                beam.CycleTime = cycle;
                beam.Damage = damage;
                Beams.Add(beam);
                return beam;
            }

            public int AddMissileTubes(int count, double cycle)
            {
                for (int i = 0; i < count; i++)
                {
                    MissileWeaponDescriptor tube = new MissileWeaponDescriptor();
                    tube.ID = MissileTubes.Count;
                    tube.CycleTime = cycle;
                    MissileTubes.Add(tube);
                }

                return MissileTubes.Count;
            }

            public int AddWeaponStores(MissileWeaponTypes torpedoType, int quantity)
            {

                if (WeaponStores.ContainsKey(torpedoType))
                    WeaponStores[torpedoType] += quantity;
                else
                    WeaponStores.Add(torpedoType, quantity);

                return WeaponStores[torpedoType];
            }

            public int AddStores(string name, int quantity)
            {
                string upperName = name.ToUpperInvariant();
                if (Stores.ContainsKey(upperName))
                    Stores[upperName] += quantity;
                else
                    Stores.Add(upperName, quantity);

                return Stores[upperName];
            }

            public override Entity Create(Areas.Zone map)
            {
                return Setup(new Vessel(map));
            }

            protected override Entity Setup(Entity ent)
            {
                Vessel vessel = base.Setup(ent) as Vessel;
                if (vessel == null)
                    return ent;

                vessel.TemplateVessel = this;
                vessel.Name = Name;

                vessel.Reactor.NominalPowerUseFactor = RactorPower * -1.0f; // reactors generate power by taking negative ammounts

                foreach (BeamWeaponDescriptor beams in Beams)
                    vessel.Beams.Add(new Vessel.BeamProjector(beams));

                foreach (MissileWeaponDescriptor tube in MissileTubes)
                    vessel.Tubes.Add(new Vessel.MissileTube(tube));

                vessel.MissileStores = new Dictionary<MissileWeaponTypes, int>(WeaponStores);
                vessel.CargoStores = new Dictionary<string, int>(Stores);

                vessel.Sensors.NominalRange = NominalSensorRange;
                vessel.Sensors.NominalResolutionTime = NominalSensorResolutionTime;

                return vessel;
            }
        }

        public VesselTemplate TemplateVessel = null;

        public string Name = string.Empty;


        public Vessel(Areas.Zone map)
            : base(map)
        {
        }

        public override string ToString()
        {
            if (Name == string.Empty)
                return base.ToString();

            return Name + " (" + ID + ")";
        }

        public class MapEntityLocationInfo
        {
            public Vector3F Postion = new Vector3F();
            public Vector3F Motion = new Vector3F();
            public double Facing = 0;
        }

        public class MapEntity
        {
            public string ID = string.Empty;

            [XmlIgnore]
            public Entity WorldEntity = null;

            public ScienceDatabaseItem.ItemGeneralizations Generalization = ScienceDatabaseItem.ItemGeneralizations.Unknown;
            public string SpecificGlyph = string.Empty;
            public MapEntityLocationInfo Location = new MapEntityLocationInfo();

            public double LastUpdate = double.MinValue;
            public UInt64 ScienceID = UInt64.MaxValue;

            public string IdentifiedFaction = string.Empty;

            public double ScanProgress = 0.0f;

            public List<NamedFloatValue> ScienceScanValues = new List<NamedFloatValue>();

            public class EventArgument : EventArgs
            {
                public MapEntity MapElement = null;
                public EventArgument(MapEntity entity)
                {
                    MapElement = entity;
                }
            }
        }


        public class BeamProjector
        {
            public int ID = 0;
            public BeamWeaponDescriptor Descriptor = null;

            public double CurrentPower = -1;
            public double CurrentChargeParam = 0;

            public BeamProjector() { }

            public BeamProjector(BeamWeaponDescriptor descriptor)
            {
                ID = descriptor.ID;
                Descriptor = descriptor;
                CurrentPower = 0;
                CurrentChargeParam = 0;
            }
        }

        public List<BeamProjector> Beams = new List<BeamProjector>();

        public class MissileTube
        {
            public int ID = 0;
            public MissileWeaponDescriptor Descriptor = null;

            public double CurrentPower = -1;
            public double LoadStatus = 0;
            public MissileWeaponTypes CurrentLoadout = MissileWeaponTypes.None;

            public MissileTube() { }
            public MissileTube(MissileWeaponDescriptor descriptor)
            {
                ID = descriptor.ID;
                Descriptor = descriptor;
            }
        }
        public List<MissileTube> Tubes = new List<MissileTube>();

        public Dictionary<MissileWeaponTypes, int> MissileStores = new Dictionary<MissileWeaponTypes, int>();
        public Dictionary<string, int> CargoStores = new Dictionary<string, int>();

        public bool IsCrewable = false;
        public string Owner = null;

        public double PowerBuffer = 0.0f;
        public double MaxPowerBuffer = 0.0f;
        public double Coolant = 1.0f;
        public double CurrentHull = 0;

        public ReactorSystem Reactor = new ReactorSystem();
        public SensorSystem Sensors = new SensorSystem();

        public VesselSystem BeamsSystem = null;
        public VesselSystem MissileSystem = null;

        public Dictionary<string, VesselSystem> Systems = new Dictionary<string, VesselSystem>();

        protected bool RedAlert = false;

        public bool RedAlertActive()
        {
            return RedAlert;
        }

        public event EventHandler RedAlertStarted = null;
        public event EventHandler RedAlertEnded = null;

        public event EventHandler ShieldsActivated = null;
        public event EventHandler ShieldsDeactivated = null;

        public virtual void SetRedAlert(bool state)
        {
            if (RedAlert == state)
                return;

            RedAlert = state;
            if (RedAlert && RedAlertStarted != null)
                RedAlertStarted(this, EventArgs.Empty);
            else if (!RedAlert && RedAlertEnded != null)
                RedAlertEnded(this, EventArgs.Empty);
        }

        public VesselSystem AddSystem(VesselSystem system)
        {
            system.ID = Systems.Count;
            Systems.Add(system.Name, system);
            return system;
        }

        public VesselSystem FindSystemByID(int index)
        {
            foreach (var v in Systems.Values)
            {
                if (index == v.ID)
                    return v;
            }
            return null;
        }

        public virtual List<int> GetShieldSystemIDs()
        {
            List<int> i = new List<int>();
            foreach (var v in Systems.Values)
            {
                if (v as ShieldSystem != null)
                    i.Add(v.ID);
            }
            return i;
        }

        public virtual bool ActivateShields(int id, bool state)
        {
            if (id < 0)
            {
                foreach (var v in Systems.Values)
                {
                    if (v as ShieldSystem != null)
                        (v as ShieldSystem).ActivateShields(state);
                }
            }
            else
            {
                ShieldSystem sys = FindSystemByID(id) as ShieldSystem;
                if (sys == null)
                    return false;

                sys.ActivateShields(state);

            }

            if (state && ShieldsActivated != null)
                ShieldsActivated(this, EventArgs.Empty);
            else if (!state && ShieldsDeactivated != null)
                ShieldsDeactivated(this, EventArgs.Empty);

            return true;
        }

        public override void Init()
        {
            base.Init();

            CurrentHull = TemplateVessel.Hull;

            AddSystem(Reactor);
            AddSystem(Sensors);

            if (Beams.Count > 0)
            {
                BeamsSystem = new VesselSystem("Beams", 3);
                AddSystem(BeamsSystem);
            }

            if (Tubes.Count > 0)
            {
                MissileSystem = new VesselSystem("Missiles", 1);
                AddSystem(MissileSystem);
            }

            MaxPowerBuffer += 100;// TODO , use this
        }

        public override void Update(Tick tick)
        {
        }

        public double Refuel(double fuel, double power)
        {
            PowerBuffer = (double)Math.Max(MaxPowerBuffer, PowerBuffer + power);
            return Reactor.AddFuel(fuel);
        }

        public override List<NamedFloatValue> GetScienceScanValues(double scanProgress)
        {
            List<NamedFloatValue> values = base.GetScienceScanValues(scanProgress);

            if (scanProgress > 0.1)
                values.Add(new NamedFloatValue("Power Level", PowerBuffer));

            if (scanProgress > 0.2)
                values.Add(new NamedFloatValue("Hull Integrity", CurrentHull / TemplateVessel.Hull));

            if (scanProgress > 0.3)
                values.Add(new NamedFloatValue("Max Power Level", MaxPowerBuffer));

            if (scanProgress > 0.4 && BeamsSystem != null)
                values.Add(new NamedFloatValue("Beam Power", BeamsSystem.ActualPowerLevel));

            if (scanProgress > 0.45 && BeamsSystem != null)
                values.Add(new NamedFloatValue("Beam Projectors", Beams.Count));

            if (scanProgress > 0.5 && MissileSystem != null)
                values.Add(new NamedFloatValue("Missile Power", MissileSystem.ActualPowerLevel));

            if (scanProgress > 0.55 && MissileSystem != null)
                values.Add(new NamedFloatValue("Missile Tubes", Tubes.Count));

            if (scanProgress > 0.6 && Reactor != null)
                values.Add(new NamedFloatValue("Reactor Heat", Reactor.HeatLevel));

            if (scanProgress > 0.7 && Reactor != null)
                values.Add(new NamedFloatValue("Reactor Damage", Reactor.Damage));


            return values;
        }

        public Dictionary<string, MapEntity> MappedItems = new Dictionary<string, MapEntity>();

        public event EventHandler<MapEntity.EventArgument> MapElementAdded = null;
        public event EventHandler<MapEntity.EventArgument> MapElementUpdated = null;

        public event EventHandler<MapEntity.EventArgument> ScanTargetChanged = null;

        public Dictionary<UInt64, ScienceDatabaseItem> ScienceDB = new Dictionary<UInt64, ScienceDatabaseItem>();

        public event EventHandler<ScienceDatabaseItem.EventArguments> DatabaseItemAdded = null;

        public MapEntity ActiveScanTarget = null;

        public double ActiveTargetRescanTime = 5;
        public double ScanUpdateGranularity = 0.1f;

        public ScienceDatabaseItem AddScienceItem(ScienceDatabaseItem item)
        {
            if (ScienceDB.ContainsKey(item.ID))
                return ScienceDB[item.ID];

            ScienceDB.Add(item.ID, item);
			DatabaseItemAdded?.Invoke(this, new ScienceDatabaseItem.EventArguments(item));

			return item;
        }

        public double GetEffectiveSensorRange()
        {
            if (Sensors == null)
                return 20;

            return Sensors.GetEffectiveRange();
        }

        protected MapEntity CreateMapEntity(Entity e)
        {
            MapEntity mapItem = new MapEntity();
            mapItem.ID = e.ID;
            mapItem.WorldEntity = e;
            mapItem.Generalization = ScienceDatabaseItem.ItemGeneralizations.Unknown;
            mapItem.ScanProgress = 0;
            mapItem.LastUpdate = LastTick;
            if (e.Template != null && ScienceDB.ContainsKey(e.Template.ScienceDBID))
            {
                var scienceItem = ScienceDB[e.Template.ScienceDBID];
                mapItem.ScienceID = scienceItem.ID;
                mapItem.Generalization = scienceItem.Generalization;
                if (scienceItem.Known)
                {
                    mapItem.ScanProgress = 1;
                    mapItem.ScienceScanValues = e.GetScienceScanValues(1);
                }
            }

            return mapItem;
        }

        public void SetScanTarget(string entityID)
        {
            if (MappedItems.ContainsKey(entityID))
                SetScanTarget(MappedItems[entityID]);
        }

        public MapEntity GetActiveScanTarget()
        {
            return ActiveScanTarget;
        }

        public void SetScanTarget(MapEntity entity)
        {
            if (entity == null)
                return;

            ActiveScanTarget = entity;
            if (ActiveScanTarget.ScanProgress < 1)
                ActiveScanTarget.LastUpdate = LastTick;

			ScanTargetChanged?.Invoke(this, new MapEntity.EventArgument(entity));
		}

        public void ClearScanTarget()
        {
            if (ActiveScanTarget == null)
                return;

            ActiveScanTarget = null;

			ScanTargetChanged?.Invoke(this, new MapEntity.EventArgument(null));
		}

		public void AddMappedItem(Entity e)
		{
			var me = CreateMapEntity(e);
			MappedItems.Add(e.ID, me);

			MapElementAdded?.Invoke(this, new MapEntity.EventArgument(me));
		}

		public void UpdateMapedItem(MapEntity ent)
		{
			MapElementUpdated?.Invoke(this, new MapEntity.EventArgument(ent));
		}
    }
}
