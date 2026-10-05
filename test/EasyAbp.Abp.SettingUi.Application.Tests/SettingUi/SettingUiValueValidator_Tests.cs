using System.Collections.Generic;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Validation;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using Xunit;

namespace EasyAbp.Abp.SettingUi.SettingUi
{
    public class SettingUiValueValidator_Tests : SettingUiApplicationTestBase
    {
        private const string PortSetting = TestPortSettingUiValueValidator.SettingName;
        private const string HostSetting = "Test.Host";

        private readonly ISettingUiAppService _service;
        private readonly TestPortSettingUiValueValidator _portValidator = new();
        private ISettingManager _settingManager;

        public SettingUiValueValidator_Tests()
        {
            _service = GetRequiredService<ISettingUiAppService>();
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            services.AddAlwaysAllowAuthorization();

            services.AddSingleton<ISettingUiValueValidator>(_portValidator);

            var settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            var portSetting = new SettingDefinition(PortSetting, "8080")
                .WithProperty(SettingUiConst.Group1, "Server")
                .WithProperty(SettingUiConst.Group2, "Connection");
            var hostSetting = new SettingDefinition(HostSetting, "localhost")
                .WithProperty(SettingUiConst.Group1, "Server")
                .WithProperty(SettingUiConst.Group2, "Connection");
            settingDefinitionManager.GetAllAsync().Returns(new List<SettingDefinition>
            {
                portSetting,
                hostSetting
            });
            settingDefinitionManager.GetOrNullAsync(PortSetting).Returns(portSetting);
            settingDefinitionManager.GetOrNullAsync(HostSetting).Returns(hostSetting);
            services.AddSingleton(settingDefinitionManager);

            _settingManager = Substitute.For<ISettingManager>();
            services.AddSingleton(_settingManager);

            services.AddSingleton(Substitute.For<ISettingProvider>());
            // Read by the setting value providers, to tell where each value comes from.
            services.AddSingleton(Substitute.For<ISettingStore>());
        }

        [Fact]
        public async Task Should_Validate_Every_Value_And_Save_Valid_Values()
        {
            await _service.SetSettingValuesAsync(new Dictionary<string, string>
            {
                { "setting_Test_Host", "example.com" },
                { "setting_Test_Port", "443" }
            });

            _portValidator.ValidatedValues.ShouldBe(new[] { (HostSetting, "example.com"), (PortSetting, "443") });
            await _settingManager.Received(1).SetForCurrentTenantAsync(HostSetting, "example.com");
            await _settingManager.Received(1).SetForCurrentTenantAsync(PortSetting, "443");
        }

        [Fact]
        public async Task Should_Reject_An_Invalid_Value_And_Save_Nothing()
        {
            var exception = await Should.ThrowAsync<AbpValidationException>(() => _service.SetSettingValuesAsync(
                new Dictionary<string, string>
                {
                    { "setting_Test_Host", "example.com" },
                    { "setting_Test_Port", "not a port" }
                }));

            var error = exception.ValidationErrors.ShouldHaveSingleItem();
            error.ErrorMessage.ShouldBe("The port must be a number between 1 and 65535.");
            error.MemberNames.ShouldBe(new[] { PortSetting });

            // The valid value posted in the same save is not written either.
            await _settingManager.DidNotReceiveWithAnyArgs().SetAsync(default, default, default, default, default);
        }

        [Fact]
        public async Task Should_Not_Validate_A_Reset()
        {
            await _service.ResetSettingValuesAsync(new List<string> { PortSetting });

            _portValidator.ValidatedValues.ShouldBeEmpty();
            await _settingManager.Received(1).SetForCurrentTenantAsync(PortSetting, null);
        }
    }
}
