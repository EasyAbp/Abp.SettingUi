using System.Linq;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Extensions;
using EasyAbp.Abp.SettingUi.Localization;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Localization;
using Volo.Abp.Settings;
using Volo.Abp.Threading;

namespace EasyAbp.Abp.SettingUi.Web.Pages.Components
{
    [Widget(StyleFiles = new[] {"/Pages/Components/Default.css"})]
    public class SettingViewComponent : AbpViewComponent
    {
        private readonly IStringLocalizerFactory _factory;
        private readonly ISettingProvider _settingProvider;
        private readonly IStringLocalizer<SettingUiResource> _localizer;

        public SettingViewComponent(IStringLocalizerFactory factory, ISettingProvider settingProvider, IStringLocalizer<SettingUiResource> localizer)
        {
            _factory = factory;
            _settingProvider = settingProvider;
            _localizer = localizer;
        }

        public IViewComponentResult Invoke(SettingGroup parameter)
        {
            var settingInfos = parameter.SettingDefinitions.Select(CreateSettingHtmlInfo);
            return View("~/Pages/Components/Default.cshtml", settingInfos);
        }

        private SettingHtmlInfo CreateSettingHtmlInfo(SettingDefinition settingDefinition)
        {
            LocalizedString displayName;
            if (settingDefinition.DisplayName is FixedLocalizableString fls && fls.Value == settingDefinition.Name)
            {
                displayName = _localizer[$"DisplayName:{settingDefinition.Name}"];
            }
            else
            {
                displayName = settingDefinition.DisplayName.Localize(_factory);
            } 
            var description = settingDefinition.Description == null ? _localizer[$"Description:{settingDefinition.Name}"] : settingDefinition.Description.Localize(_factory);
            return new SettingHtmlInfo(
                settingDefinition,
                displayName,
                description,
                AsyncHelper.RunSync(() => _settingProvider.GetOrNullAsync(settingDefinition.Name))
            );
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
            Group1 = (string) settingDefinition.Properties[SettingUiConst.Group1];
            Group2 = (string) settingDefinition.Properties[SettingUiConst.Group2];
            FormName = SettingUiConst.FormNamePrefix + Name.DotToUnderscore();
            Type = (string) settingDefinition.Properties[SettingUiConst.Type];
            SettingDefinition = settingDefinition;
        }
    }
}