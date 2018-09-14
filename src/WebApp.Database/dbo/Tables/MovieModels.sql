CREATE TABLE [dbo].[MovieModels] (
	[MovieId]			INT				NOT NULL,
	[ModelId]			INT				NOT NULL,
	CONSTRAINT [PK_MovieModels_MovieId_ModelId] PRIMARY KEY ([MovieId], [ModelId]),
	CONSTRAINT [FK_MovieModels_MovieId_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([MovieId]) ON DELETE CASCADE,
	CONSTRAINT [FK_MovieModels_ModelId_Models_ModelId] FOREIGN KEY ([ModelId]) REFERENCES [dbo].[Models] ([ModelId]) ON DELETE CASCADE
)