using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    /// <summary>
    /// Permissions for the settings of <see cref="SettingUi.SettingUiAppServiceAuthorization_Tests"/>,
    /// <see cref="SettingUi.SettingUiSettingPermission_Tests"/>,
    /// <see cref="SettingUi.SettingUiLegacySettingPermission_Tests"/> and
    /// <see cref="SettingUi.SettingUiGroupPermission_Tests"/>. Most are named by the documented convention:
    /// SettingUi.{Group1}, SettingUi.{Group1}.{Group2} and SettingUi.{Group1}.{Group2}.{SettingName}; the Legacy ones
    /// only end with the setting name, which is all version 2.10 required.
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

        // Settings in Group1 "Legacy", Group2 "Old" (no group permissions defined)
        public const string LegacyOldSetting = SettingUiPermissions.GroupName + ".Legacy.Old.Setting";
        public const string LegacyBothSetting = SettingUiPermissions.GroupName + ".Legacy.Both.Setting";
        public const string ExactBothSetting = SettingUiPermissions.GroupName + ".Legacy.Old.Both.Setting";
        public const string LegacyTwinSetting = SettingUiPermissions.GroupName + ".Legacy.Twin.Setting";
        public const string OtherTwinSetting = SettingUiPermissions.GroupName + ".Other.Twin.Setting";

        // A Group2 permission with no Group1 permission, defined at the top level of the permission group
        public const string OrphanFilesGroup = SettingUiPermissions.GroupName + ".Orphan.Files";
        // A Group1 permission and a Group2 permission that is not its child
        public const string SplitGroup = SettingUiPermissions.GroupName + ".Split";
        public const string SplitFilesGroup = SplitGroup + ".Files";
        // The usual Group1 permission with its Group2 permission as a child
        public const string NestedGroup = SettingUiPermissions.GroupName + ".Nested";
        public const string NestedFilesGroup = NestedGroup + ".Files";

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

            foreach (var name in new[] { LegacyOldSetting, LegacyBothSetting, ExactBothSetting, LegacyTwinSetting, OtherTwinSetting })
            {
                showSettingPage.AddChild(name, new FixedLocalizableString(name));
            }

            context.GetGroup(SettingUiPermissions.GroupName)
                .AddPermission(OrphanFilesGroup, new FixedLocalizableString(OrphanFilesGroup));

            showSettingPage.AddChild(SplitGroup, new FixedLocalizableString(SplitGroup));
            showSettingPage.AddChild(SplitFilesGroup, new FixedLocalizableString(SplitFilesGroup));

            showSettingPage.AddChild(NestedGroup, new FixedLocalizableString(NestedGroup))
                .AddChild(NestedFilesGroup, new FixedLocalizableString(NestedFilesGroup));
        }
    }
}
