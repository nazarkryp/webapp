CREATE TABLE [dbo].[Movies] (
	[MovieId]			INT				PRIMARY KEY		IDENTITY(1, 1),
	[Title]				NVARCHAR(250)	NOT NULL,
	[Uri]				NVARCHAR(250)	NOT NULL,
	[Description]		NVARCHAR(MAX)	NULL,
	[Date]				DATE			NULL,
	[Duration]			TIME(7)			NULL,
	[StudioId]			INT				NOT NULL,
	CONSTRAINT [FK_Movies_StudioId_Studios_StudioId] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studios] ([StudioId]),
	CONSTRAINT [Movies_Uri] UNIQUE ([Uri])
)

GO
CREATE NONCLUSTERED INDEX [IX_Movies_StudioId]
    ON [dbo].[Movies]([StudioId] ASC);

GO
CREATE INDEX [IX_Movies_Date]
	ON [dbo].[Movies] ([Date]) INCLUDE ([MovieId], [Title], [Uri], [Description], [Duration], [StudioId])
