using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    /// <summary>
    /// Permissions for the settings of <see cref="SettingUi.SettingUiAppServiceAuthorization_Tests"/> and
    /// <see cref="SettingUi.SettingUiSettingPermission_Tests"/>, named by the documented convention:
    /// SettingUi.{Group1}, SettingUi.{Group1}.{Group2} and SettingUi.{Group1}.{Group2}.{SettingName}.
    /// </summary>
    public class TestSettingUiPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public const string SecuredGroup = SettingUiPermissions.GroupName + ".Secured";
        public const string SecuredConnectionGroup = SecuredGroup + ".Connection";

        public const string NetworkGroup = SettingUiPermissions.GroupName + ".Network";
        public const string NetworkAddressGroup = NetworkGroup + ".Address";
        // "Server.Ip" ends with "Ip": both names collide under a suffix match.
        public const string ServerIpSetting = NetworkAddressGroup + ".Server.Ip";
        public const string IpSetting = NetworkAddressGroup + ".Ip";

        public override void Define(IPermissionDefinitionContext context)
        {
            var showSettingPage = context.GetPermissionOrNull(SettingUiPermissions.ShowSettingPage);
            var securedGroup = showSettingPage.AddChild(SecuredGroup, new FixedLocalizableString(SecuredGroup));
            securedGroup.AddChild(SecuredConnectionGroup, new FixedLocalizableString(SecuredConnectionGroup));

            var networkGroup = showSettingPage.AddChild(NetworkGroup, new FixedLocalizableString(NetworkGroup));
            var networkAddressGroup = networkGroup.AddChild(NetworkAddressGroup, new FixedLocalizableString(NetworkAddressGroup));
            // Defined before IpSetting, so a suffix match for the "Ip" setting finds this one first.
            networkAddressGroup.AddChild(ServerIpSetting, new FixedLocalizableString(ServerIpSetting));
            networkAddressGroup.AddChild(IpSetting, new FixedLocalizableString(IpSetting));
        }
    }
}
