using System.Collections.Generic;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.Abp.SettingUi
{
    [RemoteService(Name = SettingUiRemoteServiceConsts.RemoteServiceName)]
    [Area(SettingUiRemoteServiceConsts.ModuleName)]
    [Route("api/setting-ui")] 
    [Authorize(SettingUiPermissions.ShowSettingPage)]
    public class SettingUiController : AbpController, ISettingUiAppService
    {
        private readonly ISettingUiAppService _service;
        
        public SettingUiController(ISettingUiAppService service)
        {
            _service = service;
            LocalizationResource = typeof(SettingUiResource);
        }

        [HttpGet]
        public virtual Task<List<SettingGroup>> GroupSettingDefinitionsAsync()
        {
            return _service.GroupSettingDefinitionsAsync();
        }

        [HttpPut]
        [Route("set-setting-values")]
        public virtual Task SetSettingValuesAsync(Dictionary<string, string> settingValues)
        {
            return _service.SetSettingValuesAsync(settingValues);
        }

        [HttpPut]
        [Route("reset-setting-values")]
        public virtual Task ResetSettingValuesAsync(List<string> settingNames)
        {
            return _service.ResetSettingValuesAsync(settingNames);
        }

        /// <summary>
        /// A POST, though it changes nothing: it can return a secret, and ABP does not write audit logs for GET
        /// requests by default, nor should the response be cached.
        /// </summary>
        [HttpPost]
        [Route("get-setting-value")]
        public virtual Task<string> GetSettingValueAsync(string name)
        {
            return _service.GetSettingValueAsync(name);
        }
    }
}
