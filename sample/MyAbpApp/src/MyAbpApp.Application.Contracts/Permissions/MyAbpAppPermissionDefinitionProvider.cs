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
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MyAbpAppResource>(name);
        }
    }
}
