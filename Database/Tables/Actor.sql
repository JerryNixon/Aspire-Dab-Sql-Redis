CREATE TABLE [dbo].[Actor] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [BirthYear] INT            CONSTRAINT [DEFAULT_Actor_BirthYear] DEFAULT ((1999)) NULL,
    [Age]       AS             ((123)),
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

