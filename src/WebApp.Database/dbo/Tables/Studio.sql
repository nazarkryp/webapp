CREATE TABLE [dbo].[Studios] (
	[StudioId]				INT				PRIMARY KEY		IDENTITY(1, 1),
	[Name]					NVARCHAR(50)	NOT NULL,

	CONSTRAINT [Studios_Name] UNIQUE ([Name])
)
