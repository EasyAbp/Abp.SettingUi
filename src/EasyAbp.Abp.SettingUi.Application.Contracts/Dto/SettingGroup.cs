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

        /// <summary>
        /// The value the page shows. Always <c>null</c> for an encrypted setting (see <see cref="IsEncrypted"/>):
        /// load it on demand with <c>ISettingUiAppService.GetSettingValueAsync</c>.
        /// </summary>
        public string Value { get; set; }

        public Dictionary<string, object> Properties { get; set; } 
        public string Permission { get; set; }

        /// <summary>
        /// Whether the setting is encrypted. Its <see cref="Value"/> is not sent with the setting list.
        /// </summary>
        public bool IsEncrypted { get; set; }

        /// <summary>
        /// Whether the setting has a value the page shows (for an encrypted setting, whether one is set even though
        /// <see cref="Value"/> is not sent).
        /// </summary>
        public bool HasValue { get; set; }
    }
}