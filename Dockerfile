# SkyBot Universe - Production Dockerfile
# Builds a headless .NET 8 bot for Linux deployment

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY SkyBot.sln ./
COPY src/Core/SkyCore.Abstractions/*.csproj ./src/Core/SkyCore.Abstractions/
COPY src/Core/SkyCore.Common/*.csproj ./src/Core/SkyCore.Common/
COPY src/Core/SkyCore.Engines/*.csproj ./src/Core/SkyCore.Engines/
COPY src/Robots/SkyCoreAtlas/SkyCoreAtlas.Core/*.csproj ./src/Robots/SkyCoreAtlas/SkyCoreAtlas.Core/
COPY src/Robots/SkyCoreAtlas/SkyCoreAtlas.cBot/*.csproj ./src/Robots/SkyCoreAtlas/SkyCoreAtlas.cBot/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build and publish
WORKDIR /src/src/Robots/SkyCoreAtlas/SkyCoreAtlas.Core
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Install minimal dependencies for headless operation
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=build /app/publish .

# Environment variables (override at runtime)
ENV CTRADER_CLIENT_ID=""
ENV CTRADER_SECRET=""
ENV CTRADER_ACCOUNT_ID=""
ENV BOT_NAME="SkyCoreAtlas"
ENV LOG_LEVEL="Information"

# Health check (optional - implement HTTP endpoint in bot)
# HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
#   CMD curl -f http://localhost:8080/health || exit 1

# Run the bot
ENTRYPOINT ["dotnet", "SkyCoreAtlas.Core.dll"]
