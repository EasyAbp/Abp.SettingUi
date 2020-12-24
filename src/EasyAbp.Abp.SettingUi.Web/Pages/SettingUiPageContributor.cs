using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using EasyAbp.Abp.SettingUi.Web.Pages.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;

namespace EasyAbp.Abp.SettingUi.Web.Pages
{
    public class SettingUiPageContributor : ISettingPageContributor
    {
        public async Task ConfigureAsync(SettingPageCreationContext context)
        {
            var settingUiAppService = context.ServiceProvider.GetService<ISettingUiAppService>();
            var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();
            var permissionDefinitionManager = context.ServiceProvider.GetRequiredService<IPermissionDefinitionManager>();
            var definedSettingUiPermissions = permissionDefinitionManager.GetPermissions().Where(p => p.Name.StartsWith(SettingUiPermissions.GroupName));
            foreach (var grp in await settingUiAppService.GroupSettingDefinitions())
            {
                var definedSettingUiGroupPermission = definedSettingUiPermissions.FirstOrDefault(p => p.Name == grp.Permission);
                if (definedSettingUiGroupPermission == null || await authorizationService.IsGrantedAsync(definedSettingUiGroupPermission.Name))
                {
                    context.Groups.Add(new SettingPageGroup(grp.GroupName, grp.GroupDisplayName, typeof(SettingViewComponent), grp));
                }
            }
        }

        public async Task<bool> CheckPermissionsAsync(SettingPageCreationContext context)
        {
            var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();
            return await authorizationService.IsGrantedAsync(SettingUiPermissions.ShowSettingPage);
        }
    }
}