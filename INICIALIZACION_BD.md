# 🗄️ Inicialización Automática de Base de Datos

## 📋 Resumen

La aplicación AdminProyectos está configurada para **crear automáticamente** la base de datos y todas las tablas cuando se despliega en Docker por primera vez.

---

## ✅ ¿Cómo Funciona?

### 1. **Auto-creación al iniciar** (Program.cs)

Cuando la aplicación inicia, ejecuta este código automáticamente:

```csharp
using (var scope = app.Services.CreateScope())
{
    var context = services.GetRequiredService<DataAccesContext>();

    // Crea la BD y todas las tablas basadas en las entidades de EF Core
    if (context.Database.EnsureCreated())
    {
        logger.LogInformation("✅ Base de datos creada exitosamente");
    }
}
```

### 2. **Qué hace `EnsureCreated()`:**

- ✅ Verifica si la base de datos existe
- ✅ Si NO existe: La crea automáticamente
- ✅ Crea TODAS las tablas basadas en las entidades (DbSet) definidas en `DataAccesContext`
- ✅ Aplica las relaciones y constraints configurados en `OnModelCreating()`
- ✅ Si YA existe: No hace nada (safe)

### 3. **Tablas que se crean automáticamente:**

Basado en tu `DataAccesContext.cs`, se crearán:

```
✅ Usuarios
✅ Alertas
✅ Briefs
✅ EstatusBriefs
✅ Menus
✅ Roles
✅ TiposBrief
✅ Materiales
✅ Proyectos
✅ Participantes
✅ EstatusMateriales
✅ TipoAlerta
✅ RetrasoMateriales
✅ HistorialMateriales
✅ Prioridad
✅ PCN
✅ Audiencia
✅ Formato
```

Con todas sus relaciones, foreign keys, índices, etc.

---

## 🚀 Proceso de Deployment

### Flujo Completo:

```
1. docker-compose up -d
   ↓
2. SQL Server inicia (contenedor adminproyectos-sqlserver)
   ↓ (espera hasta que esté healthy - 90 segundos)
3. Aplicación Web inicia (contenedor adminproyectos-web)
   ↓
4. Program.cs ejecuta auto-creación
   ↓
5. EF Core crea base de datos "AdminProyectosNaturaDB"
   ↓
6. EF Core crea todas las tablas
   ↓
7. ✅ Aplicación lista para usarse
```

### Tiempos:
- **SQL Server ready:** ~60-90 segundos
- **Creación de BD y tablas:** ~10-20 segundos
- **Total:** ~2 minutos desde `docker-compose up`

---

## 🔍 Verificación

### Ver logs de creación:

```bash
# Logs de la aplicación (verás mensajes de creación de BD)
docker logs natura-adminproyectos-web

# Buscar mensajes específicos:
docker logs natura-adminproyectos-web | grep "Base de datos"

# Deberías ver:
# [10:30:45 INF] Verificando estado de la base de datos...
# [10:30:47 INF] ✅ Base de datos creada exitosamente con todas las tablas
```

### Verificar BD desde SQL Server:

```bash
# Conectarse a SQL Server
docker exec -it natura-adminproyectos-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "NaturaAdmin2025!Secure#"

# Listar bases de datos
> SELECT name FROM sys.databases;
> GO

# Deberías ver: AdminProyectosNaturaDB

# Cambiar a la BD
> USE AdminProyectosNaturaDB;
> GO

# Listar tablas
> SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;
> GO

# Salir
> exit
```

---

## 📊 ¿Qué Pasa en Diferentes Escenarios?

### Escenario 1: Primera vez (BD no existe)
```
✅ Se crea "AdminProyectosNaturaDB"
✅ Se crean todas las tablas
✅ BD lista para usar
```

### Escenario 2: Reiniciar contenedores (BD ya existe en volumen)
```
ℹ️ BD ya existe (mensaje en logs)
✅ No se toca nada
✅ Datos preservados
```

### Escenario 3: Eliminar volumen y recrear
```bash
docker-compose down -v  # Elimina volúmenes
docker-compose up -d    # Recrea todo

✅ Se vuelve a crear BD desde cero
✅ BD vacía, lista para usar
```

---

## 🔄 Datos Iniciales (Seeds)

Si necesitas datos iniciales (usuarios admin, catálogos, etc.), tienes dos opciones:

### Opción A: Seed en el código (Recomendado)

Agregar en `Program.cs` después de `EnsureCreated()`:

```csharp
if (context.Database.EnsureCreated())
{
    logger.LogInformation("✅ Base de datos creada, insertando datos iniciales...");

    // Seed de roles
    context.Roles.AddRange(
        new Rol { Id = 1, Nombre = "Administrador", Activo = true },
        new Rol { Id = 2, Nombre = "Usuario", Activo = true },
        new Rol { Id = 3, Nombre = "Cliente", Activo = true }
    );

    // Seed de usuario admin
    context.Usuarios.Add(new Usuario
    {
        Nombre = "Admin",
        Correo = "admin@natura.com",
        Password = "hashed_password_here",
        RolId = 1,
        Activo = true
    });

    context.SaveChanges();
    logger.LogInformation("✅ Datos iniciales insertados");
}
```

