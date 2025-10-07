USE AdminProyectos;
GO

-- Insertar alertas de prueba
INSERT INTO Alertas (IdUsuario, Nombre, Descripcion, lectura, Accion, FechaCreacion, IdTipoAlerta)
VALUES
(1, 'Brief pendiente de revisión', 'El brief "Campaña Navidad 2024" requiere tu revisión', 0, '/Brief/IndexAdmin', GETDATE(), 1),
(1, 'Proyecto próximo a vencer', 'El proyecto "Lanzamiento Producto Ekos" vence en 3 días', 0, '/Brief/IndexAdmin', GETDATE(), 2),
(1, 'Falta información en brief', 'El brief "Brief Ciclo 18" requiere información adicional', 0, '/Brief/IndexAdmin', GETDATE(), 2),
(1, 'Material retrasado', 'Se ha reportado un retraso en material del ciclo 18', 0, '/Materiales/Index', GETDATE(), 3);
GO

SELECT Id, Nombre, Descripcion, lectura FROM Alertas;
GO
