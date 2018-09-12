CREATE TABLE [dbo].[Attachments] (
	[AttachmentId]			INT				PRIMARY KEY		IDENTITY(1, 1),
	[Uri]					NVARCHAR(250)	NOT NULL,
	[MovieId]				INT				NOT NULL,
	CONSTRAINT [FK_Attachments_AttachmentId_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([MovieId]) ON DELETE CASCADE,
)

GO
CREATE NONCLUSTERED INDEX [IX_Attachments_MovieId]
    ON [dbo].[Attachments]([MovieId] ASC);
