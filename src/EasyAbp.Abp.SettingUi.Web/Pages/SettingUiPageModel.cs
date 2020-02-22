using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EasyAbp.Abp.SettingUi.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class SettingUiPageModel : AbpPageModel
    {
        protected SettingUiPageModel()
        {
            LocalizationResourceType = typeof(SettingUiResource);
            ObjectMapperContext = typeof(SettingUiWebModule);
        }
    }
}