using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core.Types;
using Agrotera.Core.Entities.Systems;

namespace Agrotera.Core.Entities
{
    public class Ship : Vessel
    {
        public enum FTLTypes
        {
            None,
            WarpDrive,
            JumpDrive,
        };

        public enum SystemTypes
        {
            None,
            Reactor,
            BeamWeapons,
            MissileSystem,
            Maneuver,
            Impulse,
            WarpDrive,
            JumpDrive,
            FrontShield,
            RearShield,
        }

        public class RoomDescriptor
        {
            public int ID = -1;
            public string Name = string.Empty;
            public SystemTypes ImbededSystem = SystemTypes.None;
            public Vector2F Position = Vector2F.Zero;
            public Vector2F Size = Vector2F.Zero;
        }

        public class DoorDescriptor
        {
            public int ID = -1;
            public Vector2F Position = Vector2F.Zero;
            public bool Horizontal = false;
        }

        public class ShipTemplate : VesselTemplate
        {
            public ShipTemplate()
                : base()
            {
                EntityClass = EntityClasses.Ship;
            }

            public double FrontShields = 0;
            public double RearShields = 0;

            public double SublightSpeed = 0;
            public double RotatonSpeed = 0;
            public double SublightAcceleration = 0;

            public FTLTypes FTLType = FTLTypes.None;

            public double FTLLimit = 0;

            public bool CanCloak = false;

            public List<RoomDescriptor> Rooms = new List<RoomDescriptor>();
            public List<DoorDescriptor> Doors = new List<DoorDescriptor>();

            public override void SetScienceItemFields(ScienceDatabaseItem item)
            {
                base.SetScienceItemFields(item);
                item.AddValue("Max Shield Strength", FrontShields.ToString() + "/" + RearShields.ToString());
                item.AddValue("Speed", SublightSpeed.ToString());
                item.AddValue("Maneuvering Speed", RotatonSpeed.ToString());
                item.AddValue("FTL Drive Type", FTLType.ToString());
                if (FTLType != FTLTypes.None)
                    item.AddValue("Max FTL Drive Factor", FTLLimit.ToString());
                if (CanCloak)
                    item.AddValue("Cloaking Device", "Detected");
            }

            public RoomDescriptor NewRoom(Vector2F postion, Vector2F size)
            {
                RoomDescriptor room = new RoomDescriptor();
                room.Position = new Vector2F(postion);
                room.Size = new Vector2F(size);
                room.ID = Rooms.Count;
                Rooms.Add(room);
                return room;
            }

            public RoomDescriptor NewRoomSystem(Vector2F postion, Vector2F size, SystemTypes system)
            {
                RoomDescriptor room = new RoomDescriptor();
                room.Position = new Vector2F(postion);
                room.Size = new Vector2F(size);
                room.ImbededSystem = system;
                room.ID = Rooms.Count;
                Rooms.Add(room);
                return room;
            }

            public DoorDescriptor NewDoor(Vector2F postion, bool horizontal)
            {
                DoorDescriptor door = new DoorDescriptor();
                door.Position = new Vector2F(postion);
                door.Horizontal = horizontal;
                door.ID = Doors.Count;
                Doors.Add(door);
                return door;
            }

            public bool IsCrewable()
            {
                if (Rooms.Count == 0)
                    return false;

                if (Rooms.Count == 1)
                    return true;

                return Doors.Count > 0;
            }

            public override Entity Create(Areas.Zone map)
            {
                return Setup(new Ship(map));
            }

            protected override Entity Setup(Entity ent)
            {
                Ship ship = base.Setup(ent) as Ship;
                if (ship == null)
                    return ent;

                ship.TemplateShip = this;

                ship.FrontShields = new Entities.Systems.ShieldSystem("Front Shields", 10, FrontShields);
                ship.AddSystem(ship.FrontShields);

                ship.RearShields = new Entities.Systems.ShieldSystem("Rear Shields", 10, RearShields);
                ship.AddSystem(ship.RearShields);

                return ship;
            }
        }

