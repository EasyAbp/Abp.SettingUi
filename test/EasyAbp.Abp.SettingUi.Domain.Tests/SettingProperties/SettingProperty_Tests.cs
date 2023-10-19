using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Shouldly;
using Volo.Abp.Json;
using Volo.Abp.Settings;
using Volo.Abp.VirtualFileSystem;
using Xunit;
using Xunit.Abstractions;

namespace EasyAbp.Abp.SettingUi.SettingProperties
{
    public class SettingProperty_Tests : SettingUiDomainTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly ISettingDefinitionManager _settingDefinitionManager;

        public SettingProperty_Tests(ITestOutputHelper output)
        {
            _output = output;
            _settingDefinitionManager = GetRequiredService<ISettingDefinitionManager>();
        }

        [Fact]
        public async Task BuiltIn_Setting_Properties_Should_Be_Defined()
        {
            // Arrange
            var settingDefinitions = await _settingDefinitionManager.GetAllAsync();
            var fileInfo = GetRequiredService<IVirtualFileProvider>().GetFileInfo("/SettingProperties/AbpSettingUiSettingProperties.json");
            var jsonSerializer = GetRequiredService<IJsonSerializer>();

            // Act
            var propertyDict = jsonSerializer.Deserialize<IDictionary<string, IDictionary<string, string>>>(fileInfo.ReadAsString());

            // Assert
            foreach (var settingDefinition in settingDefinitions)
            {
                _output.WriteLine($"Check property of the setting {settingDefinition.Name}");
                propertyDict.ShouldContainKey(settingDefinition.Name, $"Setting property not found, setting name: {settingDefinition.Name}");
            }
        }
    }
}