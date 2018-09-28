CREATE TABLE [dbo].[MovieCategories] (
	[MovieId]				INT				NOT NULL,
	[CategoryId]			INT				NOT NULL,
	CONSTRAINT [PK_MovieCategories_MovieId_CategoryId]				PRIMARY KEY ([MovieId], [CategoryId]),
	CONSTRAINT [FK_MovieCategories_MovieId_Movies_MovieId]			FOREIGN KEY ([MovieId])		REFERENCES [dbo].[Movies] ([MovieId])			ON DELETE CASCADE,
	CONSTRAINT [FK_MovieCategories_CategoryId_Category_ModelId]		FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([CategoryId])	ON DELETE CASCADE
)