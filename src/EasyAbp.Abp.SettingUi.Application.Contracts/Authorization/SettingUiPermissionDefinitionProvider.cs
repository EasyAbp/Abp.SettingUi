using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.UI;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    public class SettingUiPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var moduleGroup = context.AddGroup(SettingUiPermissions.GroupName, L("Permission:SettingUi"));
            var showSettingPagePermission = moduleGroup.AddPermission(SettingUiPermissions.ShowSettingPage, L("Permission:SettingUi.ShowSettingPage"));
            showSettingPagePermission.AddChild(SettingUiPermissions.Host.Default, L("Permission:SettingUi.Host"), MultiTenancySides.Host);
            showSettingPagePermission.AddChild(SettingUiPermissions.Tenant.Default, L("Permission:SettingUi.Tenant"));
            showSettingPagePermission.AddChild(SettingUiPermissions.User.Default, L("Permission:SettingUi.User"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<SettingUiResource>(name);
        }
    }
}