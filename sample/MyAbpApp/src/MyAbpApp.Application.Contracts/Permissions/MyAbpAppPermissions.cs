using EasyAbp.Abp.SettingUi.Authorization;

namespace MyAbpApp.Permissions
{
	public static class MyAbpAppPermissions
	{
		public const string GroupName = "MyAbpApp";

		//Add your own permission names. Example:
		//public const string MyPermission1 = GroupName + ".MyPermission1";
	}

	public static class System
	{
		public const string Default = SettingUiPermissions.GroupName + ".System";
		public static class Password
		{
			public const string Group2 = Default + ".Password";
			public const string RequiredLength = Group2 + ".Abp.Identity.Password.RequiredLength";
			public const string RequiredUniqueChars = Group2 + ".Abp.Identity.Password.RequiredUniqueChars";
		}
	}

	public static class SettingExample
	{
		public const string Default = SettingUiPermissions.GroupName + ".SettingExample";

		public static class ASettings
		{
			public const string GroupName = Default + ".ASettings";

			public const string Setting1 = GroupName + ".Setting1";
		}

		public static class BSettings
		{
			public const string GroupName = Default + ".BSettings";
		}
	}
}