### Opción B: Script SQL separado

Crear `database/seed.sql`:

```sql
USE AdminProyectosNaturaDB;
GO

-- Roles
INSERT INTO Roles (Id, Nombre, Activo) VALUES
(1, 'Administrador', 1),
(2, 'Usuario', 1),
(3, 'Cliente', 1);

-- Usuario admin por defecto
INSERT INTO Usuarios (Nombre, Correo, Password, RolId, Activo) VALUES
('Admin', 'admin@natura.com', 'hashed_password', 1, 1);

-- Estatus de Briefs
INSERT INTO EstatusBriefs (Nombre) VALUES
('Pendiente'),
('En Proceso'),
('Completado'),
('Cancelado');

-- Prioridades
INSERT INTO Prioridad (Nombre) VALUES
('Alta'),
('Media'),
('Baja');

GO
```

Ejecutar después del deploy:

```bash
# Copiar script al contenedor
docker cp database/seed.sql natura-adminproyectos-sqlserver:/tmp/

# Ejecutar
docker exec -it natura-adminproyectos-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "${SQL_SA_PASSWORD}" -i /tmp/seed.sql
```

---

## 🔐 Seguridad

### Connection String en Docker:

La aplicación usa esta configuración:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Server=adminproyectos-sqlserver;Database=AdminProyectosNaturaDB;User Id=sa;Password=${SQL_SA_PASSWORD};TrustServerCertificate=True;
```

**Importante:**
- ✅ Password viene de variable de entorno `.env.staging`
- ✅ SQL Server NO está expuesto públicamente (red `backend` privada)
- ✅ Solo la app puede acceder al SQL Server
- ✅ Traefik solo expone el puerto 80 de la app

---

## ⚠️ Notas Importantes

### 1. **Volumen Persistente**

La base de datos se guarda en un volumen Docker:

```yaml
volumes:
  sqlserver-data:
    driver: local
```

**Esto significa:**
- ✅ Los datos persisten entre reinicios de contenedores
- ✅ `docker-compose restart` → Datos intactos
- ✅ `docker-compose down` → Datos intactos
- ❌ `docker-compose down -v` → **ELIMINA TODOS LOS DATOS**

### 2. **Backups**

**IMPORTANTE:** Configurar backups automáticos (ver `DEPLOYMENT.md` sección "Paso 8: Backups")

```bash
# Backup manual
docker exec natura-adminproyectos-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "${SQL_SA_PASSWORD}" \
  -Q "BACKUP DATABASE AdminProyectosNaturaDB TO DISK = N'/var/opt/mssql/backup/AdminProyectos_$(date +%Y%m%d).bak'"

# Copiar backup fuera del contenedor
docker cp natura-adminproyectos-sqlserver:/var/opt/mssql/backup/ ./backups/
```

### 3. **Migraciones Futuras**

Si agregas nuevas tablas o cambios al esquema:

**Opción A: Recrear BD (solo staging/dev)**
```bash
docker-compose down -v  # Elimina volumen
docker-compose up -d    # Recrea con nuevo esquema
```

**Opción B: Usar EF Core Migrations (recomendado para producción)**
```bash
# Crear migration
dotnet ef migrations add NombreDeLaMigration --project DataAccessLayer --startup-project PresentationLayer

# Aplicar en el código
# Cambiar EnsureCreated() por Database.Migrate()
```

---

## 📋 Checklist de Verificación

Después del deployment:

- [ ] Contenedor SQL Server healthy
- [ ] Contenedor App healthy
- [ ] Logs muestran "Base de datos creada exitosamente"
- [ ] Puedes acceder a la app vía HTTPS
- [ ] BD "AdminProyectosNaturaDB" existe en SQL Server
- [ ] Todas las tablas fueron creadas
- [ ] (Opcional) Datos iniciales insertados
- [ ] Backup configurado

---

## 🐛 Troubleshooting

### Problema: "Cannot connect to SQL Server"

**Causa:** SQL Server aún no está listo

**Solución:**
```bash
# Ver logs de SQL Server
docker logs natura-adminproyectos-sqlserver

# Esperar mensaje: "SQL Server is now ready for client connections"
# Puede tomar hasta 90 segundos
```

### Problema: "Database already exists" (error)

**Causa:** Intentando crear BD que ya existe

**Solución:**
- Si es desarrollo/staging: `docker-compose down -v && docker-compose up -d`
- Si es producción: Verificar que no haya BD previa o usar migrations

### Problema: "Tablas no se crearon"

**Causa:** Error en definición de entidades

**Solución:**
```bash
# Ver logs detallados de la app
docker logs natura-adminproyectos-web --tail 100

# Verificar errores de EF Core
```

---

## 📚 Referencias

- **Código de auto-creación:** `PresentationLayer/Program.cs` (líneas ~102-131)
- **Definición de tablas:** `DataAccessLayer/Context/DataAccesContext.cs`
- **Configuración Docker:** `docker-compose.staging.yml`

---

**✅ Con esta configuración, NO necesitas crear manualmente la base de datos. Todo es automático al hacer `docker-compose up`.**

---

**Fecha:** Octubre 2025
**Autor:** Claude Code
