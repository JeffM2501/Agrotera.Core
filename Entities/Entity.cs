using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Types;

namespace Entities
{
    public class Entity
    {
        public int ID = 0;

        public string Name = string.Empty;

        public Vector3F Position = Vector3F.Zero;
        public Vector3F Velocity = Vector3F.Zero;
    }
}
