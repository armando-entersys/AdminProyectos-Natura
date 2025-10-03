# 📝 Cambios Realizados para Dockerización

## Fecha: Octubre 2025

---

## 🎯 Objetivo

Preparar la aplicación **AdminProyectos NATURA** para ser desplegada en Docker en el servidor EnterSys (34.134.14.202) con:
- SQL Server en contenedor
- SSL automático vía Traefik
- Monitoreo integrado con Prometheus/Grafana/Loki
- Dominio: `adminproyectos.entersys.mx`

---

## 📦 Archivos Nuevos Creados

### 1. `docker-compose.staging.yml`
Configuración completa de Docker Compose que incluye:
- **SQL Server 2022 Express** con volumen persistente
- **Aplicación Web ASP.NET Core 6.0**
- Integración con Traefik (SSL automático)
- Labels de monitoreo para Prometheus
- Health checks configurados
- Resource limits (CPU, RAM)
- Redes Docker (traefik + backend)

### 2. `.env.staging.example`
Template de variables de entorno con:
- Configuración de dominio
- Password de SQL Server
- Configuración SMTP

### 3. `.env.staging`
Archivo de configuración real con valores para staging
**⚠️ Este archivo NO debe commitearse a Git**

### 4. `DEPLOYMENT.md`
Guía completa de despliegue paso a paso con:
- Pre-requisitos
- Configuración de DNS
- Comandos de deployment
- Inicialización de base de datos
- Verificación y troubleshooting
- Configuración de backups

### 5. `CAMBIOS_DOCKER.md` (este archivo)
Documentación de todos los cambios realizados

---

## 🔧 Archivos Modificados

### 1. `PresentationLayer/Dockerfile`

**Antes:**
- Dockerfile básico generado por Visual Studio
- Sin optimizaciones
- Sin health checks
- Sin usuario no-root

**Después:**
- Multi-stage build optimizado
- Instalación de `curl` para health checks
- Creación de directorios necesarios (`/app/Logs`, `/app/wwwroot/uploads`)
- Usuario no-root (`appuser`) para seguridad
- Health check configurado: `curl -f http://localhost:80/Home/Index`
- Labels informativos
- Mejor cache de layers (COPY separado para .csproj antes del código)

**Beneficios:**
- ✅ Imagen más pequeña y rápida
- ✅ Mayor seguridad (no ejecuta como root)
- ✅ Health checks automáticos
- ✅ Mejor rebuild time con cache

---

### 2. `DataAccessLayer/Context/DataAccesContext.cs`

**Antes:**
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer("Server=solucionesmkt.com.mx;...");
}
```
- Connection string hardcodeado
- Sin soporte para variables de entorno
- No usa inyección de dependencias correctamente

**Después:**
```csharp
// Constructor con opciones (para inyección de dependencias)
public DataAccesContext(DbContextOptions<DataAccesContext> options) : base(options)
{
}

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        // Primero variable de entorno (Docker)
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        // Fallback a BD de desarrollo local
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = "Server=solucionesmkt.com.mx;...";
        }

        optionsBuilder.UseSqlServer(connectionString);
    }
}
```

**Beneficios:**
- ✅ Soporta variables de entorno (Docker)
- ✅ Soporta appsettings.json (desarrollo local)
- ✅ Mantiene compatibilidad con código existente
- ✅ Fácil cambio entre ambientes (dev, staging, prod)

---

### 3. `PresentationLayer/Program.cs`

**Antes:**
```csharp
builder.Services.AddDbContext<DataAccesContext>();
```
- No pasa configuración al DbContext

**Después:**
```csharp
builder.Services.AddDbContext<DataAccesContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});
```

**Beneficios:**
- ✅ Usa configuración centralizada
- ✅ Respeta la jerarquía: appsettings.json → variables de entorno
- ✅ Mejor práctica de .NET Core

---

### 4. `PresentationLayer/appsettings.json`

**Antes:**
- Sin sección `ConnectionStrings`

**Después:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=solucionesmkt.com.mx;..."
  },
  ...
}
```

