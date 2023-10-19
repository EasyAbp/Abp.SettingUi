using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Xunit;
using Xunit.Abstractions;

namespace EasyAbp.Abp.SettingUi.SettingUi
{
    public class SettingDefinitionGroupAppService_Tests : SettingUiApplicationTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly ISettingUiAppService _service;
        private ISettingManager _settingManager;

        public SettingDefinitionGroupAppService_Tests(ITestOutputHelper output)
        {
            _output = output;
            _service = GetRequiredService<ISettingUiAppService>();
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            services.AddAlwaysAllowAuthorization();

            // Mock ISettingDefinitionManager
            var settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            var setting1 = new SettingDefinition("Test.Setting1", "1")
                .WithProperty(SettingUiConst.Group1, "TestGroup1")
                .WithProperty(SettingUiConst.Group2, "TestGroup2")
                .WithProperty(SettingUiConst.Type, "number");
            var setting2 = new SettingDefinition("Test.Setting2", "2");
            var setting3 = new SettingDefinition("Test.Setting3", "3");
            settingDefinitionManager.GetAllAsync().Returns(new List<SettingDefinition>
            {
                setting1,
                setting2,
                setting3
            });
            settingDefinitionManager.GetOrNullAsync("Test.Setting1").Returns(setting1);
            settingDefinitionManager.GetOrNullAsync("Test.Setting2").Returns(setting2);
            settingDefinitionManager.GetOrNullAsync("Test.Setting3").Returns(setting3);
            services.AddSingleton(settingDefinitionManager);

            // Mock ISettingManager
            _settingManager = Substitute.For<ISettingManager>();
            services.AddSingleton(_settingManager);
            
            // Mock ISettingProvider
            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.GetOrNullAsync("Test.Setting1").Returns(Task.FromResult("1"));
            settingProvider.GetOrNullAsync("Test.Setting2").Returns(Task.FromResult("2"));
            settingProvider.GetOrNullAsync("Test.Setting3").Returns(Task.FromResult("3"));
            services.AddSingleton(settingProvider);
        }

        [Fact]
        public async Task Settings_Should_Be_Grouped()
        {
            // Arrange
            // The TestSettingDefinitionsProvider should be executed by module system

            // Act
            var groups = await _service.GroupSettingDefinitionsAsync();

            // Assert
            var group = groups.Single(g => g.GroupName == "TestGroup1");
            group.GroupDisplayName.ShouldBe("TestGroup1");
            group.SettingInfos.Count().ShouldBe(2);

            // The property values of the Test.Setting1 are set with "WithProperty" method
            var setting1 = group.SettingInfos.Single(si => si.Name == "Test.Setting1");
            setting1.Properties[SettingUiConst.Group1].ShouldBe("TestGroup1");
            setting1.Properties[SettingUiConst.Group2].ShouldBe("TestGroup2");
            setting1.Properties[SettingUiConst.Type].ShouldBe("number");

            // The property values of the Test.Setting2 are from the TestSettingProperties.json file
            var setting2 = group.SettingInfos.Single(si => si.Name == "Test.Setting2");
            setting2.Properties[SettingUiConst.Group1].ShouldBe("TestGroup1");
            setting2.Properties[SettingUiConst.Group2].ShouldBe("TestGroup2");
            setting2.Properties[SettingUiConst.Type].ShouldBe("checkbox");
        }

        [Fact]
        public async Task Default_Property_Values_Should_Be_Set()
        {
            // Arrange
            // The TestSettingDefinitionsProvider should be executed by module system

            // Act
            var groups = await _service.GroupSettingDefinitionsAsync();

            // Assert
            // The property values of the Test.Setting3 are default
            var setting3 = groups.SelectMany(grp => grp.SettingInfos).Single(si => si.Name == "Test.Setting3");
            setting3.Properties[SettingUiConst.Group1].ShouldBe(SettingUiConst.DefaultGroup);
            setting3.Properties[SettingUiConst.Group2].ShouldBe(SettingUiConst.DefaultGroup);
            setting3.Properties[SettingUiConst.Type].ShouldBe(SettingUiConst.DefaultType);
        }

        [Fact]
        public async Task SettingValues_Should_Be_Set()
        {
            // Arrange
            var settingValues = new Dictionary<string, string>
            {
                {"setting_Test_Setting1", "value1" },
                {"setting_Test_Setting2", "value2" },
                {"RequestToken", "value3" },    // This is a invalid setting name from frontend
            };

            // Act
            await _service.SetSettingValuesAsync(settingValues);

            // Assert
            await _settingManager.Received().SetForCurrentTenantAsync("Test.Setting1", "value1");
            await _settingManager.Received().SetForCurrentTenantAsync("Test.Setting2", "value2");
            await _settingManager.DidNotReceive().SetForCurrentTenantAsync("RequestToken", "value3");
        }
    }
}