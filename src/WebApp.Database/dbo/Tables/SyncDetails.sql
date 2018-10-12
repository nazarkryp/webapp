CREATE TABLE [dbo].[SyncDetails] (
	[SyncDetailsId]			INT				PRIMARY KEY		IDENTITY(1, 1),
	[StudioId]				INT				NOT NULL,
	[LastSyncDate]			DATETIME		NOT NULL,
	[LastSyncPage]			INT				NULL,
	CONSTRAINT [FK_SyncDetails_StudioId_Studios_StudioId] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studios] ([StudioId])
)

GO
CREATE NONCLUSTERED INDEX [IX_SyncDetails_StudioId]
    ON [dbo].[SyncDetails]([StudioId] ASC);
