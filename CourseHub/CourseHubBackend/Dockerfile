# syntax=docker/dockerfile:1

# ---- Build stage ----
# Uses the full SDK image (has the compiler + dotnet-ef's dependencies,
# but we don't run dotnet-ef here — see docker-compose.yml/README for
# why migrations are applied by the app itself at startup instead).
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the .csproj files first and restore before copying the rest
# of the source. Docker caches each layer by its inputs — as long as no
# .csproj changes, this restore layer is reused on every rebuild instead
# of re-downloading every NuGet package on every code change.
COPY src/CourseHub.API/CourseHubBackend.csproj src/CourseHub.API/
COPY src/CourseHub.Application/CourseHub.Application.csproj src/CourseHub.Application/
COPY src/CourseHub.Domain/CourseHub.Domain.csproj src/CourseHub.Domain/
COPY src/CourseHub.Infrastructure/CourseHub.Infrastructure.csproj src/CourseHub.Infrastructure/

RUN dotnet restore src/CourseHub.API/CourseHubBackend.csproj

# Now bring in the actual source and publish. Test projects are
# deliberately never copied into this image (see .dockerignore) — they
# aren't needed to run the API and would only bloat the build context.
COPY src/ src/
RUN dotnet publish src/CourseHub.API/CourseHubBackend.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---- Runtime stage ----
# The much smaller ASP.NET Core runtime image (no SDK/compiler) — this is
# what actually ships. Only the published output from the build stage
# above is copied in.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# curl is needed for the HEALTHCHECK instruction below; the base runtime
# image doesn't include it by default.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Run as a non-root user. The aspnet base image ships a built-in
# unprivileged "app" user (UID 64198) for exactly this purpose — no need
# to create one manually.
USER app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Mirrors the /health/live endpoint mapped in Program.cs — see that
# file's comment for why liveness (this) and readiness (checks the DB)
# are separate endpoints. Docker/Compose use this HEALTHCHECK to know
# when the container itself is actually up, not just that the process
# started.
HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD curl -f http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "CourseHubBackend.dll"]
