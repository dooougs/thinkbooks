CREATE TABLE [dbo].[AuthorStatus] (
    [AuthorStatusID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_AuthorStatus] PRIMARY KEY CLUSTERED ([AuthorStatusID] ASC)
);