        public ShipTemplate TemplateShip = null;

        public double DesiredHeading = 0;
        public double DesiredSublightSpeed = 0;

        public double FTLPower = 0;
        public double FTLCurrent = 0;
        public double FTLParam = 0;

        public ShieldSystem FrontShields = null;
        public ShieldSystem RearShields = null;

        public VesselSystem SublightSystem = null;
        public ManuverSystem ManeuverSystem = null;
        public VesselSystem FTLSystem = null;

          public Ship(Areas.Zone map)
            : base(map)
        {
        }

        public override void Init()
        {
            base.Init();

            DesiredHeading = Rotation;

            if (TemplateShip.SublightAcceleration > 0 && TemplateShip.SublightSpeed > 0)
            {
                SublightSystem = new VesselSystem("Impulse", 3);
                AddSystem(SublightSystem);
            }

            if (TemplateShip.RotatonSpeed > 0)
            {
                ManeuverSystem = new ManuverSystem();
                AddSystem(ManeuverSystem);
            }

            switch (TemplateShip.FTLType)
            {
                case FTLTypes.JumpDrive:
                    FTLSystem = new VesselSystem("Jump Drive", 4);
                    break;

                case FTLTypes.WarpDrive:
                    FTLSystem = new VesselSystem("Warp Drive", 5);
                    break;
            }

            if (FTLSystem != null)
                AddSystem(FTLSystem);

            if (TemplateShip.CanCloak)
                AddSystem(new VesselSystem("Cloaking Device", 10));

            IsCrewable = TemplateShip.IsCrewable();
        }

        public virtual void InitRotation(double rot)
        {
            DesiredHeading = Rotation = rot;
        }

        public virtual void SetDesiredHeading(double heading)
        {
            DesiredHeading = Utilities.NormalizeAngle(heading);
        }

        public virtual void SetDesiredSublightSpeed(double speed)
        {
            DesiredSublightSpeed = speed;
        }

        public override List<NamedFloatValue> GetScienceScanValues(double scanProgress)
        {
            List<NamedFloatValue> values = base.GetScienceScanValues(scanProgress);

            if (scanProgress > 0.33 && FTLSystem != null)
            {
                if (TemplateShip.FTLType == FTLTypes.JumpDrive)
                    values.Add(new NamedFloatValue("Jump Drive Power", FTLSystem.ActualPowerLevel));
                else
                    values.Add(new NamedFloatValue("Warp Drive Power", FTLSystem.ActualPowerLevel));
            }

            if (scanProgress > 0.66 && SublightSystem != null || ManeuverSystem != null)
            {
                values.Add(new NamedFloatValue("Maneuver Power", SublightSystem.ActualPowerLevel));
                values.Add(new NamedFloatValue("Sublight Drive Power", ManeuverSystem.ActualPowerLevel));
            }

            if (scanProgress > 0.8 && FrontShields != null)
                values.Add(new NamedFloatValue("Front Shield Power", FrontShields.Status.Current));

            if (scanProgress > 0.82 && RearShields != null)
                values.Add(new NamedFloatValue("Front Shield Power", RearShields.Status.Current));

            if (scanProgress > 0.9 && FrontShields != null)
                values.Add(new NamedFloatValue("Max Front Shield Power", FrontShields.Status.Max));

            if (scanProgress > 0.93 && RearShields != null)
                values.Add(new NamedFloatValue("Max Front Shield Power", RearShields.Status.Max));

            return values;
        }

        protected virtual void ProcessHeadingChange(Tick tick)
        {
            if (DesiredHeading != Rotation)
            {
                double newHeading = Utilities.LinInterpValue(Rotation, DesiredHeading, ManeuverSystem.GetEffectiveTurnSpeed(), tick.Delta);
                Spin = (newHeading - Rotation) / tick.Delta;
            }
            else
                Spin = 0;
        }

        public override void Update(Tick tick)
        {
            base.Update(tick);
            ProcessHeadingChange(tick);
        }
    }
}
