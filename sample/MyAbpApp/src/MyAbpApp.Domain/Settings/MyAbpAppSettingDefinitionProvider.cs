using MyAbpApp.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace MyAbpApp.Settings
{
    public class MyAbpAppSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(
                // Set properties with "WithProperty" method
                new SettingDefinition(
                        "Connection.Ip",
                        "127.0.0.1",
                        L("DisplayName:Connection.Ip"),
                        L("Description:Connection.Ip"))
                    .WithProperty("Group1", "Server")
                    .WithProperty("Group2", "Connection"),

                // The properties are defined in the "MyAbpAppSettingProperties.json" file
                new SettingDefinition(
                    "Connection.Port",
                    8080.ToString(),
                    L("DisplayName:Connection.Port"),
                    L("Description:Connection.Port")
                ),
                
                // If a setting's DisplayName and Description are not defined, we can still localize it by using our own localization resources.
                // Just add the localization resources to the `SettingUiResource`, see `ConfigureLocalizationServices` method in `MyAbpAppWebModule`
                new SettingDefinition("Connection.Protocol")
            );
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MyAbpAppResource>(name);
        }
    }
}