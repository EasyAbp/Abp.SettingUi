using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EasyAbp.Abp.SettingUi.Pages
{
    public abstract class SettingUiPageModel : AbpPageModel
    {
        protected SettingUiPageModel()
        {
            LocalizationResourceType = typeof(SettingUiResource);
        }
    }
}