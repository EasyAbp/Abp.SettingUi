using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Options;
using EasyAbp.Abp.SettingUi.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Volo.Abp.Authorization;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using Xunit;
using S = EasyAbp.Abp.SettingUi.Settings.SaveSemanticsSettingDefinitionProvider;

namespace EasyAbp.Abp.SettingUi.SettingUi
{
    /// <summary>
    /// What a save writes, against the real <see cref="ISettingManager"/> and <see cref="ISettingProvider"/> over an
    /// in-memory AbpSettings table.
    /// </summary>
    public class SettingUiSaveSemantics_Tests : SettingUiApplicationTestBase
    {
        private static readonly Guid TenantId = Guid.Parse("8a5e3c5e-5b0f-4b5e-9a57-0c2d7d0e6f11");

        private readonly InMemorySettingManagementStore _store = new();
        private readonly IConfigurationRoot _configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

        protected override void AfterAddApplication(IServiceCollection services)
        {
            services.AddAlwaysAllowAuthorization();
            services.AddSingleton<ISettingManagementStore>(_store);
            services.ReplaceConfiguration(_configuration);
            services.Configure<SettingManagementOptions>(options => options.SaveStaticSettingsToDatabase = false);
        }

        private ISettingUiAppService Service => GetRequiredService<ISettingUiAppService>();

        private ISettingManager SettingManager => GetRequiredService<ISettingManager>();

        private AbpSettingUiOptions Options => GetRequiredService<IOptions<AbpSettingUiOptions>>().Value;

        private IDisposable AsTenant() => GetRequiredService<ICurrentTenant>().Change(TenantId);

        private static string FormKey(string settingName) => "setting_" + settingName.Replace('.', '_');

        private async Task<Dictionary<string, SettingInfo>> GetCardAsync()
        {
            return (await Service.GroupSettingDefinitionsAsync())
                .Single(group => group.GroupName == S.Group1)
                .SettingInfos
                .ToDictionary(settingInfo => settingInfo.Name);
        }

        /// <summary>
        /// What the page posts when the user saves the card without touching it: an empty password box for an
        /// encrypted setting, <c>true</c> or <c>false</c> for a checkbox, and CRLF line endings.
        /// </summary>
        private static Dictionary<string, string> PostedUnchanged(IEnumerable<SettingInfo> settingInfos)
        {
            return settingInfos.ToDictionary(settingInfo => FormKey(settingInfo.Name), settingInfo =>
            {
                if (settingInfo.IsEncrypted)
                {
                    return "";
                }

                if ((string)settingInfo.Properties[SettingUiConst.Type] == SettingUiConst.Components.Checkbox)
                {
                    return settingInfo.Value?.ToLowerInvariant();
                }

                return (settingInfo.Value ?? "").Replace("\n", "\r\n");
            });
        }

        private Task SaveAsync(string settingName, string value)
        {
            return Service.SetSettingValuesAsync(new Dictionary<string, string> { { FormKey(settingName), value } });
        }

        private async Task SetConfigurationValueAsync(string settingName, string value)
        {
            var setting = await GetRequiredService<ISettingDefinitionManager>().GetAsync(settingName);
            // ABP decrypts an encrypted setting whichever provider it comes from, the configuration included.
            _configuration[ConfigurationSettingValueProvider.ConfigurationNamePrefix + settingName] = setting.IsEncrypted
                ? GetRequiredService<ISettingEncryptionService>().Encrypt(setting, value)
                : value;
        }

        private List<string> SnapshotRows() => _store.Rows.Select(row => row.ToString()).OrderBy(row => row).ToList();

