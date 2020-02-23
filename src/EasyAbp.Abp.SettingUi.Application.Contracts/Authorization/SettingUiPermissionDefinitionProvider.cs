using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.UI;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    public class SettingUiPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var moduleGroup = context.AddGroup(SettingUiPermissions.GroupName, L("Permission:SettingUi"));
            moduleGroup.AddPermission(SettingUiPermissions.Global, L("Permission:SettingUi.Global"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AbpUiModule>(name);
        }
    }
}