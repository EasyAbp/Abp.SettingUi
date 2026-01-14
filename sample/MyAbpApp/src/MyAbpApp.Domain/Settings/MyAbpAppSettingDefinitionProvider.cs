using System;
using EasyAbp.Abp.SettingUi;
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
                new SettingDefinition("Connection.Protocol"),

                // Date & DateTime
                new SettingDefinition(
                    "DateTimeTest.Date",
                    new DateTime(2000, 1, 1).ToString("O"),
                    L("DisplayName:DateTimeTest.Date"),
                    L("Description:DateTimeTest.Date")),
                new SettingDefinition(
                    "DateTimeTest.DateTime",
                    new DateTime(new DateOnly(2000, 1, 1), new TimeOnly(8, 10, 20)).ToString("O"),
                    L("DisplayName:DateTimeTest.DateTime"),
                    L("Description:DateTimeTest.DateTime"))
            );

            context.Add(
                new SettingDefinition(
                        MyAbpAppSettings.SettingExample.ASettings.Setting1,
                        "setting 1 value")
                    .WithProperty(SettingUiConst.Group1, MyAbpAppSettings.SettingExample.Default)
                    .WithProperty(SettingUiConst.Group2, MyAbpAppSettings.SettingExample.ASettings.GroupName),
                new SettingDefinition(
                        MyAbpAppSettings.SettingExample.ASettings.Setting2,
                        "500")
                    .WithProperty(SettingUiConst.Group1, MyAbpAppSettings.SettingExample.Default)
                    .WithProperty(SettingUiConst.Group2, MyAbpAppSettings.SettingExample.ASettings.GroupName)
                    .WithProperty(SettingUiConst.Type, SettingUiConst.SettingTypes.Number),
                new SettingDefinition(
                        MyAbpAppSettings.SettingExample.ASettings.Setting3,
                        "true")
                    .WithProperty(SettingUiConst.Group1, MyAbpAppSettings.SettingExample.Default)
                    .WithProperty(SettingUiConst.Group2, MyAbpAppSettings.SettingExample.ASettings.GroupName)
                    .WithProperty(SettingUiConst.Type, SettingUiConst.SettingTypes.Checkbox),
                new SettingDefinition(
                        MyAbpAppSettings.SettingExample.BSettings.Setting1,
                        "setting 1 value")
                    .WithProperty(SettingUiConst.Group1, MyAbpAppSettings.SettingExample.Default)
                    .WithProperty(SettingUiConst.Group2, MyAbpAppSettings.SettingExample.BSettings.GroupName)
            );
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MyAbpAppResource>(name);
        }
    }
}