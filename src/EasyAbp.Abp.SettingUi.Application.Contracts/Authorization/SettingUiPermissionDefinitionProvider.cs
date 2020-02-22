using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    public class SettingUiPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            //var moduleGroup = context.AddGroup(SettingUiPermissions.GroupName, L("Permission:SettingUi"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<SettingUiResource>(name);
        }
    }
}