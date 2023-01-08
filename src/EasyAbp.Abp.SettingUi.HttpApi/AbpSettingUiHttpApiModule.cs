﻿using System.Collections.Generic;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Localization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpSettingUiApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class AbpSettingUiHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpSettingUiHttpApiModule).Assembly);
            });
        }
    }
}