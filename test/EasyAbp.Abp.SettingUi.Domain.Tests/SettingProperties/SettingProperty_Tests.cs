using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Shouldly;
using Volo.Abp.Json;
using Volo.Abp.Settings;
using Volo.Abp.VirtualFileSystem;
using Xunit;

namespace EasyAbp.Abp.SettingUi.SettingProperties
{
    public class SettingProperty_Tests : SettingUiDomainTestBase
    {
        [Fact]
        public Task BuiltIn_Setting_Properties_Should_Be_Defined()
        {
            // Arrange
            var settingDefinitions = GetRequiredService<ISettingDefinitionManager>().GetAll();
            var fileInfo = GetRequiredService<IVirtualFileProvider>().GetFileInfo("/SettingProperties/AbpSettingUiSettingProperties.json");
            var jsonSerializer = GetRequiredService<IJsonSerializer>();

            // Act
            var propertyDict = jsonSerializer.Deserialize<IDictionary<string, IDictionary<string, string>>>(fileInfo.ReadAsString());

            // Assert
            foreach (var settingDefinition in settingDefinitions)
            {
                propertyDict.ShouldContainKey(settingDefinition.Name, $"Setting property not found, setting name: {settingDefinition.Name}");
            }


            return Task.CompletedTask;
        }
    }
}