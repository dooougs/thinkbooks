CREATE TABLE [dbo].[Book] (
    [BookID]   INT            IDENTITY (1, 1) NOT NULL,
    [AuthorID] INT            NOT NULL,
    [Title]    NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_Book] PRIMARY KEY CLUSTERED ([BookID] ASC)
);

