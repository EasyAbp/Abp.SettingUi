using Volo.Abp.Settings;

namespace MyAbpApp.Settings
{
    public class MyAbpAppSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(MyAbpAppSettings.MySetting1));
        }
    }
}
