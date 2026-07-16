SET QUOTED_IDENTIFIER OFF
SET ANSI_NULLS ON
GO

ALTER PROCEDURE [dbo].[spGetSupplierSaleWithSelection]
	@EmployeePurchaseID int = 0
   ,@SearchKeyword nvarchar(250) = NULL
   ,@PageNumber int = 1
   ,@PageSize int = 50
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Offset int = (@PageNumber - 1) * @PageSize;
	SELECT
		  ss.ID
		 ,ss.CodeNCC
		 ,ss.NameNCC
		 ,CASE
			  WHEN ssl.ID IS NOT NULL THEN 1
			  ELSE 0
		  END AS IsSelected
		 ,ISNULL(ssl.Note, '') AS Note
		 ,ISNULL(ssl.MatHang, '') AS MatHang
		 ,ssl.Website
		 ,ssl.AgencyTime
		 ,ssl.IsAgencyCertified
		 ,COUNT(*) OVER () AS TotalCount
	INTO  #tempNCC
	FROM  SupplierSale ss
		  LEFT JOIN SupplierSaleLink ssl ON ss.ID = ssl.SupplierSaleID
											AND ssl.EmployeePurchaseID = @EmployeePurchaseID
	WHERE (
			  ss.IsDeleted = 0
			  OR ss.IsDeleted IS NULL
		  )
		  AND
		  (
			  @SearchKeyword IS NULL
			  OR @SearchKeyword = ''
			  OR ss.CodeNCC LIKE '%' + @SearchKeyword + '%'
			  OR ss.NameNCC LIKE '%' + @SearchKeyword + '%'
		  );

	SELECT
			 *
	FROM	 #tempNCC
	ORDER BY IsSelected DESC, CodeNCC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
