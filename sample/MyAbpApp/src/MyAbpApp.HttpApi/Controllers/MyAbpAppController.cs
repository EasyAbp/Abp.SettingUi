using MyAbpApp.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyAbpApp.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class MyAbpAppController : AbpControllerBase
    {
        protected MyAbpAppController()
        {
            LocalizationResource = typeof(MyAbpAppResource);
        }
    }
}