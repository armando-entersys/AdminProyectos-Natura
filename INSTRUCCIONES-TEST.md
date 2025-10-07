# Instrucciones para Probar el Kanban Localmente

La aplicación está corriendo en: http://localhost:8080

## Paso 1: Login

1. Abre tu navegador y ve a: http://localhost:8080/Login
2. Usa estas credenciales:
   - **Usuario**: `ajcortest@gmail.com`
   - **Contraseña**: `Operaciones.2025`

## Paso 2: Ir a Gestión de Proyectos

1. Una vez logueado, ve a: http://localhost:8080/Brief/IndexAdmin
2. **Abre la consola del navegador** presionando **F12**
3. Ve a la pestaña **Console**

## Paso 3: Ver los Logs de Depuración

En la consola deberías ver mensajes como:
- `GetAllColumns response: {...}`
- `Processing column: {...}`
- `Processing task: {...}`
- `Transformed columns: [...]`
- `Columns in observable: [...]`

## Paso 4: Reportar lo que Ves

Dime:
1. **¿Se muestran las columnas del Kanban?** (En revisión, Producción, Falta información, Programado)
2. **¿Cuántas tarjetas/proyectos se muestran en cada columna?**
3. **¿Qué dice el console.log?** (copia el texto de los mensajes)
4. **¿Hay algún error en la consola?** (texto en rojo)

## Información Adicional

Si ves que `GetAllColumns response` tiene:
- `datos: []` - significa que no hay proyectos en la base de datos
- `datos: [...]` con contenido - significa que hay datos pero no se están renderizando

Esto me ayudará a identificar el problema exacto.
