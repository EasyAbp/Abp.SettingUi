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

        /// <summary>
        /// The name of the setting value provider the shown value comes from, using ABP's provider names:
        /// <c>D</c> (default value), <c>C</c> (configuration), <c>G</c> (global), <c>T</c> (tenant) or <c>U</c> (user).
        /// <c>null</c> when no provider has a value.
        /// </summary>
        public string ValueProviderName { get; set; }

        /// <summary>
        /// Whether the shown value is stored by the provider the page saves to (the current tenant, the host's global
        /// value when it is managed as global, or the current user), so a reset makes the setting inherit again.
        /// </summary>
        public bool IsValueSetHere { get; set; }
    }
}