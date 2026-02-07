# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TimeTracker.WebApi/TimeTracker.WebApi.csproj", "TimeTracker.WebApi/"]
COPY ["TimeTracker.Application.Interfaces/TimeTracker.Application.Interfaces.csproj", "TimeTracker.Application.Interfaces/"]
COPY ["TimeTracker.Application/TimeTracker.Application.csproj", "TimeTracker.Application/"]
COPY ["TimeTracker.Common/TimeTracker.Common.csproj", "TimeTracker.Common/"]
COPY ["TimeTracker.Domain/TimeTracker.Domain.csproj", "TimeTracker.Domain/"]
COPY ["TimeTracker.IoC/TimeTracker.IoC.csproj", "TimeTracker.IoC/"]
COPY ["TimeTracker.WebApi.Contracts/TimeTracker.WebApi.Contracts.csproj", "TimeTracker.WebApi.Contracts/"]
COPY ["TimeTracker.Infrastructure.Data.SqlServer/TimeTracker.Infrastructure.Data.SqlServer.csproj", "TimeTracker.Infrastructure.Data.SqlServer/"]
COPY ["TimeTracker.Infrastructure.Auth.Custom/TimeTracker.Infrastructure.Auth.Custom.csproj", "TimeTracker.Infrastructure.Auth.Custom/"]
COPY ["TimeTracker.Infrastructure.Messaging.ServiceBus/TimeTracker.Infrastructure.Messaging.ServiceBus.csproj", "TimeTracker.Infrastructure.Messaging.ServiceBus/"]
COPY ["TimeTracker.Infrastructure.Email.Smtp/TimeTracker.Infrastructure.Email.Smtp.csproj", "TimeTracker.Infrastructure.Email.Smtp/"]
RUN dotnet restore "./TimeTracker.WebApi/TimeTracker.WebApi.csproj"
COPY . .
WORKDIR "/src/TimeTracker.WebApi"
RUN dotnet build "./TimeTracker.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TimeTracker.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeTracker.WebApi.dll"]