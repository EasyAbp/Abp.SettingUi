using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MyAbpApp.Data
{
    /* This is used if database provider does't define
     * IMyAbpAppDbSchemaMigrator implementation.
     */
    public class NullMyAbpAppDbSchemaMigrator : IMyAbpAppDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}