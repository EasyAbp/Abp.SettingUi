using System.Collections.Generic;
using Volo.Abp.Settings;

namespace EasyAbp.Abp.SettingUi.Dto
{
    public class SettingGroup
    {
        public string GroupName { get; set; }
        public string GroupDisplayName { get; set; }
        public IEnumerable<SettingDefinition> SettingDefinitions { get; set; }
    }
}