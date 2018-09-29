CREATE TABLE [dbo].[Categories] (
	[CategoryId]			INT				PRIMARY KEY		IDENTITY(1, 1),
	[Name]					NVARCHAR(50)	NOT NULL,
	CONSTRAINT [UQ_Categories_Name]	UNIQUE ([Name])
)