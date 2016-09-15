using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Setting
{
    public class Faction
    {
        public string Name = string.Empty;
        public string Description = string.Empty;
        public byte[] GMColor = new byte[] { byte.MinValue, byte.MinValue, byte.MinValue };

        public List<string> Enimies = new List<string>();

        public UInt64 ScienceDBEntry = UInt64.MaxValue;

        public enum Generalizations
        {
            Unclassified,
            PlayerCompatible,
            PlayerFriendly,
            Cooperative,
            Nutral,
            Hostile,
        }

        public Generalizations Generalization = Generalizations.Nutral;

        public Faction() { }

        public Faction(string name)
        {
            Name = name;
        }

        public void SetGMColor(byte r, byte g, byte b)
        {
            GMColor[0] = r;
            GMColor[1] = g;
            GMColor[2] = b;
        }

        public void SetDescription(string text)
        {
            Description = text;
        }

        public void AddDescriptionLine(string text)
        {
            Description += text + "\n";
        }

        public void AddEnimy(string name)
        {
            Enimies.Add(name);
        }

        public void AddEnimy(Faction enimy)
        {
            Enimies.Add(enimy.Name);
        }
    }
}
