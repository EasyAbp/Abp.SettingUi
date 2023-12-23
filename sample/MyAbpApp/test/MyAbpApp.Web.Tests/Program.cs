using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using MyAbpApp;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environments.Staging
});
await builder.RunAbpModuleAsync<MyAbpAppWebTestModule>();

public partial class Program
{
}