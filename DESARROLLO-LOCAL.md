# 🚀 Desarrollo Local con Docker

Esta guía te ayudará a trabajar localmente con Docker antes de desplegar al servidor.

## 📋 Requisitos

- Docker Desktop instalado y corriendo
- Git
- Puerto 8080 disponible (aplicación web)
- Puerto 1433 disponible (SQL Server) - opcional

## 🔧 Configuración Inicial

### 1. Levantar los contenedores localmente

```bash
# En la raíz del proyecto
docker-compose up -d
```

Este comando:
- ✅ Construye la imagen de la aplicación
- ✅ Crea el contenedor de SQL Server
- ✅ Crea la base de datos y ejecuta el seed
- ✅ Levanta la aplicación en http://localhost:8080

### 2. Ver los logs

```bash
# Ver logs de la aplicación
docker logs local-adminproyectos-web -f

# Ver logs de SQL Server
docker logs local-adminproyectos-sqlserver -f
```

### 3. Acceder a la aplicación

Abre tu navegador en: **http://localhost:8080**

**Credenciales:**
- Email: `ajcortest@gmail.com`
- Contraseña: `Operaciones.2025`

## 🔄 Flujo de Trabajo

### 1. Hacer cambios en el código

Edita los archivos en:
- `PresentationLayer/` - Vistas, controladores, JavaScript, CSS
- `BusinessLayer/` - Lógica de negocio
- `DataAccessLayer/` - Acceso a datos
- `EntityLayer/` - Entidades

### 2. Reconstruir la imagen y probar

```bash
# Detener contenedores
docker-compose down

# Reconstruir imagen sin caché
docker-compose build --no-cache

# Levantar de nuevo
docker-compose up -d

# Ver logs
docker logs local-adminproyectos-web -f
```

### 3. Verificar que todo funcione correctamente

- ✅ La aplicación carga sin errores
- ✅ El login funciona
- ✅ Los menús del sidebar se muestran
- ✅ Los catálogos funcionan correctamente
- ✅ No hay errores 404 en las rutas

### 4. Una vez todo funcione, hacer commit y push

```bash
# Agregar cambios
git add .

# Hacer commit
git commit -m "Descripción de los cambios"

# Subir a GitHub
git push origin master
```

### 5. Desplegar en el servidor

Una vez que los cambios estén en GitHub y probados localmente:

```bash
# Conectar al servidor (desde tu máquina local)
gcloud compute ssh dev-server --zone=us-central1-c

# Una vez en el servidor, ir al directorio del proyecto
cd /srv/servicios/natura-adminproyectos

# Hacer pull de los cambios
sudo git pull origin master

# Reconstruir imagen
sudo docker-compose -f docker-compose.staging.yml --env-file .env.staging build --no-cache

# Recrear contenedores
sudo docker-compose -f docker-compose.staging.yml --env-file .env.staging up -d --force-recreate

# Ver logs
sudo docker logs natura-adminproyectos-web -f
```

## 🛠️ Comandos Útiles

### Gestión de contenedores

```bash
# Ver contenedores corriendo
docker ps

# Detener todos los contenedores
docker-compose down

# Eliminar contenedores y volúmenes (base de datos se borrará)
docker-compose down -v

# Reconstruir sin caché
docker-compose build --no-cache

# Levantar en modo detached
docker-compose up -d

# Ver logs en tiempo real
docker logs local-adminproyectos-web -f
```

### Acceso a SQL Server

```bash
# Conectar a SQL Server desde tu máquina
# Servidor: localhost,1433
# Usuario: sa
# Contraseña: Operaciones.2025
# Base de datos: AdminProyectosNaturaDB

# O usar Docker exec
docker exec -it local-adminproyectos-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -No -U sa -P "Operaciones.2025" -Q "USE AdminProyectosNaturaDB; SELECT * FROM Menus;"
```

### Limpiar todo

```bash
# Detener y eliminar contenedores, volúmenes, imágenes
docker-compose down -v --rmi all

# Limpiar el sistema Docker completo (cuidado, afecta todos los proyectos)
docker system prune -a --volumes
```

## 📝 Notas Importantes

1. **Base href:** El archivo local usa `<base href="/" />` igual que staging
2. **Contraseña BD:** Local usa `Operaciones.2025` (hardcodeada en docker-compose.yml)
3. **Puerto:** La app corre en puerto 8080 localmente: http://localhost:8080
4. **Seed:** La base de datos se crea automáticamente con todos los datos iniciales
5. **Volúmenes:** Los datos persisten entre reinicios. Usa `docker-compose down -v` para borrar todo y empezar de cero

## ⚠️ Troubleshooting

### La aplicación no carga
```bash
# Ver logs para identificar el error
docker logs local-adminproyectos-web -f
```

### SQL Server no inicia
```bash
# Verificar que el puerto 1433 no esté en uso
netstat -ano | findstr :1433

# Ver logs de SQL Server
docker logs local-adminproyectos-sqlserver -f
```

### Cambios no se reflejan
```bash
# Reconstruir sin caché
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

### Base de datos corrupta o con datos malos
```bash
# Eliminar volúmenes y recrear todo
docker-compose down -v
docker-compose up -d
```

## 🎯 Workflow Recomendado

1. **Desarrollo Local** ➔ Hacer cambios y probar en http://localhost:8080
2. **Commit** ➔ `git commit -m "mensaje"`
3. **Push** ➔ `git push origin master`
4. **Despliegue** ➔ Conectar al servidor y hacer pull + rebuild + recreate
5. **Verificación** ➔ Probar en https://adminproyectos.entersys.mx

¡Happy coding! 🎉
