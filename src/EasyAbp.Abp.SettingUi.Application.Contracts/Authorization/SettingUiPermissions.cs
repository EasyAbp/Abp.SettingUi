using Volo.Abp.Reflection;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    public class SettingUiPermissions
    {
        public const string GroupName = "SettingUi";
        public const string Tenant = GroupName + ".Tenant";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(SettingUiPermissions));
        }
    }
}