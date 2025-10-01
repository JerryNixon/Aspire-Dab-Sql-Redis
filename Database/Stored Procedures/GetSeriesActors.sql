CREATE PROCEDURE dbo.GetSeriesActors
	@seriesId INT = 1,
	@top INT = 5
AS
BEGIN
	SET NOCOUNT ON;
	SELECT TOP (@top) * 
	FROM dbo.SeriesActors
	WHERE SeriesId = @seriesId;
END