        [Fact]
        public async Task Saving_An_Untouched_Card_Should_Write_Nothing_For_A_Tenant()
        {
            await SettingManager.SetGlobalAsync(S.Text, "host text");
            await SettingManager.SetGlobalAsync(S.Checkbox, "False");
            await SettingManager.SetForTenantAsync(TenantId, S.Number, "7");
            await SettingManager.SetForTenantAsync(TenantId, S.Secret, "tenant secret");
            await SetConfigurationValueAsync(S.KeyVaultSecret, "key vault secret");
            var rows = SnapshotRows();
            _store.Writes.Clear();

            using (AsTenant())
            {
                await Service.SetSettingValuesAsync(PostedUnchanged((await GetCardAsync()).Values));
            }

            _store.Writes.ShouldBeEmpty();
            SnapshotRows().ShouldBe(rows);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Saving_An_Untouched_Card_Should_Write_Nothing_On_The_Host(bool manageGlobalSettingsOnHostSide)
        {
            Options.ManageGlobalSettingsOnHostSide = manageGlobalSettingsOnHostSide;
            await SettingManager.SetGlobalAsync(S.Text, "host text");
            await SettingManager.SetGlobalAsync(S.Secret, "global secret");
            await SettingManager.SetForCurrentTenantAsync(S.Number, "7"); // the host's own value
            await SetConfigurationValueAsync(S.KeyVaultSecret, "key vault secret");
            var rows = SnapshotRows();
            _store.Writes.Clear();

            await Service.SetSettingValuesAsync(PostedUnchanged((await GetCardAsync()).Values));

            _store.Writes.ShouldBeEmpty();
            SnapshotRows().ShouldBe(rows);
        }

        [Fact]
        public async Task Clearing_A_Box_Should_Reset_The_Setting()
        {
            await SettingManager.SetGlobalAsync(S.Text, "host text");
            await SettingManager.SetForTenantAsync(TenantId, S.Text, "tenant text");
            await SettingManager.SetForTenantAsync(TenantId, S.Number, "7");

            using (AsTenant())
            {
                var card = await GetCardAsync();
                card[S.Text].Value.ShouldBe("tenant text");

                await SaveAsync(S.Text, "");
                await SaveAsync(S.Number, "");

                card = await GetCardAsync();
                card[S.Text].Value.ShouldBe("host text");
                card[S.Number].Value.ShouldBe("5");
            }

            // The rows are deleted, not set to an empty string; the host value is untouched.
            (await SettingManager.GetOrNullForTenantAsync(S.Text, TenantId, false)).ShouldBeNull();
            (await SettingManager.GetOrNullForTenantAsync(S.Number, TenantId, false)).ShouldBeNull();
            (await SettingManager.GetOrNullGlobalAsync(S.Text, false)).ShouldBe("host text");
            _store.Rows.ShouldNotContain(row => row.Value == "");
        }

        [Fact]
        public async Task An_Empty_String_Stored_By_An_Earlier_Version_Should_Be_Reset_By_The_Next_Save()
        {
            await SettingManager.SetGlobalAsync(S.Text, "host text");
            _store.Rows.Add(new InMemorySettingManagementStore.SettingRow(S.Text, "",
                TenantSettingValueProvider.ProviderName, TenantId.ToString()));

            using (AsTenant())
            {
                await Service.SetSettingValuesAsync(PostedUnchanged((await GetCardAsync()).Values));

                (await GetCardAsync())[S.Text].Value.ShouldBe("host text");
            }

            _store.Rows.ShouldNotContain(row => row.Value == "");
        }

        [Fact]
        public async Task Line_Endings_Should_Be_Normalized()
        {
            using (AsTenant())
            {
                // An untouched textarea comes back with CRLF line endings.
                await SaveAsync(S.MultiLine, S.MultiLineDefault.Replace("\n", "\r\n"));
                _store.Writes.ShouldBeEmpty();

                await SaveAsync(S.MultiLine, "a\r\nb\rc");
                (await SettingManager.GetOrNullForTenantAsync(S.MultiLine, TenantId, false)).ShouldBe("a\nb\nc");

                _store.Writes.Clear();
                await SaveAsync(S.MultiLine, "a\r\nb\r\nc");
                _store.Writes.ShouldBeEmpty();
            }
        }

        [Fact]
        public async Task The_Setting_List_Should_Not_Carry_An_Encrypted_Value()
        {
            await SettingManager.SetForTenantAsync(TenantId, S.Secret, "tenant secret");
            await SetConfigurationValueAsync(S.KeyVaultSecret, "key vault secret");

            using (AsTenant())
            {
                var card = await GetCardAsync();
                card[S.Secret].IsEncrypted.ShouldBeTrue();
                card[S.Secret].Value.ShouldBeNull();
                card[S.Secret].HasValue.ShouldBeTrue();

                // A tenant is shown only its own encrypted value, never the one of the host or the configuration.
                card[S.KeyVaultSecret].Value.ShouldBeNull();
                card[S.KeyVaultSecret].HasValue.ShouldBeFalse();
                (await Service.GetSettingValueAsync(S.KeyVaultSecret)).ShouldBeNull();
            }

            var hostCard = await GetCardAsync();
            hostCard[S.KeyVaultSecret].Value.ShouldBeNull();
            hostCard[S.KeyVaultSecret].HasValue.ShouldBeTrue();
            hostCard[S.Secret].HasValue.ShouldBeFalse();
        }

        [Fact]
        public async Task An_Encrypted_Value_Should_Be_Kept_Unless_Replaced_Or_Reset()
        {
            await SettingManager.SetForTenantAsync(TenantId, S.Secret, "old secret");
            _store.Writes.Clear();

            using (AsTenant())
            {
                // An empty password box keeps the value.
                await SaveAsync(S.Secret, "");
                _store.Writes.ShouldBeEmpty();

                // The eye loads it; saving it unchanged writes nothing.
                (await Service.GetSettingValueAsync(S.Secret)).ShouldBe("old secret");
                await SaveAsync(S.Secret, "old secret");
                _store.Writes.ShouldBeEmpty();

                await SaveAsync(S.Secret, "new secret");
                (await SettingManager.GetOrNullForTenantAsync(S.Secret, TenantId, false)).ShouldBe("new secret");
                _store.Find(S.Secret, TenantSettingValueProvider.ProviderName, TenantId.ToString())
                    .Value.ShouldNotBe("new secret"); // stored encrypted

                await Service.ResetSettingValuesAsync(new List<string> { S.Secret });
                (await SettingManager.GetOrNullForTenantAsync(S.Secret, TenantId, false)).ShouldBeNull();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task An_Encrypted_Value_From_The_Configuration_Should_Not_Be_Copied_On_The_Host(
            bool manageGlobalSettingsOnHostSide)
        {
            Options.ManageGlobalSettingsOnHostSide = manageGlobalSettingsOnHostSide;
            await SetConfigurationValueAsync(S.KeyVaultSecret, "key vault secret");

            await Service.SetSettingValuesAsync(PostedUnchanged((await GetCardAsync()).Values));

            (await Service.GetSettingValueAsync(S.KeyVaultSecret)).ShouldBe("key vault secret");
            await SaveAsync(S.KeyVaultSecret, "key vault secret");

            // Neither an untouched box nor the revealed value pins a copy that would outlive a rotation.
            _store.Writes.ShouldBeEmpty();
            _store.Rows.ShouldBeEmpty();
        }

        [Fact]
        public async Task Revealing_A_Value_Should_Be_Refused_For_A_Setting_The_Caller_Is_Not_Shown()
        {
            (await Service.GetSettingValueAsync(S.Ungrouped)).ShouldBe("ungrouped");

            Options.DisableDefaultGroup = true;

            await Should.ThrowAsync<AbpAuthorizationException>(() => Service.GetSettingValueAsync(S.Ungrouped));
            await Should.ThrowAsync<AbpAuthorizationException>(() => Service.GetSettingValueAsync("Not.A.Defined.Setting"));
        }

        [Theory]
        [InlineData(S.Number, "abc", "Save number must be a whole number.")]
        [InlineData(S.Number, "1.5", "Save number must be a whole number.")]
        [InlineData(S.Date, "not a date", "Save date must be a valid date.")]
        [InlineData(S.DateTime, "not a date", "Save date time must be a valid date and time.")]
        public async Task An_Invalid_Value_Of_A_Built_In_Type_Should_Be_Refused(string settingName, string value,
            string message)
        {
            using (CultureHelper.Use("en"))
            using (AsTenant())
            {
                var exception = await Should.ThrowAsync<AbpValidationException>(() => Service.SetSettingValuesAsync(
                    new Dictionary<string, string>
                    {
                        { FormKey(S.Text), "changed" },
                        { FormKey(settingName), value }
                    }));

                var error = exception.ValidationErrors.ShouldHaveSingleItem();
                error.ErrorMessage.ShouldBe(message);
                error.MemberNames.ShouldBe(new[] { settingName });
            }

            // The valid value posted with it is not written either.
            _store.Writes.ShouldBeEmpty();
        }

        [Fact]
        public async Task A_Valid_Value_Of_A_Built_In_Type_Or_A_Reset_Should_Be_Saved()
        {
            using (AsTenant())
            {
                await Service.SetSettingValuesAsync(new Dictionary<string, string>
                {
                    { FormKey(S.Number), "12" },
                    { FormKey(S.Date), "2026-02-01" },
                    { FormKey(S.DateTime), "2026-02-01T10:00:00.0000000Z" }
                });

                (await SettingManager.GetOrNullForTenantAsync(S.Number, TenantId, false)).ShouldBe("12");
                (await SettingManager.GetOrNullForTenantAsync(S.Date, TenantId, false)).ShouldBe("2026-02-01");
                DateTime.Parse(await SettingManager.GetOrNullForTenantAsync(S.DateTime, TenantId, false))
                    .ToUniversalTime().ShouldBe(new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc));

                // A cleared box is validated as null, which every built-in type accepts.
                await Service.SetSettingValuesAsync(new Dictionary<string, string>
                {
                    { FormKey(S.Number), "" },
                    { FormKey(S.Date), "" },
                    { FormKey(S.DateTime), "" }
                });

                (await SettingManager.GetOrNullForTenantAsync(S.Number, TenantId, false)).ShouldBeNull();
                (await SettingManager.GetOrNullForTenantAsync(S.Date, TenantId, false)).ShouldBeNull();
                (await SettingManager.GetOrNullForTenantAsync(S.DateTime, TenantId, false)).ShouldBeNull();
            }
        }
    }
}
