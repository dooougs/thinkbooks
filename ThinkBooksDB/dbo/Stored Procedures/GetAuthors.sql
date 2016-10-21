
CREATE PROCEDURE [dbo].[GetAuthors]
	@SortColumn nvarchar(20) 
	,@SortDirection nvarchar(4)
	,@AuthorID int
	,@FirstName nvarchar(255)
	,@LastName nvarchar(255)
	,@DateOfBirth datetime
	,@NumberOfResults int
AS
BEGIN
    SELECT TOP (@NumberOfResults) * FROM Author
	WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
	AND (@FirstName IS NULL OR FirstName LIKE '%'+@FirstName+'%')
	AND (@LastName IS NULL OR LastName LIKE '%'+@LastName+'%')
	AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
	ORDER BY
	  --int
	  CASE WHEN @SortDirection = 'ASC' THEN
		  CASE @SortColumn
			WHEN 'AuthorID' THEN AuthorID
		  END
	  END ASC
	  --nvarchar
	  ,CASE WHEN @SortDirection = 'ASC' THEN
		  CASE @SortColumn
			WHEN 'FirstName' THEN FirstName
			WHEN 'LastName' THEN LastName
		  END
	  END ASC
	  --datetime
	  ,CASE WHEN @SortDirection = 'ASC' THEN
		  CASE @SortColumn
			WHEN 'DateOfBirth' THEN DateOfBirth
		  END
	  END ASC
	   --int
	  ,CASE WHEN @SortDirection = 'DESC' THEN
		  CASE @SortColumn
			WHEN 'AuthorID' THEN AuthorID
		  END
	  END DESC
	  ,CASE WHEN @SortDirection = 'DESC' THEN
		  CASE @SortColumn
			WHEN 'FirstName' THEN FirstName
			WHEN 'LastName' THEN LastName
		  END
	   END DESC
	   --datetime
	  ,CASE WHEN @SortDirection = 'DESC' THEN
		  CASE @SortColumn
			WHEN 'DateOfBirth' THEN DateOfBirth
		  END
	  END DESC

	-- Count
	SELECT COUNT(*) FROM Author
	WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
	AND (@FirstName IS NULL OR FirstName LIKE '%'+@FirstName+'%')
	AND (@LastName IS NULL OR LastName LIKE '%'+@LastName+'%')
	AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
END
