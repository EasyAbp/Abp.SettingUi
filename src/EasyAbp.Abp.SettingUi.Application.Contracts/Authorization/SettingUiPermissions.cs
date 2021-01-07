using Volo.Abp.Reflection;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    public class SettingUiPermissions
    {
        public const string GroupName = "SettingUi";
        public const string ShowSettingPage = GroupName + ".ShowSettingPage";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(SettingUiPermissions));
        }
    }
}