CREATE TABLE [dbo].[Author] (
    [AuthorID]       INT            IDENTITY (1, 1) NOT NULL,
    [AuthorStatusID] INT            NOT NULL,
    [FirstName]      NVARCHAR (255) NOT NULL,
    [LastName]       NVARCHAR (255) NOT NULL,
    [DateOfBirth]    DATE           NULL,
    CONSTRAINT [PK__Author__70DAFC14D901D86E] PRIMARY KEY CLUSTERED ([AuthorID] ASC),
    CONSTRAINT [FK_Author_AuthorStatus] FOREIGN KEY ([AuthorStatusID]) REFERENCES [dbo].[AuthorStatus] ([AuthorStatusID])
);






GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-LastName]
    ON [dbo].[Author]([LastName] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-FirstName]
    ON [dbo].[Author]([FirstName] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-DOB]
    ON [dbo].[Author]([DateOfBirth] ASC);