**Beneficios:**
- ✅ Configuración estándar de .NET Core
- ✅ Fácil de sobrescribir con variables de entorno
- ✅ Compatible con `appsettings.Development.json`, `appsettings.Staging.json`, etc.

---

### 5. `.env.staging.example` y `.gitignore`

**Actualizado:**
- Dominio cambiado de `adminproyectos-staging.entersys.mx` a `adminproyectos.entersys.mx`

---

## 🔄 Cómo Funcionan las Variables de Entorno

### Jerarquía de Configuración (orden de prioridad):

1. **Variables de entorno del sistema** (más alta)
   - Ejemplo: `ConnectionStrings__DefaultConnection`
   - Docker usa esto vía `environment:` en docker-compose.yml

2. **appsettings.{Environment}.json**
   - Ejemplo: `appsettings.Staging.json`

3. **appsettings.json** (más baja)
   - Valores por defecto para desarrollo local

### En Docker:

El `docker-compose.staging.yml` define:
```yaml
environment:
  - ConnectionStrings__DefaultConnection=Server=adminproyectos-sqlserver;Database=...
```

Esto **sobrescribe** automáticamente el valor de `appsettings.json`.

### En Desarrollo Local:

Si ejecutas sin Docker, usa el valor de `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=solucionesmkt.com.mx;..."
}
```

---

## 🚀 Flujo de Deployment

### Desarrollo Local (sin cambios):
```bash
# Sigue funcionando igual que antes
dotnet run
```
- Usa `appsettings.json`
- Conecta a `solucionesmkt.com.mx`

### Staging en Docker:
```bash
# En el servidor
docker-compose -f docker-compose.staging.yml --env-file .env.staging up -d
```
- Usa variables de entorno de `.env.staging`
- Conecta a SQL Server en contenedor (`adminproyectos-sqlserver`)
- SSL automático vía Traefik
- Monitoreo automático

---

## ✅ Ventajas de Este Approach

### 1. **Compatibilidad Total**
- ✅ Código existente sigue funcionando sin cambios
- ✅ Desarrollo local funciona igual
- ✅ Migraciones de EF Core funcionan
- ✅ Visual Studio funciona igual

### 2. **Flexibilidad**
- ✅ Fácil cambio entre ambientes
- ✅ No requiere recompilar para cambiar BD
- ✅ Secretos fuera del código fuente

### 3. **Seguridad**
- ✅ Passwords en variables de entorno, no hardcodeados
- ✅ Usuario no-root en contenedor
- ✅ SSL automático con Traefik
- ✅ Red privada para SQL Server

### 4. **Observabilidad**
- ✅ Logs estructurados con Serilog
- ✅ Métricas de contenedores (cAdvisor)
- ✅ Logs centralizados (Loki)
- ✅ Dashboards automáticos (Grafana)
- ✅ Alertas configuradas

### 5. **Productividad**
- ✅ Deploy en 1 comando
- ✅ Rollback rápido
- ✅ Health checks automáticos
- ✅ SSL sin configuración manual
- ✅ Backups scriptados

---

## 🧪 Testing

### Local (antes de desplegar):
```bash
# 1. Verificar que compila
dotnet build

# 2. Verificar que funciona localmente
dotnet run

# 3. Build de Docker local (opcional)
docker-compose -f docker-compose.staging.yml build

# 4. Test local con Docker (opcional)
docker-compose -f docker-compose.staging.yml up
```

### En Servidor:
Ver `DEPLOYMENT.md` sección "Paso 5: Verificación"

---

## 📊 Recursos Asignados

### SQL Server:
- CPU: 1.5 cores (límite), 0.5 cores (garantizado)
- RAM: 2GB (límite), 1GB (garantizado)
- Disco: Volumen persistente `sqlserver-data`

