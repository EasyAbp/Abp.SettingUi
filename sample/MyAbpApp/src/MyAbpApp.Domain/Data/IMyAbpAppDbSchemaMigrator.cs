using System.Threading.Tasks;

namespace MyAbpApp.Data
{
    public interface IMyAbpAppDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
