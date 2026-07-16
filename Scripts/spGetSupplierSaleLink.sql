SET QUOTED_IDENTIFIER OFF
SET ANSI_NULLS ON
GO

ALTER PROCEDURE [dbo].[spGetSupplierSaleLink]
    @EmployeePurchaseID INT = 0,
    @KeyWord NVARCHAR(250) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT 
        ssl.ID,
        ssl.SupplierSaleID,
        ssl.EmployeePurchaseID,
        ssl.Note,
        ssl.MatHang,
        ssl.Website,
        ssl.AgencyTime,
        ssl.IsAgencyCertified,
        ss.CodeNCC,
        ss.NameNCC,
        e.Code AS EmployeePurchaseCode,
        e.FullName AS EmployeePurchaseName
    FROM SupplierSaleLink ssl WITH (NOLOCK)
    INNER JOIN SupplierSale ss WITH (NOLOCK) ON ssl.SupplierSaleID = ss.ID
    INNER JOIN Employee e WITH (NOLOCK) ON ssl.EmployeePurchaseID = e.ID
    WHERE (@EmployeePurchaseID = 0 OR ssl.EmployeePurchaseID = @EmployeePurchaseID)
      AND (@KeyWord = '' OR ss.CodeNCC LIKE '%' + @KeyWord + '%' OR ss.NameNCC LIKE '%' + @KeyWord + '%' OR e.FullName LIKE '%' + @KeyWord + '%')
    ORDER BY e.FullName, ss.CodeNCC
END
GO
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
