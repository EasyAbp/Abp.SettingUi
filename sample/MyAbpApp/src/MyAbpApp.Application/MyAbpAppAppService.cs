using System;
using System.Collections.Generic;
using System.Text;
using MyAbpApp.Localization;
using Volo.Abp.Application.Services;

namespace MyAbpApp
{
    /* Inherit your application services from this class.
     */
    public abstract class MyAbpAppAppService : ApplicationService
    {
        protected MyAbpAppAppService()
        {
            LocalizationResource = typeof(MyAbpAppResource);
        }
    }
}
