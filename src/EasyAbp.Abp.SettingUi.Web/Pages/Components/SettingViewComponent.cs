using System.Linq;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Settings;
using Volo.Abp.Threading;

namespace EasyAbp.Abp.SettingUi.Web.Pages.Components
{
    [Widget(StyleFiles = new[] { "/Pages/Components/Default.css" })]
    public class SettingViewComponent : AbpViewComponent
    {
        private readonly IStringLocalizerFactory _factory;
        private readonly ISettingProvider _settingProvider;

        public SettingViewComponent(IStringLocalizerFactory factory, ISettingProvider settingProvider)
        {
            _factory = factory;
            _settingProvider = settingProvider;
        }

        public IViewComponentResult Invoke(SettingGroup parameter)
        {
            var settingInfos = parameter.SettingDefinitions.Select((sd, index) => new SettingHtmlInfo(
                sd,
                sd.DisplayName.Localize(_factory),
                sd.Description.Localize(_factory),
                AsyncHelper.RunSync(() => _settingProvider.GetOrNullAsync(sd.Name))
                ));
            return View("~/Pages/Components/Default.cshtml", settingInfos);
        }
    }

    public class SettingHtmlInfo
    {
        public string Name { get; }
        public LocalizedString DisplayName { get; }
        public LocalizedString Description { get; }
        public string Value { get; }
        public string Group1 { get; }
        public string Group2 { get; }
        public string FormName { get; }
        public string Type { get; }
        public SettingDefinition SettingDefinition { get; }

        public SettingHtmlInfo(SettingDefinition settingDefinition,
            LocalizedString displayName, LocalizedString description,
            string value)
        {
            Name = settingDefinition.Name;
            DisplayName = displayName;
            Description = description;
            Value = value;
            Group1 = (string)settingDefinition.Properties[SettingUiConst.Group1];
            Group2 = (string)settingDefinition.Properties[SettingUiConst.Group2];
            FormName = SettingUiConst.FormNamePrefix + Name.DotToUnderscore();
            Type = (string)settingDefinition.Properties[SettingUiConst.Type];
            SettingDefinition = settingDefinition;
        }
    }
}