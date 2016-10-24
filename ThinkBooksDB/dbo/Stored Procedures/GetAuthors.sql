
CREATE PROCEDURE [dbo].[GetAuthors]
	@SortColumn nvarchar(20) 
	,@SortDirection varchar(4)
	,@AuthorID int
	,@FirstName nvarchar(255)
	,@LastName nvarchar(255)
	,@DateOfBirth datetime
	,@NumberOfResults int
AS
BEGIN

DECLARE @parameters nvarchar(2000) = 
	N'@SortColumn nvarchar(75)
	,@SortDirection varchar(4)
	,@AuthorID int
	,@FirstName nvarchar(255)
	,@LastName nvarchar(255)
	,@DateOfBirth datetime
	,@NumberOfResults int
	'
DECLARE @sqlCommand nvarchar(2000)
SET @sqlCommand = 'SELECT TOP (@NumberOfResults) * FROM Author 
				   WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
                   AND (@FirstName IS NULL OR FirstName LIKE CONCAT(''%'',@FirstName,''%''))
	               AND (@LastName IS NULL OR LastName LIKE ''%''+@LastName+''%'')
	               AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
                   ORDER BY ' + @SortColumn + ' ' + @SortDirection

EXECUTE sp_executesql @sqlCommand, @parameters
	,@SortColumn = @SortColumn
	,@SortDirection = @SortDirection 
	,@AuthorID = @AuthorID
	,@FirstName = @FirstName
	,@LastName = @LastName
	,@DateOfBirth = @DateOfBirth
	,@NumberOfResults = @NumberOfResults

-- https://sqlperformance.com/2014/10/t-sql-queries/bad-habits-count-the-hard-way	
-- Count - if there are no filters then just need the total number in the table
IF (@AuthorID IS NULL
    AND @FirstName IS NULL -- these were ''
	AND @LastName IS NULL -- ''
	AND @DateOfBirth IS NULL)
	BEGIN
   SELECT SUM(p.rows)
	  FROM sys.partitions AS p
	  INNER JOIN sys.tables AS t
	  ON p.[object_id] = t.[object_id]
	  INNER JOIN sys.schemas AS s
	  ON t.[schema_id] = s.[schema_id]
	  WHERE p.index_id IN (0,1) -- heap or clustered index
	  AND t.name = N'Author'
	  AND s.name = N'dbo'
	  END
ELSE
    BEGIN
	DECLARE @sqlCommand2 nvarchar(2000)
	SET @sqlCommand2 = 'SELECT COUNT(*) FROM Author 
				   WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
                   AND (@FirstName IS NULL OR FirstName LIKE CONCAT(''%'',@FirstName,''%''))
	               AND (@LastName IS NULL OR LastName LIKE ''%''+@LastName+''%'')
	               AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)'
	EXECUTE sp_executesql @sqlCommand2, @parameters
	,@SortColumn = @SortColumn
	,@SortDirection = @SortDirection 
	,@AuthorID = @AuthorID
	,@FirstName = @FirstName
	,@LastName = @LastName
	,@DateOfBirth = @DateOfBirth
	,@NumberOfResults = @NumberOfResults
	END
END
