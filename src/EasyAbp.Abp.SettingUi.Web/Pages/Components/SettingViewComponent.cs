using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Authorization.Permissions;

namespace EasyAbp.Abp.SettingUi.Web.Pages.Components
{
    [Widget(StyleFiles = new[] {"/Pages/Components/Default.css"})]
    public class SettingViewComponent : AbpViewComponent
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;

        public SettingViewComponent(IAuthorizationService authorizationService, IPermissionDefinitionManager permissionDefinitionManager)
        {
            _authorizationService = authorizationService;
            _permissionDefinitionManager = permissionDefinitionManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(SettingGroup parameter)
        {
            var definedSettingUiPermissions = _permissionDefinitionManager.GetPermissions().Where(p => p.Name.StartsWith(SettingUiPermissions.GroupName));

            var settingInfoGroups = parameter.SettingInfos
                .GroupBy(sd => sd.Properties[SettingUiConst.Group2].ToString())
                .Select(grp => new
                {
                    Permission = $"{SettingUiPermissions.GroupName}.{grp.FirstOrDefault()?.Properties[SettingUiConst.Group1]}.{grp.Key}",
                    SettingInfoList = grp.ToList()
                });

            var settingHtmlInfoList = new List<SettingHtmlInfo>();
            foreach (var settingInfoGroup in settingInfoGroups)
            {
                var definedSettingUiGroupPermission = definedSettingUiPermissions.FirstOrDefault(p => p.Name == settingInfoGroup.Permission);
                if (definedSettingUiGroupPermission == null || await _authorizationService.IsGrantedAsync(definedSettingUiGroupPermission.Name))
                {
                    settingHtmlInfoList.AddRange(
                        settingInfoGroup.SettingInfoList
                            .Where(si => string.IsNullOrEmpty(si.Permission))
                            .Select(si => new SettingHtmlInfo(si))
                        );
                    foreach (var settingInfo in settingInfoGroup.SettingInfoList.Where(si => !string.IsNullOrEmpty(si.Permission)))
                    {
                        if (await _authorizationService.IsGrantedAsync(settingInfo.Permission))
                        {
                            settingHtmlInfoList.Add(new SettingHtmlInfo(settingInfo));
                        }
                    }
                }
            }

            return View("~/Pages/Components/Default.cshtml", settingHtmlInfoList);
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

        public SettingHtmlInfo(SettingInfo settingInfo)
        {
            Name = settingInfo.Name;
            DisplayName = settingInfo.DisplayName;
            Description = settingInfo.Description;
            Value = settingInfo.Value;
            Group1 = (string) settingInfo.Properties[SettingUiConst.Group1];
            Group2 = (string) settingInfo.Properties[SettingUiConst.Group2];
            FormName = SettingUiConst.FormNamePrefix + Name.DotToUnderscore();
            Type = (string) settingInfo.Properties[SettingUiConst.Type];
            Properties = settingInfo.Properties;
        }
    }
}