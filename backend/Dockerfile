FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Marketplace.Api/Marketplace.Api.csproj", "src/Marketplace.Api/"]
COPY ["src/Marketplace.Application/Marketplace.Application.csproj", "src/Marketplace.Application/"]
COPY ["src/Marketplace.Domain/Marketplace.Domain.csproj", "src/Marketplace.Domain/"]
COPY ["src/Marketplace.Infrastructure/Marketplace.Infrastructure.csproj", "src/Marketplace.Infrastructure/"]
RUN dotnet restore "src/Marketplace.Api/Marketplace.Api.csproj"
COPY . .
WORKDIR "/src/src/Marketplace.Api"
RUN dotnet build "Marketplace.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Marketplace.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Marketplace.Api.dll"]
