CREATE TABLE [dbo].[BookStatus] (
    [BookStatusID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_BookStatus] PRIMARY KEY CLUSTERED ([BookStatusID] ASC)
);

