using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agrotera.Core.Types;

namespace Agrotera.Core
{
    public class ScienceDatabaseItem
    {
        public UInt64 ID = UInt64.MinValue;

        public string Name = string.Empty;
        public string DisplayName = string.Empty;
        public string Category = string.Empty;

        public string LongDescription = string.Empty;

        public bool Known = false;

        public List<NamedStringValue> Values = new List<NamedStringValue>();

        public event EventHandler Dirtied = null;

        public override string ToString()
        {
            return DisplayName + " (" + ID.ToString() + ")";
        }

        public void AddValue(string name, string value)
        {
            NamedStringValue p = Values.Find(x => x.Name == name);
            if (p != null)
                Values.Remove(p);
            Values.Add(new NamedStringValue(name, value));
        }

        public void SetValue(string name, string value)
        {
            AddValue(name, value);
        }

        public string GetValue(string name)
        {
            NamedStringValue p = Values.Find(x => x.Name == name);
            if (p != null)
                return p.Value;
            return string.Empty;
        }

        public enum ItemGeneralizations
        {
            Unknown = 0,
            Planet,
            Body,
            Station,
            Ship,
            Weapon,
            Probe,
            Anomaly,
            Data,
        }

        public ItemGeneralizations Generalization = ItemGeneralizations.Unknown;

        public void SetDescription(string text)
        {
            LongDescription = text;
        }

        public void AddDescriptionLine(string text)
        {
            LongDescription += text + "\n";
        }

        public void Dirty()
        {
            if (Dirtied != null)
                Dirtied(this,EventArgs.Empty);
        }

        public class EventArguments : EventArgs
        {
            public ScienceDatabaseItem DatabaseItem = null;
            public EventArguments(ScienceDatabaseItem item)
            {
                DatabaseItem = item;
            }
        }
    }
}
