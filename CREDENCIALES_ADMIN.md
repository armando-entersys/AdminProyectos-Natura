# 🔐 Credenciales de Usuario Administrador

## Usuario Admin Creado Automáticamente

Cuando despliegues la aplicación por primera vez en Docker, se creará automáticamente un usuario administrador con las siguientes credenciales:

---

## 📧 Credenciales

| Campo | Valor |
|-------|-------|
| **Correo** | `ajcortest@gmail.com` |
| **Contraseña** | `Operaciones.2025` |
| **Rol** | Administrador |
| **Estado** | Activo |

---

## 🎯 Acceso

1. Ir a: https://adminproyectos.entersys.mx
2. Hacer clic en "Iniciar Sesión"
3. Ingresar credenciales:
   - Email: `ajcortest@gmail.com`
   - Contraseña: `Operaciones.2025`

---

## ⚙️ Detalles Técnicos

### Creación Automática

El usuario se crea automáticamente cuando la aplicación inicia por primera vez y detecta que la base de datos está vacía.

**Ubicación del código:** `PresentationLayer/Program.cs` (líneas ~120-150)

### Datos del Usuario

```csharp
Nombre: "Admin"
ApellidoPaterno: "Sistema"
ApellidoMaterno: ""
Correo: "ajcortest@gmail.com"
Contrasena: "Operaciones.2025"
Rol: "Administrador"
Estatus: true (activo)
```

### Verificación

Para verificar que el usuario fue creado, ver los logs:

```bash
docker logs natura-adminproyectos-web | grep "Datos iniciales"

# Deberías ver:
# [INFO] Insertando datos iniciales...
# [INFO] ✅ Datos iniciales insertados: Usuario admin creado
```

O consultar la base de datos:

```bash
docker exec -it natura-adminproyectos-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "${SQL_SA_PASSWORD}"

> USE AdminProyectosNaturaDB;
> SELECT * FROM Usuarios WHERE Correo = 'ajcortest@gmail.com';
> GO
```

---

## 🔒 Seguridad

### ⚠️ IMPORTANTE: Cambiar Contraseña

**Después del primer login, SE RECOMIENDA CAMBIAR LA CONTRASEÑA** desde el panel de administración.

### Consideraciones de Seguridad

1. **Contraseña en texto plano:**
   - Actualmente la contraseña se guarda sin hash
   - Para producción, considerar implementar hash (BCrypt, Argon2, etc.)

2. **Credenciales por defecto:**
   - Todos los deployments usan las mismas credenciales
   - Cambiar inmediatamente después del primer acceso

3. **Acceso de administrador:**
   - Este usuario tiene permisos completos
   - Usar solo para configuración inicial
   - Crear usuarios específicos para operación diaria

---

## 🔄 Recrear Usuario Admin

Si eliminas la base de datos y la recreas:

```bash
# Eliminar volúmenes (CUIDADO: elimina todos los datos)
docker-compose -f docker-compose.staging.yml down -v

# Recrear todo
docker-compose -f docker-compose.staging.yml --env-file .env.staging up -d

# El usuario admin se creará automáticamente de nuevo
```

---

## 📝 Personalización

Si necesitas cambiar las credenciales por defecto, editar `PresentationLayer/Program.cs`:

```csharp
var usuarioAdmin = new Usuario
{
    Nombre = "Admin",
    ApellidoPaterno = "Sistema",
    ApellidoMaterno = "",
    Correo = "tu-email@ejemplo.com",  // ← Cambiar aquí
    Contrasena = "TuPasswordSeguro",   // ← Cambiar aquí
    RolId = rolAdmin.Id,
    Estatus = true,
    // ...
};
```

Luego recompilar y redesplegar.

---

## 🚨 Troubleshooting

### Problema: No puedo iniciar sesión

**Verificar:**
1. Usuario existe en BD (ver comandos arriba)
2. Correo y contraseña son exactos (case-sensitive)
3. Usuario está activo (Estatus = true)
4. Rol existe y es Administrador

### Problema: Usuario no se creó

**Verificar logs:**
```bash
docker logs natura-adminproyectos-web | grep -A 5 "Base de datos"
```

**Posibles causas:**
- BD ya existía (usuario solo se crea en BD nueva)
- Error durante la creación (ver logs completos)

**Solución:**
Crear usuario manualmente desde SQL Server o recrear BD completamente.

---

**⚠️ RECORDATORIO: Cambiar la contraseña después del primer login por razones de seguridad.**

---

**Fecha de creación:** Octubre 2025
**Versión:** 1.0
