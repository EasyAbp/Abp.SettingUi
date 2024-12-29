using Microsoft.AspNetCore.Builder;
using MyAbpApp;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();

builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("MyAbpApp.Web.csproj");
await builder.RunAbpModuleAsync<MyAbpAppWebTestModule>(applicationName: "MyAbpApp.Web" );

public partial class Program
{
}