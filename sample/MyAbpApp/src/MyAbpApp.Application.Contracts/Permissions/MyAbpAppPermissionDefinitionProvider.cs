using EasyAbp.Abp.SettingUi.Authorization;
using MyAbpApp.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MyAbpApp.Permissions
{
	public class MyAbpAppPermissionDefinitionProvider : PermissionDefinitionProvider
	{
		public override void Define(IPermissionDefinitionContext context)
		{
			var myGroup = context.AddGroup(MyAbpAppPermissions.GroupName);

			//Define your own permissions here. Example:
			//myGroup.AddPermission(MyAbpAppPermissions.MyPermission1, L("Permission:MyPermission1"));

			var settingUiHostPermission = context.GetPermissionOrNull(SettingUiPermissions.Host.Default);
			var setttingUiUserPermission = context.GetPermissionOrNull(SettingUiPermissions.User.Default);
			var settingUiTenantPermission = context.GetPermissionOrNull(SettingUiPermissions.Tenant.Default);

			// group 1
			var systemGroup = settingUiTenantPermission.AddChild(System.Default, L("Permission:SettingUi.System"));
			// group 2
			var passwordGroup = systemGroup.AddChild(System.Password.Group2, L("Permission:SettingUi.System.Password"));
			passwordGroup.AddChild(
				System.Password.RequiredLength,
				L("Permission:SettingUi.System.Password.RequiredLength")
			);
			passwordGroup.AddChild(
				System.Password.RequiredUniqueChars,
				L("Permission:SettingUi.System.Password.RequiredUniqueChars")
			);

			// group 1
			var settingExampleGroup = settingUiTenantPermission.AddChild(SettingExample.Default, L("Permission:SettingUi.SettingExample"));
			// group 2
			var aSettings = settingExampleGroup.AddChild(SettingExample.ASettings.GroupName, L("Permission:SettingUi.SettingExample.ASettings"));
			aSettings.AddChild(
				SettingExample.ASettings.Setting1,
				L("Permission:SettingUi.SettingExample.ASettings.Setting1")
			);
			var bSettings = settingExampleGroup.AddChild(SettingExample.BSettings.GroupName, L("Permission:SettingUi.SettingExample.BSettings"));
		}

		private static LocalizableString L(string name)
		{
			return LocalizableString.Create<MyAbpAppResource>(name);
		}
	}
}
