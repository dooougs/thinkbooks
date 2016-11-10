CREATE TABLE [dbo].[Sale] (
    [SaleID] INT      IDENTITY (1, 1) NOT NULL,
    [Price]  MONEY    NOT NULL,
    [Date]   DATETIME NOT NULL,
    [BookID] INT      NOT NULL,
    CONSTRAINT [PK_Sale] PRIMARY KEY CLUSTERED ([SaleID] ASC),
    CONSTRAINT [FK_Sale_Book] FOREIGN KEY ([BookID]) REFERENCES [dbo].[Book] ([BookID])
);

