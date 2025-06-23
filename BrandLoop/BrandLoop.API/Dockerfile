# Base image cho runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 7222

# Image để build project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy từng project
COPY ["BrandLoop.API/BrandLoop.API.csproj", "BrandLoop.API/"]
COPY ["BrandLoop.Application/BrandLoop.Application.csproj", "BrandLoop.Application/"]
COPY ["BrandLoop.Domain/BrandLoop.Domain.csproj", "BrandLoop.Domain/"]
COPY ["BrandLoop.Shared/BrandLoop.Shared.csproj", "BrandLoop.Shared/"]
COPY ["BrandLoop.Infratructure/BrandLoop.Infratructure.csproj", "BrandLoop.Infratructure/"]

# Restore dependencies
RUN dotnet restore "BrandLoop.API/BrandLoop.API.csproj"

# Copy toàn bộ source
COPY . .

WORKDIR "/src/BrandLoop.API"
RUN dotnet build "BrandLoop.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BrandLoop.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy output đã publish
COPY --from=publish /app/publish .

# Copy file cert vào đúng stage
COPY BrandLoop.API/brandloop.pfx /app/https/brandloop.pfx

ENTRYPOINT ["dotnet", "BrandLoop.API.dll"]
