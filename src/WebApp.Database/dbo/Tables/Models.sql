CREATE TABLE [dbo].[Models] (
	[ModelId]			INT				PRIMARY KEY		IDENTITY(1, 1),
	[Name]				NVARCHAR(250)	NOT NULL,
	CONSTRAINT [UQ_Models_Name] UNIQUE ([Name])
)
