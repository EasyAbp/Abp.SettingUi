using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.Abp.SettingUi
{
    public abstract class SettingUiController : AbpController
    {
        protected SettingUiController()
        {
            LocalizationResource = typeof(SettingUiResource);
        }
    }
}
