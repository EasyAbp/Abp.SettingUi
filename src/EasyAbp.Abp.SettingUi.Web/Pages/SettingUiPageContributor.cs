using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.SettingUi;
using EasyAbp.Abp.SettingUi.Web.Pages.Components;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;

namespace EasyAbp.Abp.SettingUi.Web.Pages
{
    public class SettingUiPageContributor: ISettingPageContributor
    {
        public async Task ConfigureAsync(SettingPageCreationContext context)
        {
            var settingUiAppService = context.ServiceProvider.GetService<ISettingUiAppService>();

            foreach (var grp in await settingUiAppService.GroupSettingDefinitions())
            {
                context.Groups.Add(new SettingPageGroup(grp.GroupName, grp.GroupDisplayName, typeof(SettingViewComponent), grp));
            }
        }

        public async Task<bool> CheckPermissionsAsync(SettingPageCreationContext context)
        {
            return true;
            /*
            var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();
            if (await authorizationService.IsGrantedAsync(AbpSettingManagementMvcUIPermissions.Global) ||
                await authorizationService.IsGrantedAsync(AbpSettingManagementMvcUIPermissions.Tenant) ||
                await authorizationService.IsGrantedAsync(AbpSettingManagementMvcUIPermissions.User))
            {
                return true;
            }

            return false;
        */
        }
    }
}