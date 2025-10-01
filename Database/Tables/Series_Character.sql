CREATE TABLE [dbo].[Series_Character] (
    [SeriesId]    INT           NOT NULL,
    [CharacterId] INT           NOT NULL,
    [Role]        VARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([SeriesId] ASC, [CharacterId] ASC),
    FOREIGN KEY ([CharacterId]) REFERENCES [dbo].[Character] ([Id]),
    FOREIGN KEY ([SeriesId]) REFERENCES [dbo].[Series] ([Id])
);

