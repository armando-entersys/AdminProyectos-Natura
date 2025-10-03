# 🚀 Guía de Despliegue - AdminProyectos NATURA (Staging)

## 📋 Información General

- **Aplicación:** AdminProyectos NATURA
- **Versión:** 1.0.0
- **Tecnología:** ASP.NET Core 6.0 + SQL Server 2022
- **Ambiente:** Staging
- **Servidor:** dev-server (GCP 34.134.14.202)
- **Stack:** Traefik + Docker + Prometheus/Grafana

---

## ✅ Pre-requisitos

### En el Servidor
- [x] Docker y Docker Compose instalados
- [x] Red `traefik` creada
- [x] Traefik funcionando
- [x] Stack de monitoreo (Prometheus/Grafana/Loki)

### Localmente
- [x] Git
- [x] Acceso SSH al servidor: `gcloud compute ssh dev-server --zone=us-central1-c`

---

## 📦 Componentes de la Aplicación

### 1. Base de Datos
- **Imagen:** `mcr.microsoft.com/mssql/server:2022-latest`
- **Edición:** Express (gratuita, hasta 10GB)
- **Puerto:** 1433 (solo interno)
- **Recursos:** 1.5 CPU, 2GB RAM

### 2. Aplicación Web
- **Framework:** ASP.NET Core 6.0
- **Puerto:** 80 (HTTP interno)
- **SSL:** Automático vía Traefik (Let's Encrypt)
- **Recursos:** 1 CPU, 1GB RAM

### 3. Volúmenes Persistentes
- `sqlserver-data`: Base de datos SQL Server
- `uploads-data`: Archivos subidos por usuarios
- `logs-data`: Logs de la aplicación (Serilog)

---

## 🔧 Paso 1: Configuración Inicial

### 1.1 Clonar el repositorio en el servidor

```bash
# Conectarse al servidor
gcloud compute ssh dev-server --zone=us-central1-c

# Crear directorio de servicios
sudo mkdir -p /srv/servicios/natura-adminproyectos
cd /srv/servicios/natura-adminproyectos

# Clonar repositorio (o subir archivos)
git clone [URL_DEL_REPO] .
# O usando SCP/SFTP para subir archivos
```

### 1.2 Configurar variables de entorno

```bash
# Copiar plantilla de configuración
cp .env.staging.example .env.staging

# Editar configuración
nano .env.staging
```

**Configurar los siguientes valores:**

```bash
# Dominio
DOMAIN_NAME=adminproyectos.entersys.mx

# SQL Server Password (IMPORTANTE: cambiar este password)
SQL_SA_PASSWORD=NaturaAdmin2025!Secure#

# Email/SMTP (ajustar si es necesario)
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SENDER_EMAIL=no-reply@natura.com
SMTP_USERNAME=ajcortest@gmail.com
SMTP_PASSWORD=wfqbvzuiwzrwlpeg
```

**⚠️ IMPORTANTE:** Nunca commitear `.env.staging` a Git

---

## 🌐 Paso 2: Configuración de DNS

El dominio debe apuntar al servidor:

```dns
Tipo: A
Host: adminproyectos
Valor: 34.134.14.202
TTL: 300
```

**Verificar DNS:**
```bash
nslookup adminproyectos.entersys.mx
# Debe devolver: 34.134.14.202
```

---

## 🐳 Paso 3: Despliegue con Docker

### 3.1 Crear red de Traefik (si no existe)

```bash
docker network create traefik
```

### 3.2 Build de la aplicación

```bash
# Desde /srv/servicios/natura-adminproyectos
docker-compose -f docker-compose.staging.yml build --no-cache
```

**⏱️ Tiempo estimado:** 5-10 minutos (primera vez)

### 3.3 Iniciar servicios

```bash
# Iniciar en background
docker-compose -f docker-compose.staging.yml --env-file .env.staging up -d

# Ver logs en tiempo real
docker-compose -f docker-compose.staging.yml logs -f
```

### 3.4 Verificar estado

```bash
# Ver contenedores corriendo
docker ps | grep natura-adminproyectos

# Debería mostrar:
# - natura-adminproyectos-sqlserver (healthy)
# - natura-adminproyectos-web (healthy)
```

---

## 🗄️ Paso 4: Inicializar Base de Datos

### ✅ AUTO-CREACIÓN AUTOMÁTICA (Recomendado)

**La base de datos se crea automáticamente** cuando la aplicación inicia por primera vez.

No necesitas hacer NADA manualmente. El proceso es:

```
1. SQL Server inicia y queda healthy
2. Aplicación inicia
3. Program.cs detecta que BD no existe
4. EF Core crea "AdminProyectosNaturaDB"
5. EF Core crea todas las tablas automáticamente
6. ✅ Aplicación lista
```

**Ver logs de creación:**
```bash
docker logs natura-adminproyectos-web | grep "Base de datos"

# Deberías ver:
# [INFO] Verificando estado de la base de datos...
# [INFO] ✅ Base de datos creada exitosamente con todas las tablas
```

**Ver detalles completos:** Consulta `INICIALIZACION_BD.md`

---

### 🔄 Opciones Alternativas (solo si necesitas control manual)

#### Opción A: Restaurar Backup Existente

Si tienes un backup de datos existentes:

```bash
# Copiar backup al contenedor
docker cp AdminProyectosNaturaDB.bak natura-adminproyectos-sqlserver:/var/opt/mssql/backup/

# Restaurar
docker exec -it natura-adminproyectos-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "${SQL_SA_PASSWORD}" \
  -Q "RESTORE DATABASE AdminProyectosNaturaDB FROM DISK = '/var/opt/mssql/backup/AdminProyectosNaturaDB.bak' WITH REPLACE"
```

#### Opción B: Insertar Datos Iniciales (Seeds)

Si necesitas datos de prueba o catálogos iniciales, ver `INICIALIZACION_BD.md` sección "Datos Iniciales (Seeds)"

---

## ✅ Paso 5: Verificación

### 5.1 Health Checks

```bash
# Health check de la app
curl http://localhost/Home/Index
# Debe devolver: 200 OK

# Health check del SQL Server
docker exec natura-adminproyectos-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "TuPasswordSeguro123!" -Q "SELECT 1"
# Debe devolver: 1
```

### 5.2 Traefik Detection

```bash
# Verificar que Traefik detectó la app
curl -s http://localhost:8080/api/http/routers | jq '.[] | select(.name | contains("adminproyectos"))'
```

### 5.3 SSL/HTTPS

```bash
# Verificar certificado SSL
curl -I https://adminproyectos.entersys.mx

# Debe devolver: HTTP/2 200
# Con certificado Let's Encrypt válido
```

### 5.4 Logs

```bash
# Logs de la aplicación
docker logs natura-adminproyectos-web --tail 50

# Logs de SQL Server
docker logs natura-adminproyectos-sqlserver --tail 50

# Logs en tiempo real
docker-compose -f docker-compose.staging.yml logs -f
```

---

## 📊 Paso 6: Monitoreo

### 6.1 Grafana

Acceder a: https://monitoring.entersys.mx

**Dashboards disponibles:**
- **Container Metrics:** CPU, Memoria, Red de los contenedores
- **Logs:** Logs centralizados de la aplicación
- **Alertas:** Configuración de alertas automáticas

### 6.2 Verificar Métricas

```bash
# Ver métricas de contenedores
docker stats natura-adminproyectos-web natura-adminproyectos-sqlserver
```

### 6.3 Loki (Logs Centralizados)

En Grafana, buscar:
```logql
{container_name="natura-adminproyectos-web"}
```

---

## 🔄 Paso 7: Actualización de la Aplicación

Para hacer un nuevo deploy:

```bash
# 1. Pull del nuevo código
cd /srv/servicios/natura-adminproyectos
git pull

# 2. Rebuild
docker-compose -f docker-compose.staging.yml build --no-cache

# 3. Recrear contenedores (sin downtime)
docker-compose -f docker-compose.staging.yml --env-file .env.staging up -d --force-recreate

# 4. Ver logs
docker-compose -f docker-compose.staging.yml logs -f adminproyectos-web
```

---

## 💾 Paso 8: Backups

### 8.1 Backup Automático de Base de Datos

Agregar cron job para backup diario:

```bash
# Crear script de backup
sudo nano /srv/scripts/backup-adminproyectos-db.sh
```

```bash
#!/bin/bash
BACKUP_DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/srv/backups/adminproyectos"
mkdir -p $BACKUP_DIR

echo "🔄 Backing up AdminProyectos DB - $BACKUP_DATE"

docker exec natura-adminproyectos-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "$SQL_SA_PASSWORD" \
  -Q "BACKUP DATABASE AdminProyectosNaturaDB TO DISK = N'/var/opt/mssql/backup/AdminProyectos_$BACKUP_DATE.bak' WITH NOFORMAT, NOINIT, NAME = 'AdminProyectos-full', SKIP, NOREWIND, NOUNLOAD, STATS = 10"

docker cp natura-adminproyectos-sqlserver:/var/opt/mssql/backup/AdminProyectos_$BACKUP_DATE.bak $BACKUP_DIR/

echo "✅ Backup completado: $BACKUP_DIR/AdminProyectos_$BACKUP_DATE.bak"

# Limpiar backups antiguos (mantener últimos 7 días)
find $BACKUP_DIR -name "AdminProyectos_*.bak" -mtime +7 -delete
```

```bash
# Permisos
sudo chmod +x /srv/scripts/backup-adminproyectos-db.sh

# Cron job (diario a las 2 AM)
sudo crontab -e
0 2 * * * /srv/scripts/backup-adminproyectos-db.sh >> /var/log/backup-adminproyectos.log 2>&1
```

### 8.2 Backup de Archivos Subidos

```bash
# Backup de uploads
tar -czf adminproyectos-uploads-$(date +%Y%m%d).tar.gz \
  /var/lib/docker/volumes/natura-adminproyectos_uploads-data
```

---

## 🔧 Troubleshooting

### Problema: Contenedor no inicia

```bash
# Ver logs detallados
docker-compose -f docker-compose.staging.yml logs adminproyectos-web

# Causas comunes:
# 1. Variables de entorno faltantes
# 2. SQL Server no listo (esperar 60s)
# 3. Permisos de volúmenes
```

### Problema: No conecta a la base de datos

```bash
# Verificar SQL Server está corriendo
docker exec natura-adminproyectos-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "TuPasswordSeguro123!" -Q "SELECT @@VERSION"

# Verificar conexión desde la app
docker exec natura-adminproyectos-web cat /app/appsettings.json | grep ConnectionStrings
```

### Problema: SSL no funciona

```bash
# Ver logs de Traefik
docker logs traefik | grep -i adminproyectos

# Verificar labels
docker inspect natura-adminproyectos-web | grep -A 30 Labels

# Causas comunes:
# 1. DNS no propagado
# 2. Puerto 80 no accesible
# 3. Rate limit de Let's Encrypt
```

### Problema: Archivos subidos se pierden

```bash
# Verificar volumen
docker volume inspect natura-adminproyectos_uploads-data

# Verificar permisos dentro del contenedor
docker exec natura-adminproyectos-web ls -la /app/wwwroot/uploads
```

---

## 📞 Soporte

**Equipo de Infraestructura EnterSys:**
- Email: infraestructura@entersys.mx
- Slack: #infraestructura

**Documentación adicional:**
- Servidor: `C:\Documentacion Infraestructura\DOCUMENTACION_CONTENEDORES_SERVIDOR.md`
- Onboarding: `C:\Documentacion Infraestructura\GUIA_ONBOARDING_APLICACIONES.md`

---

## 🎯 Checklist Final

- [ ] Variables de entorno configuradas (`.env.staging`)
- [ ] DNS apuntando al servidor
- [ ] Contenedores corriendo y healthy
- [ ] Base de datos inicializada
- [ ] SSL activo y funcionando
- [ ] Health checks pasando
- [ ] Monitoreo visible en Grafana
- [ ] Logs fluyendo a Loki
- [ ] Backup automático configurado
- [ ] Documentación actualizada
- [ ] Equipo notificado del deployment

---

**🎉 ¡Deployment completado!**

Accede a tu aplicación en: **https://adminproyectos.entersys.mx**

---

**Fecha de creación:** Octubre 2025
**Versión:** 1.0
**Mantenedor:** Equipo DevOps NATURA
