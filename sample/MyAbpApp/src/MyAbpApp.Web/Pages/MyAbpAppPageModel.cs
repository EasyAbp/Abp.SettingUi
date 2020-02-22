using MyAbpApp.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MyAbpApp.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class MyAbpAppPageModel : AbpPageModel
    {
        protected MyAbpAppPageModel()
        {
            LocalizationResourceType = typeof(MyAbpAppResource);
        }
    }
}