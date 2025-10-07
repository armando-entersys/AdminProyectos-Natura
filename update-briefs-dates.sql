USE AdminProyectos;
GO

-- Actualizar fechas de los briefs existentes para que sean relevantes
UPDATE Briefs SET FechaPublicacion = GETDATE(), FechaEntrega = GETDATE() WHERE Id = 2; -- Hoy
UPDATE Briefs SET FechaPublicacion = DATEADD(day, 2, GETDATE()), FechaEntrega = DATEADD(day, 2, GETDATE()) WHERE Id = 3; -- En 2 días (esta semana)
UPDATE Briefs SET FechaPublicacion = DATEADD(day, 10, GETDATE()), FechaEntrega = DATEADD(day, 10, GETDATE()) WHERE Id = 4; -- Próxima semana
UPDATE Briefs SET FechaPublicacion = DATEADD(day, 15, GETDATE()), FechaEntrega = DATEADD(day, 15, GETDATE()) WHERE Id = 5; -- En 2 semanas
UPDATE Briefs SET FechaPublicacion = GETDATE(), FechaEntrega = GETDATE() WHERE Id = 6; -- Hoy

-- Poner todos en estatus 1 (En revisión) para que se cuenten
UPDATE Briefs SET EstatusBriefId = 1;
GO

SELECT Id, Nombre, EstatusBriefId, FechaEntrega, FechaPublicacion FROM Briefs;
GO
