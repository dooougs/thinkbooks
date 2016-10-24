CREATE TABLE [dbo].[Author] (
    [AuthorID]    INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (255) NOT NULL,
    [LastName]    NVARCHAR (255) NOT NULL,
    [DateOfBirth] DATE           NULL,
    PRIMARY KEY CLUSTERED ([AuthorID] ASC)
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

