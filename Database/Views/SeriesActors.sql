CREATE VIEW dbo.SeriesActors
AS
SELECT
	a.*, s.Id AS SeriesId
FROM [dbo].[Series] AS s
JOIN [dbo].[Series_Character] AS sc ON s.Id = sc.SeriesId
JOIN [dbo].[Character] AS c ON sc.CharacterId = c.Id
JOIN [dbo].[Actor] AS a ON c.ActorId = a.Id;