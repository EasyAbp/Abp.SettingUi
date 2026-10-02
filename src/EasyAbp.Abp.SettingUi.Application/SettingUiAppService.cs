using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Extensions;
using EasyAbp.Abp.SettingUi.Localization;
using EasyAbp.Abp.SettingUi.Options;
using EasyAbp.Abp.SettingUi.Validation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Json;
using Volo.Abp.Localization;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Timing;
using Volo.Abp.Validation;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Abp.SettingUi
{
    [Authorize(SettingUiPermissions.ShowSettingPage)]
    public class SettingUiAppService : ApplicationService, ISettingUiAppService
    {
        private readonly AbpSettingUiOptions _options;
        private readonly IStringLocalizer<SettingUiResource> _localizer;
        private readonly IStringLocalizerFactory _factory;
        private readonly IVirtualFileProvider _fileProvider;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;
        private readonly ITimezoneProvider _timezoneProvider;
        private readonly ICurrentTimezoneProvider _currentTimezoneProvider;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;

        protected ISettingValueProviderManager SettingValueProviderManager =>
            LazyServiceProvider.LazyGetRequiredService<ISettingValueProviderManager>();

        public SettingUiAppService(
            IOptions<AbpSettingUiOptions> options,
            IStringLocalizer<SettingUiResource> localizer,
            IStringLocalizerFactory factory,
            IVirtualFileProvider fileProvider,
            IJsonSerializer jsonSerializer,
            ISettingDefinitionManager settingDefinitionManager,
            ISettingManager settingManager,
            ITimezoneProvider timezoneProvider,
            ICurrentTimezoneProvider currentTimezoneProvider,
            IPermissionDefinitionManager permissionDefinitionManager)
        {
            _options = options.Value;
            _localizer = localizer;
            _factory = factory;
            _fileProvider = fileProvider;
            _jsonSerializer = jsonSerializer;
            _settingDefinitionManager = settingDefinitionManager;
            _settingManager = settingManager;
            _timezoneProvider = timezoneProvider;
            _currentTimezoneProvider = currentTimezoneProvider;
            _permissionDefinitionManager = permissionDefinitionManager;

            LocalizationResource = typeof(SettingUiResource);
            ObjectMapperContext = typeof(AbpSettingUiApplicationModule);
        }

        public virtual async Task<List<SettingGroup>> GroupSettingDefinitionsAsync()
        {
            if (!await AuthorizationService.IsGrantedAsync(SettingUiPermissions.ShowSettingPage))
            {
                throw new AbpAuthorizationException("Authorization failed! No SettingUi policy granted.");
            }

            // Merge all setting properties into one dictionary
            var settingProperties = GetMergedSettingPropertiesAsync();

            var definedSettingUiPermissions = (await _permissionDefinitionManager.GetPermissionsAsync())
                .Where(p => p.Name.StartsWith(SettingUiPermissions.GroupName))
                .ToList();

            // Set properties of the setting definitions
            var settingDefinitions = await SetSettingDefinitionPropertiesAsync(settingProperties, definedSettingUiPermissions);

            // Group the setting definitions
            var groups = new List<SettingGroup>();
            foreach (var settingGroup in settingDefinitions
                .GroupBy(sd => sd.Properties[SettingUiConst.Group1].ToString())
                .Select(grp => new SettingGroup
                {
                    GroupName = grp.Key,
                    GroupDisplayName = _localizer[grp.Key!],
                    SettingInfos = grp.ToList(),
                    Permission = $"{SettingUiPermissions.GroupName}.{grp.Key}"
                }))
            {
                var definedSettingUiGroupPermission = definedSettingUiPermissions.FirstOrDefault(p => p.Name == settingGroup.Permission);
                if (definedSettingUiGroupPermission == null
                    || await AuthorizationService.IsGrantedAsync(definedSettingUiGroupPermission.Name) //Group1 permission check
                )
                {
                    var settingInfoGroups = settingGroup.SettingInfos
                        .GroupBy(sd => sd.Properties[SettingUiConst.Group2].ToString())
                        .Select(grp => new
                        {
                            Permission = $"{SettingUiPermissions.GroupName}.{settingGroup.GroupName}.{grp.Key}",
                            SettingInfoList = grp.ToList()
                        });

                    var settingInfos = new List<SettingInfo>();
                    foreach (var settingInfoGroup in settingInfoGroups)
                    {
                        // Group2 permission check: enforced wherever SettingUi.{Group1}.{Group2} is defined in the
                        // permission tree, with or without a Group1 permission as its parent.
                        if (definedSettingUiPermissions.All(p => p.Name != settingInfoGroup.Permission)
                            || await AuthorizationService.IsGrantedAsync(settingInfoGroup.Permission)
                        )
                        {
                            settingInfos.AddRange(settingInfoGroup.SettingInfoList);
                        }
                    }

                    settingGroup.SettingInfos = settingInfos;
                    groups.Add(settingGroup);
                }
            }

            if (_options.DisableDefaultGroup)
            {
                // remove the default group
                var defaultGroup = groups.Find(x => x.GroupName == SettingUiConst.DefaultGroup);
                if (defaultGroup is not null)
                {
                    groups.Remove(defaultGroup);
                }
            }

            return groups;
        }

        public virtual async Task SetSettingValuesAsync(Dictionary<string, string> settingValues)
        {
            // Only the settings the caller is shown can be changed; computed once for the whole request.
            var visibleSettingInfos = await GetVisibleSettingInfosAsync();

            // Check every posted setting before writing any, so a rejected request changes nothing.
            var pendingValues = new List<KeyValuePair<SettingDefinition, string>>();

            foreach (var kv in settingValues)
            {
                // The key of the settingValues is in camel_Case, like "setting_Abp_Localization_DefaultLanguage",
                // change it to "Abp.Localization.DefaultLanguage" form
                string pascalCaseName = kv.Key.ToPascalCase();
                if (!pascalCaseName.StartsWith(SettingUiConst.FormNamePrefix))
                {
                    continue;
                }

                string name = pascalCaseName.RemovePreFix(SettingUiConst.FormNamePrefix).UnderscoreToDot();
                var setting = await _settingDefinitionManager.GetOrNullAsync(name);
                if (setting == null)
                {
                    continue;
                }

                CheckSettingIsVisible(visibleSettingInfos, setting);

                var settingInfo = visibleSettingInfos[setting.Name];
                var value = NormalizeLineEndings(kv.Value);

                if (value.IsNullOrEmpty())
                {
                    if (setting.IsEncrypted)
                    {
                        // An encrypted setting is shown as an empty password box: left empty, it keeps its value.
                        continue;
                    }

                    // A cleared box resets the setting, so it inherits its value again. A stored empty string would
                    // hide the configuration, host and default values, and break the consumers of a number setting.
                    value = null;
                }

                if (value != null && IsSettingUiType(settingInfo, SettingUiConst.Components.DateTime))
                {
                    value = NormalizeDateTimeValue(value);
                }

                // The page posts every box of a card: write only the values the user changed. ABP's SettingManager
                // skips a value equal to the inherited one, but not an encrypted value, an empty one, or one with
                // other line endings, so an untouched box would otherwise pin a copy of the inherited value.
                var displayedValue = await GetDisplayedSettingValueAsync(setting, settingInfo);
                if (IsSameSettingValue(settingInfo, displayedValue, value))
                {
                    continue;
                }

                pendingValues.Add(new KeyValuePair<SettingDefinition, string>(setting, value));
            }

            // Validate all values before writing any, so an invalid value saves nothing.
            var validationErrors = new List<ValidationResult>();
            foreach (var pendingValue in pendingValues)
            {
                await ValidateSettingValueAsync(pendingValue.Key, visibleSettingInfos[pendingValue.Key.Name],
                    pendingValue.Value, validationErrors);
            }

            if (validationErrors.Any())
            {
                throw new AbpValidationException(validationErrors);
            }

            foreach (var pendingValue in pendingValues)
            {
                await SetSettingAsync(pendingValue.Key, pendingValue.Value);
            }
        }

        public virtual async Task ResetSettingValuesAsync(List<string> settingNames)
        {
            // Same authorization as showing the page: only the settings the caller is shown can be reset.
            var visibleSettingInfos = await GetVisibleSettingInfosAsync();

            // Check every requested name before resetting any, so a rejected request changes nothing.
            var settings = new List<SettingDefinition>();
            foreach (var name in settingNames)
            {
                var setting = await _settingDefinitionManager.GetOrNullAsync(name);
                if (setting == null)
                {
                    continue;
                }

                CheckSettingIsVisible(visibleSettingInfos, setting);
                settings.Add(setting);
            }

            foreach (var setting in settings)
            {
                await SetSettingAsync(setting, null); // use fallback value
            }
        }

        public virtual async Task<string> GetSettingValueAsync(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            // Same authorization as saving: only the value of a setting the caller is shown can be read.
            var visibleSettingInfos = await GetVisibleSettingInfosAsync();

            var setting = await _settingDefinitionManager.GetOrNullAsync(name);
            if (setting == null)
            {
                throw new AbpAuthorizationException($"Authorization failed! The setting '{name}' is not available to the current user.");
            }

            CheckSettingIsVisible(visibleSettingInfos, setting);

            return await GetSettingValueAsync(setting);
        }

        /// <summary>
        /// Returns the settings the current user is shown by <see cref="GroupSettingDefinitionsAsync"/>, by name.
        /// Throws <see cref="AbpAuthorizationException"/> if the user may not see the setting page at all.
        /// </summary>
        protected virtual async Task<Dictionary<string, SettingInfo>> GetVisibleSettingInfosAsync()
        {
            return (await GroupSettingDefinitionsAsync())
                .SelectMany(group => group.SettingInfos)
                .ToDictionary(settingInfo => settingInfo.Name);
        }

        /// <summary>
        /// A defined setting the page does not show the current user (hidden by a group or setting permission,
        /// by <see cref="AbpSettingUiOptions.DisableDefaultGroup"/> or by
        /// <see cref="AbpSettingUiOptions.ExcludeInVisibleToClientSettings"/>) cannot be changed either.
        /// </summary>
        protected virtual void CheckSettingIsVisible(Dictionary<string, SettingInfo> visibleSettingInfos, SettingDefinition setting)
        {
            if (!visibleSettingInfos.ContainsKey(setting.Name))
            {
                throw new AbpAuthorizationException($"Authorization failed! The setting '{setting.Name}' is not available to the current user.");
            }
        }

        /// <summary>
        /// Runs every registered <see cref="ISettingUiValueValidator"/> on a value
        /// <see cref="SetSettingValuesAsync"/> is about to write, and adds their errors to <paramref name="errors"/>.
        /// </summary>
        protected virtual Task ValidateSettingValueAsync(SettingDefinition setting, [CanBeNull] string value,
            List<ValidationResult> errors)
        {
            return ValidateSettingValueAsync(setting, null, value, errors);
        }

        /// <summary>
        /// Runs every registered <see cref="ISettingUiValueValidator"/> on a value
        /// <see cref="SetSettingValuesAsync"/> is about to write, and adds their errors to <paramref name="errors"/>.
        /// The validators see the SettingUi properties and display name of <paramref name="settingInfo"/>.
        /// </summary>
        protected virtual async Task ValidateSettingValueAsync(SettingDefinition setting,
            [CanBeNull] SettingInfo settingInfo, [CanBeNull] string value, List<ValidationResult> errors)
        {
            var validators = LazyServiceProvider.LazyGetRequiredService<IEnumerable<ISettingUiValueValidator>>();

            var context = new SettingUiValueValidationContext(setting, value, settingInfo?.Properties,
                settingInfo?.DisplayName);
            foreach (var validator in validators)
            {
                await validator.ValidateAsync(context);
            }

            errors.AddRange(context.Errors);
        }

        /// <summary>
        /// The value the page showed for a setting, which a posted value is compared with:
        /// <see cref="SettingInfo.Value"/>, or for an encrypted setting the value the page loads on demand.
        /// </summary>
        protected virtual async Task<string> GetDisplayedSettingValueAsync(SettingDefinition setting, SettingInfo settingInfo)
        {
            var value = setting.IsEncrypted ? await GetSettingValueAsync(setting) : settingInfo.Value;
            return NormalizeLineEndings(value);
        }

        /// <summary>
        /// Whether a posted value is the value the page showed, so saving it would change nothing. Both are
        /// normalized already. Values are compared as the type of the setting: a checkbox posts <c>true</c> where
        /// <c>True</c> may be stored, and a date can come back in another format.
        /// </summary>
        protected virtual bool IsSameSettingValue(SettingInfo settingInfo, [CanBeNull] string displayedValue,
            [CanBeNull] string postedValue)
        {
            if (displayedValue == null || postedValue == null)
            {
                return displayedValue == postedValue;
            }

            if (string.Equals(displayedValue, postedValue, StringComparison.Ordinal))
            {
                return true;
            }

            if (IsSettingUiType(settingInfo, SettingUiConst.Components.Checkbox))
            {
                return bool.TryParse(displayedValue.Trim(), out var displayedBool) &&
                       bool.TryParse(postedValue.Trim(), out var postedBool) &&
                       displayedBool == postedBool;
            }

            if (IsSettingUiType(settingInfo, SettingUiConst.Components.Number))
            {
                return decimal.TryParse(displayedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var displayedNumber) &&
                       decimal.TryParse(postedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var postedNumber) &&
                       displayedNumber == postedNumber;
            }

            if (IsSettingUiType(settingInfo, SettingUiConst.Components.DateTime))
            {
                // The posted value is normalized to UTC already; the shown one is normalized the same way here.
                return TryParseDateTime(NormalizeDateTimeValue(displayedValue), out var displayedDateTime) &&
                       TryParseDateTime(postedValue, out var postedDateTime) &&
                       displayedDateTime.ToUniversalTime() == postedDateTime.ToUniversalTime();
            }

            if (IsSettingUiType(settingInfo, SettingUiConst.Components.Date))
            {
                return TryParseDateTime(displayedValue, out var displayedDate) &&
                       TryParseDateTime(postedValue, out var postedDate) &&
                       displayedDate == postedDate;
            }

            return false;
        }

        /// <summary>
        /// Turns CRLF and lone CR line endings into LF. A form posts a textarea with CRLF line endings, whatever the
        /// stored value used.
        /// </summary>
        [CanBeNull]
        protected virtual string NormalizeLineEndings([CanBeNull] string value)
        {
            return value?.Replace("\r\n", "\n").Replace('\r', '\n');
        }

        /// <summary>
        /// Converts a <c>dateTime</c> value to UTC in the round-trip format. A value without time zone information
        /// is read in the time zone of the current user. A value that cannot be parsed is returned unchanged.
        /// </summary>
        protected virtual string NormalizeDateTimeValue(string value)
        {
            if (!DateTime.TryParse(value, out var dateTime))
            {
                return value;
            }

            // If the DateTime has no timezone info (most cases from input)
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                // Try to get user's timezone
                var userTz = _currentTimezoneProvider.TimeZone;
                if (!userTz.IsNullOrWhiteSpace())
                {
                    try
                    {
                        var tzInfo = _timezoneProvider.GetTimeZoneInfo(userTz);
                        // Treat the input as user's local time and convert to UTC
                        return TimeZoneInfo.ConvertTimeToUtc(dateTime, tzInfo).ToString("O");
                    }
                    catch
                    {
                        // skip handling this...
                        return value;
                    }
                }
            }

            return Clock.Normalize(dateTime).ToString("O");
        }

        private static bool TryParseDateTime(string value, out DateTime dateTime)
        {
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime) ||
                   DateTime.TryParse(value, out dateTime);
        }

        protected virtual bool IsSettingUiType(SettingInfo settingInfo, string type)
        {
            return settingInfo.Properties.TryGetValue(SettingUiConst.Type, out var settingType) &&
                   string.Equals(settingType?.ToString(), type, StringComparison.InvariantCultureIgnoreCase);
        }

        protected virtual Task SetSettingAsync(SettingDefinition setting, [CanBeNull] string value)
        {
            switch (GetManagingProviderName(setting))
            {
                case UserSettingValueProvider.ProviderName:
                    return _settingManager.SetForCurrentUserAsync(setting.Name, value);
                case GlobalSettingValueProvider.ProviderName:
                    return _settingManager.SetGlobalAsync(setting.Name, value);
                default:
                    return _settingManager.SetForCurrentTenantAsync(setting.Name, value);
            }
        }

        /// <summary>
        /// The name of the setting value provider the page saves a setting to: the current user (<c>U</c>), the
        /// global value (<c>G</c>), or the current tenant (<c>T</c>, the host's own value on the host side).
        /// </summary>
        protected virtual string GetManagingProviderName(SettingDefinition setting)
        {
            if (setting.Providers.Any(p => p == UserSettingValueProvider.ProviderName))
            {
                return UserSettingValueProvider.ProviderName;
            }

            if (setting.Providers.Any(p => p == GlobalSettingValueProvider.ProviderName))
            {
                return GlobalSettingValueProvider.ProviderName;
            }

            return ShouldManageAsGlobal(setting)
                ? GlobalSettingValueProvider.ProviderName
                : TenantSettingValueProvider.ProviderName;
        }

        protected virtual IDictionary<string, IDictionary<string, string>> GetMergedSettingPropertiesAsync()
        {
            return _fileProvider
                .GetDirectoryContents(SettingUiConst.SettingPropertiesFileFolder)
                .Where(x => x.Name.EndsWith(".json"))
                .Select(content =>
                    _jsonSerializer.Deserialize<IDictionary<string, IDictionary<string, string>>>(content.ReadAsString()))
                .SelectMany(dict => dict)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        protected virtual async Task<List<SettingInfo>> SetSettingDefinitionPropertiesAsync(IDictionary<string, IDictionary<string, string>> settingProperties, IList<PermissionDefinition> permissionDefinitions)
        {
            var settingInfos = new List<SettingInfo>();
            var settingDefinitions = (await _settingDefinitionManager.GetAllAsync())
                .WhereIf(_options.ExcludeInVisibleToClientSettings, setting => setting.IsVisibleToClients);
            foreach (var settingDefinition in settingDefinitions)
            {
				var si =  await CreateSettingInfoAsync(settingDefinition);

                if (settingProperties.ContainsKey(si.Name))
                {
                    // This Setting is defined in the property file,
                    // set its property values from the dictionary
                    var properties = settingProperties[si.Name];
                    foreach (var kv in properties)
                    {
                        // Do not assign the property if it has already been set by the user.
                        if (!si.Properties.ContainsKey(kv.Key))
                        {
							si.Properties[kv.Key] =  kv.Value;
                        }
                    }
                }

                // Default group1: Others
                if (!si.Properties.ContainsKey(SettingUiConst.Group1))
                {
                    si.Properties[SettingUiConst.Group1] = SettingUiConst.DefaultGroup;
                }

                // Default group2: Others
                if (!si.Properties.ContainsKey(SettingUiConst.Group2))
                {
					si.Properties[SettingUiConst.Group2] =  SettingUiConst.DefaultGroup;
                }

                // Default type: text
                if (!si.Properties.ContainsKey(SettingUiConst.Type))
                {
                    si.Properties[SettingUiConst.Type] = SettingUiConst.DefaultType;
                }

                // Setting permission check, once the groups are known
                var permissionNames = FindSettingPermissionNames(si, permissionDefinitions);
                if (permissionNames.Any())
                {
                    si.Permission = permissionNames.First();
                    if (!await IsGrantedAllAsync(permissionNames))
                    {
                        continue;
                    }
                }

                settingInfos.Add(si);
            }

            return settingInfos;
        }

        /// <summary>
        /// The name of the permission that, when defined, must be granted to show a setting:
        /// <c>SettingUi.{Group1}.{Group2}.{SettingName}</c>, for example
        /// <c>SettingUi.System.Password.Abp.Identity.Password.RequiredLength</c>.
        /// </summary>
        protected virtual string GetSettingPermissionName(SettingInfo settingInfo)
        {
            return $"{SettingUiPermissions.GroupName}.{settingInfo.Properties[SettingUiConst.Group1]}." +
                   $"{settingInfo.Properties[SettingUiConst.Group2]}.{settingInfo.Name}";
        }

        /// <summary>
        /// The permissions that must all be granted to show a setting; empty if the setting is unrestricted.
        /// <para>
        /// The permission named <see cref="GetSettingPermissionName"/> is used when it is defined. Otherwise, for
        /// compatibility with version 2.10 and earlier, every SettingUi permission whose name ends with the setting name
        /// is used and a warning is logged so the application can rename it. Requiring all of them, where 2.10 used
        /// whichever came first, never shows a setting that 2.10 hid, and does not depend on definition order.
        /// </para>
        /// </summary>
        protected virtual List<string> FindSettingPermissionNames(SettingInfo settingInfo,
            IList<PermissionDefinition> permissionDefinitions)
        {
            var expectedPermissionName = GetSettingPermissionName(settingInfo);
            if (permissionDefinitions.Any(p => p.Name == expectedPermissionName))
            {
                return new List<string> { expectedPermissionName };
            }

            var legacyPermissionNames = permissionDefinitions
                .Where(p => p.Name != SettingUiPermissions.ShowSettingPage && p.Name.EndsWith(settingInfo.Name))
                .Select(p => p.Name)
                .ToList();

            if (legacyPermissionNames.Any())
            {
                Logger.LogWarning(
                    "The setting {SettingName} is protected by {LegacyPermissionNames} only because the permission name " +
                    "ends with the setting name. This is deprecated: name the setting permission {ExpectedPermissionName} " +
                    "(SettingUi.{{Group1}}.{{Group2}}.{{SettingName}}).",
                    settingInfo.Name, string.Join(", ", legacyPermissionNames), expectedPermissionName);
            }

            return legacyPermissionNames;
        }

        protected virtual async Task<bool> IsGrantedAllAsync(IEnumerable<string> permissionNames)
        {
            foreach (var permissionName in permissionNames)
            {
                if (!await AuthorizationService.IsGrantedAsync(permissionName))
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual async Task<SettingInfo> CreateSettingInfoAsync(SettingDefinition settingDefinition)
        {
            string name = settingDefinition.Name;
            string displayName;
            if (settingDefinition.DisplayName is FixedLocalizableString fls && fls.Value == settingDefinition.Name)
            {
                displayName = _localizer[$"DisplayName:{settingDefinition.Name}"];
            }
            else
            {
                displayName = settingDefinition.DisplayName.Localize(_factory);
            }

            string description;
            if (settingDefinition.Description == null)
            {
                string descName = $"Description:{settingDefinition.Name}";
                description = _localizer[descName];
                if (description == descName)
                {
                    // No localized description found
                    description = String.Empty;
                }
            }
            else
            {
                description = settingDefinition.Description.Localize(_factory);
            }

            var value = await GetSettingValueAsync(settingDefinition);
            var valueProviderName = await GetSettingValueProviderNameAsync(settingDefinition);

            var si = new SettingInfo
            {
                Name = name,
                DisplayName = displayName,
                Description = description,
                // The page loads the value of an encrypted setting on demand, so a secret is not sent with the list.
                Value = settingDefinition.IsEncrypted ? null : value,
                Properties = new ExtraPropertyDictionary(),
                IsEncrypted = settingDefinition.IsEncrypted,
                HasValue = value != null,
                ValueProviderName = valueProviderName,
                IsValueSetHere = valueProviderName != null &&
                                 valueProviderName == GetManagingProviderName(settingDefinition),
            };

            // Copy properties from SettingDefinition
            foreach (var property in settingDefinition.Properties)
            {
                si.Properties[property.Key] = property.Value;
            }

            return si;
        }

        protected virtual async Task<string> GetSettingValueAsync(SettingDefinition settingDefinition)
        {
            /* Hide default/global value for tenants if the setting item is encrypted. */
            if (settingDefinition.IsEncrypted && CurrentTenant.IsAvailable)
            {
                return await _settingManager.GetOrNullForCurrentTenantAsync(settingDefinition.Name, false);
            }

            return ShouldManageAsGlobal(settingDefinition)
                ? await _settingManager.GetOrNullGlobalAsync(settingDefinition.Name)
                : await SettingProvider.GetOrNullAsync(settingDefinition.Name);
        }

        /// <summary>
        /// The name of the setting value provider the value of <see cref="GetSettingValueAsync(SettingDefinition)"/>
        /// comes from, or <c>null</c> when no provider has a value. Looks the value up the same way.
        /// </summary>
        [ItemCanBeNull]
        protected virtual async Task<string> GetSettingValueProviderNameAsync(SettingDefinition settingDefinition)
        {
            /*
             * Only the tenant's own value of an encrypted setting is shown to a tenant. Without one, the tenant still
             * uses the inherited value, so its source is reported (the value itself is not) by the walk below.
             */
            if (settingDefinition.IsEncrypted && CurrentTenant.IsAvailable &&
                await _settingManager.GetOrNullForCurrentTenantAsync(settingDefinition.Name, false) != null)
            {
                return TenantSettingValueProvider.ProviderName;
            }

            var providerNames = Enumerable.Reverse(SettingValueProviderManager.Providers).Select(p => p.Name);

            if (ShouldManageAsGlobal(settingDefinition))
            {
                // The global value, falling back like SettingManager.GetOrNullGlobalAsync does.
                providerNames = providerNames.SkipWhile(p => p != GlobalSettingValueProvider.ProviderName);
                if (!settingDefinition.IsInherited)
                {
                    providerNames = providerNames.Take(1);
                }

                foreach (var providerName in providerNames)
                {
                    if (await _settingManager.GetOrNullAsync(settingDefinition.Name, providerName, null, false) != null)
                    {
                        return providerName;
                    }
                }

                return null;
            }

            // The effective value, walking the providers from the highest priority down like SettingProvider does.
            var providers = Enumerable.Reverse(SettingValueProviderManager.Providers);
            if (settingDefinition.Providers.Any())
            {
                providers = providers.Where(p => settingDefinition.Providers.Contains(p.Name));
            }

            foreach (var provider in providers)
            {
                if (await provider.GetOrNullAsync(settingDefinition) != null)
                {
                    return provider.Name;
                }
            }

            return null;
        }

        protected virtual bool ShouldManageAsGlobal(SettingDefinition settingDefinition)
        {
            // todo: settingDefinition.Providers.Count != 0 can be improved.
            return !CurrentTenant.IsAvailable &&
                   _options.ManageGlobalSettingsOnHostSide &&
                   settingDefinition.Providers.Count == 0;
        }
    }
}