CREATE TABLE [dbo].[Movies] (
	[MovieId]			INT				PRIMARY KEY		IDENTITY(1, 1),
	[Title]				NVARCHAR(250)	NOT NULL,
	[Uri]				NVARCHAR(250)	NOT NULL,
	[Description]		NVARCHAR(1250)	NULL,
	[Date]				DATETIME		NULL,
	[Duration]			TIME(7)			NULL,
	[StudioId]			INT				NOT NULL,
	CONSTRAINT [FK_Movies_StudioId_Studios_StudioId] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studios] ([StudioId])
)

GO
CREATE NONCLUSTERED INDEX [IX_Movies_StudioId]
    ON [dbo].[Movies]([StudioId] ASC);
