using Microsoft.AspNetCore.Builder;
using MyAbpApp;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
await builder.RunAbpModuleAsync<MyAbpAppWebTestModule>();

public partial class Program
{
}