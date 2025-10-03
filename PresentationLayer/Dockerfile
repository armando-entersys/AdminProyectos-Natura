# ═══════════════════════════════════════════════════════════
# Multi-stage Dockerfile para AdminProyectos NATURA
# Optimizado para producción y staging
# ═══════════════════════════════════════════════════════════

# ═══════════════════════════════════════════════════════════
# STAGE 1: Base runtime image
# ═══════════════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Instalar curl para health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Crear directorios necesarios
RUN mkdir -p /app/Logs /app/wwwroot/uploads

# ═══════════════════════════════════════════════════════════
# STAGE 2: Build
# ═══════════════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto para restore (mejor cache de layers)
COPY ["EntityLayer/EntityLayer.csproj", "EntityLayer/"]
COPY ["DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]
COPY ["BusinessLayer/BusinessLayer.csproj", "BusinessLayer/"]
COPY ["PresentationLayer/PresentationLayer.csproj", "PresentationLayer/"]

# Restore de dependencias
RUN dotnet restore "PresentationLayer/PresentationLayer.csproj"

# Copiar todo el código fuente
COPY . .

# Build del proyecto
WORKDIR "/src/PresentationLayer"
RUN dotnet build "PresentationLayer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ═══════════════════════════════════════════════════════════
# STAGE 3: Publish
# ═══════════════════════════════════════════════════════════
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PresentationLayer.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false

# ═══════════════════════════════════════════════════════════
# STAGE 4: Final production image
# ═══════════════════════════════════════════════════════════
FROM base AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Crear usuario no-root para seguridad
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Permisos para directorios de escritura
RUN chown -R appuser:appuser /app/Logs /app/wwwroot/uploads

# Cambiar a usuario no-root
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=90s --retries=3 \
    CMD curl -f http://localhost:80/Home/Index || exit 1

# Labels informativos
LABEL maintainer="NATURA ADCON" \
      description="Sistema de Administración de Proyectos NATURA" \
      version="1.0.0" \
      app.name="adminproyectos" \
      app.component="web"

# Entry point
ENTRYPOINT ["dotnet", "PresentationLayer.dll"]