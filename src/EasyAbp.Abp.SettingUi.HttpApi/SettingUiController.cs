using System.Collections.Generic;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Localization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.Abp.SettingUi
{
    [RemoteService(Name = "EasyAbpAbpSettingUi")]
    [Area("settingUi")]
    [Route("api/setting-ui")] 
    public class SettingUiController : AbpController, ISettingUiAppService
    {
        private readonly ISettingUiAppService _service;
        
        public SettingUiController(ISettingUiAppService service)
        {
            _service = service;
            LocalizationResource = typeof(SettingUiResource);
        }

        [HttpGet]
        public virtual Task<List<SettingGroup>> GroupSettingDefinitions()
        {
            return _service.GroupSettingDefinitions();
        }

        [HttpPut]
        [Route("set-setting-values")]
        public virtual Task SetSettingValues(Dictionary<string, string> settingValues)
        {
            return _service.SetSettingValues(settingValues);
        }

        [HttpPut]
        [Route("reset-setting-values")]
        public virtual Task ResetSettingValues(List<string> settingNames)
        {
            return _service.ResetSettingValues(settingNames);
        }
    }
}
