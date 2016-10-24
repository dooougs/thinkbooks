
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
--DECLARE @sortOrderColumn varchar(75)
--SET @SortColumn = 'AuthorID'
SET @sqlCommand = 'SELECT TOP 100 * FROM Author 
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


 --   SELECT TOP (@NumberOfResults) * FROM Author
	--WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
	--AND (@FirstName IS NULL OR FirstName LIKE '%'+@FirstName+'%')
	--AND (@LastName IS NULL OR LastName LIKE '%'+@LastName+'%')
	--AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
	---- big conditional here!

	--ORDER BY AuthorID DESC
	   --CASE WHEN @SortOrderCode = 1 THEN AuthorID 
	   --END ASC
	   --,CASE WHEN @SortOrderCode = 2 THEN AuthorID 
	   --END DESC
	  ----int
	  --CASE WHEN @SortDirection = 'ASC' THEN
		 -- CASE @SortColumn
			--WHEN 'AuthorID' THEN AuthorID
		 -- END
	  --END ASC
	  ----nvarchar
	  --,CASE WHEN @SortDirection = 'ASC' THEN
		 -- CASE @SortColumn
			--WHEN 'FirstName' THEN FirstName
			--WHEN 'LastName' THEN LastName
		 -- END
	  --END ASC
	  ----datetime
	  --,CASE WHEN @SortDirection = 'ASC' THEN
		 -- CASE @SortColumn
			--WHEN 'DateOfBirth' THEN DateOfBirth
		 -- END
	  --END ASC
	  -- --int
	  --,CASE WHEN @SortDirection = 'DESC' THEN
		 -- CASE @SortColumn
			--WHEN 'AuthorID' THEN AuthorID
		 -- END
	  --END DESC
	  --,CASE WHEN @SortDirection = 'DESC' THEN
		 -- CASE @SortColumn
			--WHEN 'FirstName' THEN FirstName
			--WHEN 'LastName' THEN LastName
		 -- END
	  -- END DESC
	  -- --datetime
	  --,CASE WHEN @SortDirection = 'DESC' THEN
		 -- CASE @SortColumn
			--WHEN 'DateOfBirth' THEN DateOfBirth
		 -- END
	  --END DESC

	-- Count
	--SELECT COUNT(*) FROM Author
	--WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
	--AND (@FirstName IS NULL OR FirstName LIKE '%'+@FirstName+'%')
	--AND (@LastName IS NULL OR LastName LIKE '%'+@LastName+'%')
	--AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
END
