#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WalletApi/WalletApi.csproj", "WalletApi/"]
COPY ["Authentication/Authentication.csproj", "Authentication/"]
COPY ["AuthEntities/AuthEntities.csproj", "AuthEntities/"]
COPY ["ExceptionHandling/ExceptionHandling.csproj", "ExceptionHandling/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "./WalletApi/WalletApi.csproj"
COPY . .
WORKDIR "/src/WalletApi"
RUN dotnet build "./WalletApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WalletApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WalletApi.dll"]