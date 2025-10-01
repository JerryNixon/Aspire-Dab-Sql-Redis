CREATE TABLE [dbo].[Character_Species] (
    [CharacterId] INT NOT NULL,
    [SpeciesId]   INT NOT NULL,
    PRIMARY KEY CLUSTERED ([CharacterId] ASC, [SpeciesId] ASC),
    FOREIGN KEY ([CharacterId]) REFERENCES [dbo].[Character] ([Id]),
    FOREIGN KEY ([SpeciesId]) REFERENCES [dbo].[Species] ([Id])
);

