using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Types;

namespace Entities
{
    public interface IEntityContorller
    {
        void AddEntity(Entity ent);
        void UpdateEntity(Entity ent);
    }

    public class Entity
    {
        public int ID = 0;

        public string Name = string.Empty;

        public Vector3F Position = Vector3F.Zero;
        public Vector3F Velocity = Vector3F.Zero;

        public object Tag = string.Empty;
        protected IEntityContorller Controller = null;

        public event EventHandler Deleted = null;

        public void Delete()
        {
            if (Deleted != null)
                Deleted.Invoke(this, EventArgs.Empty);
        }

        public void SetController(IEntityContorller ctl)
        {
            Controller = ctl;
            ctl.AddEntity(this);
        }

        public void UpdateController()
        {
            if (Controller != null)
                Controller.UpdateEntity(this);
        }

        protected Dictionary<int, double> Paramaters = new Dictionary<int, double>();

        public int SetParam(string keyName, double val)
        {
            int key = keyName.GetHashCode();
            SetParam(key, val);
            return key;
        }

        public void SetParam(int key, double val)
        {
            if (Paramaters.ContainsKey(key))
                Paramaters[key] = val;
            else
                Paramaters.Add(key, val);
        }

        public double GetParam(int key)
        {
            if (Paramaters.ContainsKey(key))
                return Paramaters[key];
            return 0;
        }

        public double GetParam(string keyName)
        {
            return GetParam(keyName.GetHashCode());
        }
    }
}