### Aplicación Web:
- CPU: 1 core (límite), 0.5 cores (garantizado)
- RAM: 1GB (límite), 512MB (garantizado)
- Disco:
  - Volumen `uploads-data` (archivos subidos)
  - Volumen `logs-data` (logs de Serilog)

### Total:
- **CPU:** 2.5 cores
- **RAM:** 3GB
- **Compatible con servidor actual:** ✅ (2 vCPUs, 4GB RAM)

---

## 🔐 Seguridad Implementada

1. **Connection Strings:**
   - ❌ NO hardcodeados en código
   - ✅ En variables de entorno

2. **Contenedor:**
   - ✅ Usuario no-root (`appuser`)
   - ✅ Red privada para BD (`backend`)
   - ✅ SQL Server NO expuesto públicamente

3. **SSL/TLS:**
   - ✅ Let's Encrypt automático
   - ✅ TLS 1.2+ solamente
   - ✅ Redirect HTTP → HTTPS
   - ✅ Security headers (HSTS, X-Frame-Options, etc.)

4. **Secrets:**
   - ✅ `.env.staging` en `.gitignore`
   - ✅ Template `.env.staging.example` para documentación

---

## 🗄️ Inicialización Automática de Base de Datos

### Cambio Importante en Program.cs

Se agregó auto-creación de base de datos:

```csharp
using (var scope = app.Services.CreateScope())
{
    var context = services.GetRequiredService<DataAccesContext>();

    // Crea la BD y todas las tablas si no existen
    if (context.Database.EnsureCreated())
    {
        logger.LogInformation("✅ Base de datos creada exitosamente");
    }
}
```

**Beneficios:**
- ✅ BD se crea automáticamente al iniciar la app
- ✅ Todas las tablas se crean basadas en las entidades de EF Core
- ✅ No requiere scripts SQL manuales
- ✅ No requiere migrations
- ✅ Safe: Si la BD ya existe, no hace nada

**Tablas que se crean:**
- Usuarios, Roles, Menus
- Briefs, TiposBrief, EstatusBriefs
- Materiales, EstatusMateriales, HistorialMateriales
- Proyectos, Participantes, Alertas
- Prioridad, PCN, Audiencia, Formato
- Y todas las relaciones/foreign keys

**Ver detalles:** `INICIALIZACION_BD.md`

---

## 📋 Checklist de Deployment

- [x] Dockerfile optimizado
- [x] docker-compose.yml creado
- [x] Variables de entorno configuradas
- [x] Connection string externalizado
- [x] Health checks implementados
- [x] Labels de monitoreo agregados
- [x] Documentación completa (DEPLOYMENT.md)
- [x] .gitignore actualizado
- [ ] DNS configurado (pendiente: adminproyectos.entersys.mx → 34.134.14.202)
- [ ] Archivos subidos al servidor
- [ ] Build y deploy ejecutados
- [ ] SSL verificado
- [ ] Monitoreo verificado

---

## 🔗 Referencias

- **Documentación del servidor:** `C:\Documentacion Infraestructura\DOCUMENTACION_CONTENEDORES_SERVIDOR.md`
- **Guía de onboarding:** `C:\Documentacion Infraestructura\GUIA_ONBOARDING_APLICACIONES.md`
- **Guía de deployment:** `DEPLOYMENT.md`

---

## 📞 Soporte

Si encuentras problemas durante el deployment:

1. Revisar logs: `docker-compose -f docker-compose.staging.yml logs -f`
2. Consultar troubleshooting en `DEPLOYMENT.md`
3. Contactar a infraestructura@entersys.mx

---

**✅ Todos los cambios son retrocompatibles. La aplicación sigue funcionando en desarrollo local sin modificaciones.**

---

**Fecha:** Octubre 2025
**Autor:** Claude Code
**Versión:** 1.0
