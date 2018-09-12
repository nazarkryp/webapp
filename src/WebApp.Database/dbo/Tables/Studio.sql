CREATE TABLE [dbo].[Studios] (
	[StudioId]				INT				PRIMARY KEY		IDENTITY(1, 1),
	[Name]					NVARCHAR(50)	NOT NULL, 
	-- [SyncDetailsId]			INT				NULL,
	-- CONSTRAINT [FK_SyncDetails_SyncDetailsId_SyncDetails_SyncDetailsId] FOREIGN KEY ([SyncDetailsId]) REFERENCES [dbo].[SyncDetails]([SyncDetailsId])
)
