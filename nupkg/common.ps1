# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$rootFolder = Join-Path $packFolder "../"

# List of projects
$projects = (

    "src/EasyAbp.Abp.SettingUi.Application",
    "src/EasyAbp.Abp.SettingUi.Application.Contracts",
    "src/EasyAbp.Abp.SettingUi.Domain",
    "src/EasyAbp.Abp.SettingUi.Domain.Shared",
    "src/EasyAbp.Abp.SettingUi.HttpApi",
    "src/EasyAbp.Abp.SettingUi.HttpApi.Client",
    "src/EasyAbp.Abp.SettingUi.Web"
)