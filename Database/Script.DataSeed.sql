IF NOT EXISTS(SELECT 1 FROM dbo.Todo)
BEGIN
    INSERT INTO dbo.Todo (Title, IsCompleted)
    VALUES ('Walk the dog', 0),
           ('Walk the cat', 1);
END