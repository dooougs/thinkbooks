-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetAuthors
	@SortColumn nvarchar(20) = 'AuthorID'
	,@SortDirection nvarchar(5) = 'ASC' -- default for ease of testing
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT * FROM Author
	ORDER BY
	  --nvarchar
	  CASE WHEN @SortDirection = 'ASC' THEN
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
