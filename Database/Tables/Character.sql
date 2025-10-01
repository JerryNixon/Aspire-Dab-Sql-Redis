CREATE TABLE [dbo].[Character] (
    [Id]       INT             NOT NULL,
    [Name]     NVARCHAR (255)  NOT NULL,
    [ActorId]  INT             NOT NULL,
    [Stardate] DECIMAL (10, 2) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([ActorId]) REFERENCES [dbo].[Actor] ([Id])
);

