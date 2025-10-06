USE AdminProyectos;
GO

INSERT INTO Briefs (UsuarioId, Nombre, Descripcion, Objetivo, DirigidoA, Comentario, FechaEntrega, EstatusBriefId, TipoBriefId, LinksReferencias, PlanComunicacion, DeterminarEstadoEstadoId, FechaPublicacion, FechaRegistro, FechaModificacion)
VALUES
(1, 'Campaña Navidad 2024', 'Campaña promocional para temporada navideña', 'Incrementar ventas en 30%', 'Público general', 'Revisar propuesta de diseño', '2024-12-20', 1, 1, 'https://ejemplo.com', 0, 1, GETDATE(), GETDATE(), GETDATE()),
(1, 'Lanzamiento Producto Ekos', 'Material para lanzamiento de nueva línea Ekos', 'Posicionar nueva línea', 'Millennials', 'Enfocarse en sustentabilidad', '2024-11-15', 2, 1, 'https://ejemplo.com', 0, 1, GETDATE(), GETDATE(), GETDATE()),
(1, 'Brief Ciclo 18', 'Materiales para inicio de ciclo 18', 'Activar consultoras', 'Red de consultoras', 'Urgente para inicio de ciclo', '2024-11-01', 3, 1, 'https://ejemplo.com', 0, 1, GETDATE(), GETDATE(), GETDATE()),
(1, 'Redes Sociales Diciembre', 'Contenido para redes sociales mes de diciembre', 'Engagement en redes', 'Seguidores de marca', 'Incluir calendario de publicaciones', '2024-12-01', 4, 1, 'https://ejemplo.com', 0, 1, GETDATE(), GETDATE(), GETDATE()),
(1, 'Video Institucional 2025', 'Video corporativo para presentación anual', 'Comunicar resultados y visión 2025', 'Stakeholders', 'Duración máxima 3 minutos', '2025-01-15', 1, 1, 'https://ejemplo.com', 0, 1, GETDATE(), GETDATE(), GETDATE());
GO

SELECT Id, Nombre, EstatusBriefId FROM Briefs;
GO
