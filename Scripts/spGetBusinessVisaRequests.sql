CREATE OR ALTER PROCEDURE [dbo].[spGetBusinessVisaRequests]
    @Keyword NVARCHAR(200) = '',
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @Type INT = NULL,
    @EmployeeID INT = NULL,
    @DepartmentID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        v.ID,
        v.STT,
        v.Type,
        v.EmployeeID,
        -- Nếu Type = 1 (CBNV) và có ID nhân viên thì lấy tên nhân viên, nếu không thì lấy FullName nhập tay
        ISNULL(e.FullName, v.FullName) AS FullName,
        ISNULL(e.DateOfBirth, v.DateOfBirth) AS DateOfBirth,
        ISNULL(e.Gender, v.Gender) AS Gender,
        v.Nation,
        v.HoChieu,
        v.NgheNghiep,
        v.CompanyName,
        v.Destination,
        v.BusinessTripFromDate,
        v.BusinessTripToDate,
        v.Cost,
        v.VisaIssueDate,
        v.Note,
        v.Status,
        v.CreatedDate,
        v.CreatedBy,
        v.UpdatedDate,
        v.UpdatedBy,
        v.IsDeleted,
        
        e.Code AS EmployeeCode,
        e.FullName AS EmployeeName,
        d.Name AS DepartmentName
    FROM dbo.BusinessVisaRequest v
    LEFT JOIN dbo.Employee e ON v.EmployeeID = e.ID
    LEFT JOIN dbo.Department d ON e.DepartmentID = d.ID
    WHERE (v.IsDeleted = 0 OR v.IsDeleted IS NULL)
        AND (@Type IS NULL OR @Type = 0 OR v.Type = @Type)
        AND (@EmployeeID IS NULL OR @EmployeeID = 0 OR v.EmployeeID = @EmployeeID)
        AND (@DepartmentID IS NULL OR @DepartmentID = 0 OR d.ID = @DepartmentID)
        AND (@StartDate IS NULL OR v.CreatedDate >= @StartDate)
        AND (@EndDate IS NULL OR v.CreatedDate <= @EndDate)
        AND (@Keyword = '' 
            OR v.FullName LIKE N'%' + @Keyword + '%'
            OR v.CompanyName LIKE N'%' + @Keyword + '%'
            OR v.Destination LIKE N'%' + @Keyword + '%'
            OR e.FullName LIKE N'%' + @Keyword + '%'
            OR e.Code LIKE N'%' + @Keyword + '%'
        )
    ORDER BY v.ID DESC;
END
GO
