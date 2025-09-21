namespace MyAbpApp.Settings
{
	public static class MyAbpAppSettings
	{
		private const string Prefix = "MyAbpApp";

		//Add your own setting names here. Example:
		//public const string MySetting1 = Prefix + ".MySetting1";

		public static class SettingExample
		{
			public const string Default = "SettingExample";
			public static class ASettings
			{
				// important note: dont start with Default value
				public const string GroupName = "ASettings";

				public const string Setting1 = GroupName + ".Setting1";
				public const string Setting2 = GroupName + ".Setting2";
				public const string Setting3 = GroupName + ".Setting3";
			}

			public static class BSettings
			{
				// important note: dont start with Default value
				public const string GroupName = "BSettings";

				public const string Setting1 = GroupName + ".Setting1";
                public const string Setting2 = GroupName + ".Setting2";
            }
		}
	}
}