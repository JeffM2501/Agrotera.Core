using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Setting
{
    public class ConfigurationData
    {
        public List<ConfigurationItem> Items = new List<ConfigurationItem>();

        public ConfigurationItem Find(string name)
        {
            return Items.Find(x => x.Name == name);
        }

        public bool Exists(string name)
        {
            return Find(name) != null;
        }

        public void Add(ConfigurationItem item)
        {
            var c = Find(item.Name);
            if (c != null)
                Items.Remove(c);
            Items.Add(item);
        }
    }

    public class ConfigurationItem
    {
        public string Name = string.Empty;
        public string Label = string.Empty;
        public string Description = string.Empty;
    }

    public class StringConfigruationItem : ConfigurationItem
    {
        public string Value = string.Empty;

        public StringConfigruationItem() { }

        public StringConfigruationItem(string name, string label, string desc, string value)
        {
            Name = name;
            Label = label;
            Description = desc;
            Value = value;
        }
    }

    public class ListConfigurationItem : ConfigurationItem
    {
        public List<string> Items = new List<string>();

        public bool AllowMultipleSelections = false;
        public List<int> SelectedItems = new List<int>();

        public ListConfigurationItem() { }

        public ListConfigurationItem(string name, string label, string desc, string[] values)
        {
            Name = name;
            Label = label;
            Description = desc;
            Items.AddRange(values);
        }
    }

    public class BooleanConfigruationItem : ConfigurationItem
    {
        public bool Value = false;

        public BooleanConfigruationItem() { }

        public BooleanConfigruationItem(string name, string label, string desc, bool value)
        {
            Name = name;
            Label = label;
            Description = desc;
            Value = value;
        }
    }

    public class NumberConfigurationItem : ConfigurationItem
    {
        public float Value = 0;
        public float MiniumValue = float.MaxValue;
        public float MaximumValue = float.MinValue;

        public bool UseWholeNumbers = false;

        public NumberConfigurationItem() { }

        public NumberConfigurationItem(string name, string label, string desc, float value)
        {
            Name = name;
            Label = label;
            Description = desc;
            Value = value;
        }

        public NumberConfigurationItem(string name, string label, string desc, float value, float min, float max)
        {
            Name = name;
            Label = label;
            Description = desc;
            Value = value;
            MiniumValue = min;
            MaximumValue = max;
        }

        public NumberConfigurationItem(string name, string label, string desc, float value, float min, float max, bool wholeNumbers)
        {
            Name = name;
            Label = label;
            Description = desc;
            Value = value;
            MiniumValue = min;
            MaximumValue = max;
            UseWholeNumbers = wholeNumbers;
        }
    }
}
