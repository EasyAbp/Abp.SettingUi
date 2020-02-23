using Volo.Abp.Settings;

namespace MyAbpApp.Settings
{
    public class MyAbpAppSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(
                // Set properties with "WithProperty" method
                new SettingDefinition("Connection.Ip", "127.0.0.1")
                    .WithProperty("Group1", "Server")
                    .WithProperty("Group2", "Connection"),

                // The properties are defined in the "MyAbpAppSettingProperties.json" file
                new SettingDefinition("Connection.Port", 8080.ToString()),
                
                new SettingDefinition("Connection.Protocol")
            );
        }
    }
}
