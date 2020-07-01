using System.Collections.Generic;
using Volo.Abp.Settings;

namespace EasyAbp.Abp.SettingUi.Dto
{
    public class SettingGroup
    {
        public string GroupName { get; set; }
        public string GroupDisplayName { get; set; }
        public List<SettingInfo> SettingInfos { get; set; }
    }

    public class SettingInfo
    {
        public string Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string Value { get; }
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public SettingInfo(string name, string displayName, string description, string value)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
            Value = value;
        }

        public virtual SettingInfo WithProperty(string key, object value)
        {
            Properties[key] = value;
            return this;
        }
    }
}