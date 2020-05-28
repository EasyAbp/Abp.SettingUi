using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace MyAbpApp.EntityFrameworkCore
{
    public static class MyAbpAppDbContextModelCreatingExtensions
    {
        public static void ConfigureMyAbpApp(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(MyAbpAppConsts.DbTablePrefix + "YourEntities", MyAbpAppConsts.DbSchema);

            //    //...
            //});
        }
    }
}