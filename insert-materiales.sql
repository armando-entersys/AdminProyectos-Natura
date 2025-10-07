USE AdminProyectos;
GO

-- Crear materiales de prueba vinculados a los briefs existentes
INSERT INTO Materiales (Nombre, Mensaje, PrioridadId, Ciclo, PCNId, AudienciaId, FormatoId, FechaEntrega, Responsable, Area, FechaRegistro, FechaModificacion, BriefId, EstatusMaterialId)
VALUES
-- Materiales para hoy
('Banner Principal Navidad', 'Banner para campaña navideña', 3, 'C18', 1, 1, 6, GETDATE(), 'Juan Pérez', 'Diseño', GETDATE(), GETDATE(), 2, 2),
('Video Promocional Ekos', 'Video de lanzamiento producto', 4, 'C18', 3, 2, 1, GETDATE(), 'María López', 'Audiovisual', GETDATE(), GETDATE(), 3, 3),

-- Materiales para esta semana
('Post Facebook Ciclo 18', 'Contenido para redes sociales', 2, 'C18', 5, 1, 7, DATEADD(day, 3, GETDATE()), 'Pedro García', 'Marketing Digital', GETDATE(), GETDATE(), 4, 1),
('Infografía Productos', 'Infografía de nuevos productos', 3, 'C18', 1, 3, 5, DATEADD(day, 5, GETDATE()), 'Ana Martínez', 'Diseño', GETDATE(), GETDATE(), 5, 2),

-- Material para próxima semana
('Catálogo Digital', 'Catálogo de productos para diciembre', 2, 'C19', 4, 1, 4, DATEADD(day, 12, GETDATE()), 'Carlos Ruiz', 'Contenidos', GETDATE(), GETDATE(), 6, 1);
GO

SELECT Id, Nombre, FechaEntrega, EstatusMaterialId, BriefId FROM Materiales;
GO
