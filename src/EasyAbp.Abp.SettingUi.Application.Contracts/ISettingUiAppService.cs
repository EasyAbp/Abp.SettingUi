using System.Collections.Generic;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Dto;
using Volo.Abp.Application.Services;

namespace EasyAbp.Abp.SettingUi
{
    public interface ISettingUiAppService : IApplicationService
    {
        Task<List<SettingGroup>> GroupSettingDefinitionsAsync();
        Task SetSettingValuesAsync(Dictionary<string, string> settingValues);
        Task ResetSettingValuesAsync(List<string> settingNames);

        /// <summary>
        /// Returns the value the page shows for one setting the caller is shown, decrypted. The setting list leaves
        /// out the value of an encrypted setting; the page loads it with this method when the user asks to see it.
        /// </summary>
        Task<string> GetSettingValueAsync(string name);
    }
}