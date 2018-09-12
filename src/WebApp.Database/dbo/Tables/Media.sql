CREATE TABLE [dbo].[Media] (
	[MediaId]			INT				PRIMARY KEY		IDENTITY(1, 1),
	[OriginalUri]		NVARCHAR(250)	NOT NULL,
	[Thumbnail]			NVARCHAR(250)	NULL,
	[Small]				NVARCHAR(250)	NULL,
	[Description]		NVARCHAR(1250)	NULL,
	[IsVideo]			BIT				NOT NULL		DEFAULT(0), 
    [ObjectId]			NVARCHAR(100)	NOT NULL
)