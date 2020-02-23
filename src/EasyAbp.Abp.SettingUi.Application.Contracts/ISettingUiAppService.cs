using System.Collections.Generic;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Dto;
using Volo.Abp.Application.Services;

namespace EasyAbp.Abp.SettingUi
{
    public interface ISettingUiAppService : IApplicationService
    {
        Task<IEnumerable<SettingGroup>> GroupSettingDefinitions();
        Task SetSettingValues(IDictionary<string, string> settingValues);
    }
}