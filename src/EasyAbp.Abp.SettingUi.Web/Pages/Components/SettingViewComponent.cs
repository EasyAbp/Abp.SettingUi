using System.Collections.Generic;
using System.Linq;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Settings;
using Volo.Abp.Threading;

namespace EasyAbp.Abp.SettingUi.Web.Pages.Components
{
    [Widget(StyleFiles = new[] {"/Pages/Components/Default.css"})]
    public class SettingViewComponent : AbpViewComponent
    {
        private readonly ISettingProvider _settingProvider;

        public SettingViewComponent(ISettingProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }

        public IViewComponentResult Invoke(SettingGroup parameter)
        {
            var settingInfos = parameter.SettingInfos.Select(CreateSettingHtmlInfo);
            return View("~/Pages/Components/Default.cshtml", settingInfos);
        }

        private SettingHtmlInfo CreateSettingHtmlInfo(SettingInfo settingInfo)
        {
            return new SettingHtmlInfo(settingInfo, AsyncHelper.RunSync(() => _settingProvider.GetOrNullAsync(settingInfo.Name)));
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

        public SettingHtmlInfo(SettingInfo settingInfo, string value)
        {
            Name = settingInfo.Name;
            DisplayName = settingInfo.DisplayName;
            Description = settingInfo.Description;
            Value = value;
            Group1 = (string) settingInfo.Properties[SettingUiConst.Group1];
            Group2 = (string) settingInfo.Properties[SettingUiConst.Group2];
            FormName = SettingUiConst.FormNamePrefix + Name.DotToUnderscore();
            Type = (string) settingInfo.Properties[SettingUiConst.Type];
            Properties = settingInfo.Properties;
        }
    }
}