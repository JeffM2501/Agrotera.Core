using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core.Types;

using Agrotera.Core.Entities.Controllers;

namespace Agrotera.Core
{
    public class Tick
    {
        public double Now = 0;
        public double Delta = 0;

        public Tick(){}
        private Tick(double now) { Now = now; }

        private static Stopwatch Timer = new Stopwatch();
        private static double GetNowSeconds()
        {
            if (!Timer.IsRunning)
                Timer.Start();

            return Timer.ElapsedMilliseconds * 0.001;
        }

        private static Tick GloblalClock = new Tick(GetNowSeconds());

        private static bool UseFixedTimes = false;

        public static Tick UpdateGlobalClock()
        {
            double now = GetNowSeconds();

            if (UseFixedTimes)
            {
                GloblalClock.Delta = 0.01;
                GloblalClock.Now += GloblalClock.Delta;
            }
            else
            {
                GloblalClock.Delta = now - GloblalClock.Now;
                GloblalClock.Now = now;
            }

            return GloblalClock;
        }

        public static void ShutdownGlobalClock()
        {
            Timer.Stop();
        }
    }


    public class Entity
    {
        public class EntityTemplate
        {
            public string Name = string.Empty;
            public enum EntityClasses
            {
                Basic,
                Planet,
                Body,
                Station,
                Ship,
                Missile,
                Beam,
                Probe,
            }
            public EntityClasses EntityClass = EntityClasses.Basic;

            public UInt64 ScienceDBID = UInt64.MaxValue;
            public double Mass = 0;
            public double Radius = 0;

            public string ControllerName = "Default";

            public List<Tuple<string, string>> ControllerArguments = new List<Tuple<string, string>>();

            public virtual Entity Create(Areas.Zone map)
            {
                return Setup(new Entity(map));
            }

            protected virtual Entity Setup(Entity ent)
            {
                ent.ID = Entity.NewID();
                ent.Template = this;
                ent.SetController(ControllerCache.CreateController(this.ControllerName));
                if (ent.Controller == null)
                    ent.SetController(ControllerCache.CreateController("Default"));

                if (ent.Controller != null)
                {
                    foreach (var a in ControllerArguments)
                        ent.Controller.AddArgument(a.Item1, a.Item2);

                    ent.Controller.Init(ent);
                }

                return ent;
            }

            public virtual void SetScienceItemFields(ScienceDatabaseItem item)
            {
                item.AddValue("Mass", Mass.ToString());
            }
        }

        public class BasicData
        {
            public string ID = string.Empty;
            public Vector3F Postion = new Vector3F(0, 0, 0);
            public Vector3F Motion = new Vector3F(0, 0, 0);
        }

        public BasicData PositionData = new BasicData();

        public string ID
        {
            get { return PositionData.ID; }
            set { PositionData.ID = value; }
        }

        public EntityTemplate Template = null;
        public Vector3F Position
        {
            get { return PositionData.Postion; }
            set { PositionData.Postion = value; }
        }

        public Vector3F Vector
        {
            get { return PositionData.Motion; }
            set { PositionData.Motion = value; }
        }

        public double Rotation = (new Random().NextDouble() * Math.PI);
        public double Spin = 0;

        public object Model = null;

        private IEntityController EntController = null;

        public IEntityController Controller {  get { return EntController; } }

        public void SetController(IEntityController controller)
        {
            EntController = controller;
        }

        public static Dictionary<string, Entity> EntityCache = new Dictionary<string, Entity>();

        public class EntityEventArgs : EventArgs
        {
            public Entity AffectedEntity = null;
        }

        public EntityEventArgs Args = new EntityEventArgs();

        public readonly Areas.Zone Map = null;

        public Entity(Areas.Zone map)
        {
            Args.AffectedEntity = this;
            Map = map;
        }

        private static string MakeID()
        {
            return Utilities.RNG.Next().ToString() + Utilities.RNG.Next().ToString();
        }

        public static string NewID()
        {
            string id = MakeID();
            while (EntityCache.ContainsKey(id))
                id = MakeID();

            return id;
        }

        public virtual void Init()
        {
        }

        protected double LastTick = double.MinValue;

        public virtual void Update(Tick tick)
        {
            LastTick = tick.Now;
            if (Controller != null)
                Controller.Update(tick,this);
        }

        public virtual ScienceDatabaseItem.ItemGeneralizations GetScienceGeneralization()
        {
            if (Template == null)
                return ScienceDatabaseItem.ItemGeneralizations.Unknown;

            switch (Template.EntityClass)
            {
                case EntityTemplate.EntityClasses.Planet:
                    return ScienceDatabaseItem.ItemGeneralizations.Planet;

                case EntityTemplate.EntityClasses.Body:
                    return ScienceDatabaseItem.ItemGeneralizations.Body;

                case EntityTemplate.EntityClasses.Probe:
                    return ScienceDatabaseItem.ItemGeneralizations.Probe;

                case EntityTemplate.EntityClasses.Station:
                    return ScienceDatabaseItem.ItemGeneralizations.Station;

                case EntityTemplate.EntityClasses.Ship:
                    return ScienceDatabaseItem.ItemGeneralizations.Ship;

                case EntityTemplate.EntityClasses.Beam:
                case EntityTemplate.EntityClasses.Missile:
                    return ScienceDatabaseItem.ItemGeneralizations.Weapon;

                default:
                    return ScienceDatabaseItem.ItemGeneralizations.Unknown;
            }
        }

        public virtual List<NamedFloatValue> GetScienceScanValues(double scanProgress)
        {
            List<NamedFloatValue> values = new List<NamedFloatValue>();
            if (scanProgress > 0.25)
                values.Add(new NamedFloatValue("Heading", Rotation));
            if (scanProgress > 0.5)
                values.Add(new NamedFloatValue("Angular Velocity", Spin));

            return values;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
