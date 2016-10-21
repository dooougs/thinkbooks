
CREATE PROCEDURE [dbo].[GetAuthors]
	@SortColumn nvarchar(20) 
	,@SortDirection nvarchar(4)
AS
BEGIN
    SELECT * FROM Author
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
END
