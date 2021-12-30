#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["sample/MyAbpApp/NuGet.Config", "sample/MyAbpApp/"]
COPY ["sample/MyAbpApp/src/MyAbpApp.Web/MyAbpApp.Web.csproj", "sample/MyAbpApp/src/MyAbpApp.Web/"]
COPY ["src/EasyAbp.Abp.SettingUi.Web/EasyAbp.Abp.SettingUi.Web.csproj", "src/EasyAbp.Abp.SettingUi.Web/"]
COPY ["src/EasyAbp.Abp.SettingUi.HttpApi/EasyAbp.Abp.SettingUi.HttpApi.csproj", "src/EasyAbp.Abp.SettingUi.HttpApi/"]
COPY ["src/EasyAbp.Abp.SettingUi.Application.Contracts/EasyAbp.Abp.SettingUi.Application.Contracts.csproj", "src/EasyAbp.Abp.SettingUi.Application.Contracts/"]
COPY ["src/EasyAbp.Abp.SettingUi.Domain.Shared/EasyAbp.Abp.SettingUi.Domain.Shared.csproj", "src/EasyAbp.Abp.SettingUi.Domain.Shared/"]
COPY ["sample/MyAbpApp/src/MyAbpApp.Application/MyAbpApp.Application.csproj", "sample/MyAbpApp/src/MyAbpApp.Application/"]
COPY ["sample/MyAbpApp/src/MyAbpApp.Application.Contracts/MyAbpApp.Application.Contracts.csproj", "sample/MyAbpApp/src/MyAbpApp.Application.Contracts/"]
COPY ["sample/MyAbpApp/src/MyAbpApp.Domain.Shared/MyAbpApp.Domain.Shared.csproj", "sample/MyAbpApp/src/MyAbpApp.Domain.Shared/"]
COPY ["src/EasyAbp.Abp.SettingUi.Application/EasyAbp.Abp.SettingUi.Application.csproj", "src/EasyAbp.Abp.SettingUi.Application/"]
COPY ["src/EasyAbp.Abp.SettingUi.Domain/EasyAbp.Abp.SettingUi.Domain.csproj", "src/EasyAbp.Abp.SettingUi.Domain/"]
COPY ["sample/MyAbpApp/src/MyAbpApp.Domain/MyAbpApp.Domain.csproj", "sample/MyAbpApp/src/MyAbpApp.Domain/"]
COPY ["sample/MyAbpApp/src/MyAbpApp.EntityFrameworkCore/MyAbpApp.EntityFrameworkCore.csproj", "sample/MyAbpApp/src/MyAbpApp.EntityFrameworkCore/"]
COPY ["sample/MyAbpApp/src/MyAbpApp.HttpApi/MyAbpApp.HttpApi.csproj", "sample/MyAbpApp/src/MyAbpApp.HttpApi/"]
COPY Directory.Build.props .
RUN dotnet restore "sample/MyAbpApp/src/MyAbpApp.Web/MyAbpApp.Web.csproj"
COPY . .
WORKDIR "/src/sample/MyAbpApp/src/MyAbpApp.Web"
RUN dotnet build "MyAbpApp.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyAbpApp.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyAbpApp.Web.dll"]
