using System;
using System.Collections.Generic;
using Volo.Abp.Data;
using Volo.Abp.Settings;

namespace EasyAbp.Abp.SettingUi.Dto
{
    [Serializable]
    public class SettingGroup
    {
        public string GroupName { get; set; }
        public string GroupDisplayName { get; set; }
        public List<SettingInfo> SettingInfos { get; set; }
        public string Permission { get; set; }
    }

    [Serializable]
    public class SettingInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public Dictionary<string, object> Properties { get; set; } 
        public string Permission { get; set; }
    }
}