using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.Application.Services;

namespace EasyAbp.Abp.SettingUi
{
    public abstract class SettingUiAppService : ApplicationService
    {
        protected SettingUiAppService()
        {
            LocalizationResource = typeof(SettingUiResource);
            ObjectMapperContext = typeof(SettingUiApplicationModule);
        }
    }
}
