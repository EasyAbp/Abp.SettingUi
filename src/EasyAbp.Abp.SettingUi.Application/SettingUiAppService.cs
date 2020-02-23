using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Extensions;
using EasyAbp.Abp.SettingUi.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Json;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Abp.SettingUi
{
    public class SettingUiAppService : ApplicationService, ISettingUiAppService
    {
        private readonly IStringLocalizer<SettingUiResource> _localizer;
        private readonly IVirtualFileProvider _fileProvider;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;

        public SettingUiAppService(IStringLocalizer<SettingUiResource> localizer, IVirtualFileProvider fileProvider, IJsonSerializer jsonSerializer, ISettingDefinitionManager settingDefinitionManager, ISettingManager settingManager)
        {
            _localizer = localizer;
            _fileProvider = fileProvider;
            _jsonSerializer = jsonSerializer;
            _settingDefinitionManager = settingDefinitionManager;
            _settingManager = settingManager;
        }

        public async Task<IEnumerable<SettingGroup>> GroupSettingDefinitions()
        {
            if (!await AuthorizationService.IsGrantedAsync(SettingUiPermissions.Global))
            {
                throw new AbpAuthorizationException("Authorization failed! No SettingUi policy granted.");
            }

            // Merge all the setting properties in to one dictionary
            var settingProperties = GetMergedSettingProperties();

            // Set the properties of the setting definitions
            var settingDefinitions = SetSettingDefinitionProperties(settingProperties);

            // Group the setting definitions
            return settingDefinitions
                    .GroupBy(sd => sd.Properties[SettingUiConst.Group1].ToString())
                    .Select(grp => new SettingGroup
                    {
                        GroupName = grp.Key,
                        GroupDisplayName = _localizer[grp.Key],
                        SettingDefinitions = grp
                    })
                ;
        }

        public async Task SetSettingValues(IDictionary<string, string> settingValues)
        {
            foreach (var kv in settingValues)
            {
                // The key of the settingValues is in camelCase, like "setting_Abp_Localization_DefaultLanguage",
                // change it to "Abp.Localization.DefaultLanguage" form
                string pascalCaseName = kv.Key.ToPascalCase();
                if (!pascalCaseName.StartsWith(SettingUiConst.FormNamePrefix))
                {
                    continue;
                }

                string name = pascalCaseName.RemovePreFix(SettingUiConst.FormNamePrefix).UnderscoreToDot();
                await _settingManager.SetGlobalAsync(name, kv.Value);
            }
        }

        private IDictionary<string, IDictionary<string, string>> GetMergedSettingProperties()
        {
            return _fileProvider
                .GetDirectoryContents(SettingUiConst.SettingPropertiesFileFolder)
                .Select(content =>
                    _jsonSerializer.Deserialize<IDictionary<string, IDictionary<string, string>>>(content.ReadAsString()))
                .SelectMany(dict => dict)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private IEnumerable<SettingDefinition> SetSettingDefinitionProperties(IDictionary<string, IDictionary<string, string>> settingProperties)
        {
            var settingDefinitions = _settingDefinitionManager.GetAll();
            foreach (var settingDefinition in settingDefinitions)
            {
                if (settingProperties.ContainsKey(settingDefinition.Name))
                {
                    // This SettingDefinition is defined in the property file,
                    // set its property values from the dictionary
                    var properties = settingProperties[settingDefinition.Name];
                    foreach (var kv in properties)
                    {
                        settingDefinition.WithProperty(kv.Key, kv.Value);
                    }
                }

                // Default group1: Others
                if (!settingDefinition.Properties.ContainsKey(SettingUiConst.Group1))
                {
                    settingDefinition.WithProperty(SettingUiConst.Group1,
                        SettingUiConst.DefaultGroup);
                }

                // Default group2: Others
                if (!settingDefinition.Properties.ContainsKey(SettingUiConst.Group2))
                {
                    settingDefinition.WithProperty(SettingUiConst.Group2,
                        SettingUiConst.DefaultGroup);
                }

                // Default type: text
                if (!settingDefinition.Properties.ContainsKey(SettingUiConst.Type))
                {
                    settingDefinition.WithProperty(SettingUiConst.Type,
                        SettingUiConst.DefaultType);
                }
            }

            return settingDefinitions;
        }
    }
}