MERGE dbo.Category AS target
USING (VALUES
    ('Home'),
    ('Work'),
    ('School'),
    ('Personal')
) AS source(Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Name) VALUES (source.Name);

DECLARE @Seeds TABLE
(
    Title NVARCHAR(500),
    IsCompleted BIT,
    CategoryName NVARCHAR(100)
);

INSERT INTO @Seeds (Title, IsCompleted, CategoryName)
VALUES
    ('Walk the dog', 0, 'Home'),
    ('Walk the cat', 1, 'Personal');

MERGE dbo.Todo AS target
USING (
    SELECT s.Title, s.IsCompleted, c.Id AS CategoryId
    FROM @Seeds s
    INNER JOIN dbo.Category c ON c.Name = s.CategoryName
) AS source(Title, IsCompleted, CategoryId)
ON target.Title = source.Title
WHEN MATCHED AND (target.IsCompleted <> source.IsCompleted OR target.CategoryId <> source.CategoryId) THEN
    UPDATE SET IsCompleted = source.IsCompleted, CategoryId = source.CategoryId
WHEN NOT MATCHED THEN
    INSERT (Title, IsCompleted, CategoryId)
    VALUES (source.Title, source.IsCompleted, source.CategoryId);