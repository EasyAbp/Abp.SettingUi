using System.Collections.Generic;
using System.Linq;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Settings;

namespace EasyAbp.Abp.SettingUi.Web.Pages.Components.SettingUi
{
    [Widget(StyleFiles = new[] { "/Pages/Components/SettingUi/Default.css" })]
    public class SettingViewComponent : AbpViewComponent
    {
        public IViewComponentResult Invoke(SettingGroup parameter)
        {
            var settingInfos = parameter.SettingInfos.Select(si => new SettingHtmlInfo(si));
            return View("~/Pages/Components/SettingUi/Default.cshtml", settingInfos);
        }
    }

    public class SettingHtmlInfo
    {
        public string Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string Value { get; }
        public string Group1 { get; }
        public string Group2 { get; }
        public string FormName { get; }
        public string Type { get; }
        public Dictionary<string, object> Properties { get; }
        public bool IsEncrypted { get; }
        public bool HasValue { get; }
        public string ValueProviderName { get; }
        public bool IsValueSetHere { get; }

        public SettingHtmlInfo(SettingInfo settingInfo)
        {
            Name = settingInfo.Name;
            DisplayName = settingInfo.DisplayName;
            Description = settingInfo.Description;
            Value = settingInfo.Value;
            Group1 = (string)settingInfo.Properties[SettingUiConst.Group1];
            Group2 = (string)settingInfo.Properties[SettingUiConst.Group2];
            FormName = SettingUiConst.FormNamePrefix + Name.DotToUnderscore();
            Type = (string)settingInfo.Properties[SettingUiConst.Type];
            Properties = settingInfo.Properties;
            IsEncrypted = settingInfo.IsEncrypted;
            HasValue = settingInfo.HasValue;
            ValueProviderName = settingInfo.ValueProviderName;
            IsValueSetHere = settingInfo.IsValueSetHere;
        }

        /// <summary>
        /// The localization key suffix (<c>ValueSource:{suffix}</c>) of the badge that tells where the value comes
        /// from, or <c>null</c> for a provider SettingUi has no text for. Keep in sync with <c>Index.js</c>.
        /// </summary>
        public string GetValueSourceKey(bool isTenantSide)
        {
            if (IsValueSetHere)
            {
                return "SetHere";
            }

            switch (ValueProviderName)
            {
                case null:
                    return "NotSet";
                case DefaultValueSettingValueProvider.ProviderName:
                    return "Default";
                case ConfigurationSettingValueProvider.ProviderName:
                    return "Configuration";
                case GlobalSettingValueProvider.ProviderName:
                    return isTenantSide ? "InheritedFromHost" : "Global";
                case TenantSettingValueProvider.ProviderName:
                    return "Tenant";
                case UserSettingValueProvider.ProviderName:
                    return "User";
                default:
                    return null;
            }
        }
    }
}