using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    /// <summary>
    /// Group permissions for the settings of <see cref="SettingUi.SettingUiAppServiceAuthorization_Tests"/>,
    /// named by the documented convention: SettingUi.{Group1}.{Group2}.
    /// </summary>
    public class TestSettingUiPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public const string SecuredGroup = SettingUiPermissions.GroupName + ".Secured";
        public const string SecuredConnectionGroup = SecuredGroup + ".Connection";

        public override void Define(IPermissionDefinitionContext context)
        {
            var showSettingPage = context.GetPermissionOrNull(SettingUiPermissions.ShowSettingPage);
            var securedGroup = showSettingPage.AddChild(SecuredGroup, new FixedLocalizableString(SecuredGroup));
            securedGroup.AddChild(SecuredConnectionGroup, new FixedLocalizableString(SecuredConnectionGroup));
        }
    }
}
