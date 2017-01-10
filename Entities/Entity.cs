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

        public string VisualGraphics = string.Empty;

        public Location Position = Location.Zero;
        public Rotation Orientation = new Rotation();

        public Vector3D Velocity = Vector3D.Zero;
        public Rotation AngularVelocity = new Rotation();

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

        protected Dictionary<int, object> Paramaters = new Dictionary<int, object>();

        public int SetParam(string keyName, object val)
        {
            int key = keyName.GetHashCode();
            SetParam(key, val);
            return key;
        }

        public void SetParam(int key, object val)
        {
            if (Paramaters.ContainsKey(key))
                Paramaters[key] = val;
            else
                Paramaters.Add(key, val);
        }

        public object GetParam(int key)
        {
            if (Paramaters.ContainsKey(key))
                return Paramaters[key];
            return null;
        }

        public object GetParam(string keyName)
        {
            return GetParam(keyName.GetHashCode());
        }
    }
}
