CREATE TABLE [dbo].[TopCategories] (
	[TopCategoryId]		INT		PRIMARY KEY		IDENTITY(1,1),
	[CategoryId]		INT		NOT NULL,
	CONSTRAINT [FK_TopCategories_CategoryId_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([CategoryId])
)
