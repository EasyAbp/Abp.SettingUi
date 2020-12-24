using Volo.Abp.Reflection;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    public class SettingUiPermissions
    {
        public const string GroupName = "SettingUi";
        public const string ShowSettingPage = GroupName + ".ShowSettingPage";

        public static class Tenant
        {
            public const string Default = GroupName + ".Tenant";
        }

        public static class User
        {
            public const string Default = GroupName + ".User";
        }

        public static class Host
        {
            public const string Default = GroupName + ".Host";
        }

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(SettingUiPermissions));
        }
    }
}