﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core.Types;

namespace Agrotera.Core
{
    public class Tick
    {
        public double Now = 0;
        public double Delta = 0;
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

            public virtual Entity Create()
            {
                return Setup(new Entity());
            }

            protected virtual Entity Setup(Entity ent)
            {
                ent.ID = Entity.NewID();
                ent.Template = this;
                return ent;
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

        public static Dictionary<string, Entity> EntityCache = new Dictionary<string, Entity>();

        public class EntityEventArgs : EventArgs
        {
            public Entity AffectedEntity = null;
        }

        public EntityEventArgs Args = new EntityEventArgs();

        public Entity()
        {
            Args.AffectedEntity = this;
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

        public virtual void Update(Tick tick)
        {

        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
