SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
ALTER PROCEDURE [dbo].[spGetVehicleRentalRequests]
    @Keyword NVARCHAR(MAX) = '',
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @EmployeeRequestID INT = 0,
    @EmployeeID INT = 0,
    @DepartmentID INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT vr.ID,
           vr.DateRequest,
           vr.STT,
           vr.DepartmentID,
           vr.VehicleType,
           vr.PackageName,
           vr.ProjectID,
           vr.PackageQuantity,
           vr.PackageLengthCm,
           vr.PackageWidthCm,
           vr.PackageHeightCm,
           vr.PackageWeightKg,
           vr.DepartureLocation,
           vr.AddressLocation,
           vr.DistanceKm,
           vr.NameNCC,
           vr.Cost,
           vr.Note,
           vr.EmployeeRequestID,
           vr.EmployeeID,
           vr.UpdatedDate,
           vr.UpdatedBy,
           vr.CreatedDate,
           vr.CreatedBy,
           vr.IsDeleted,
           er.FullName AS EmployeeRequestName,
           e.FullName AS EmployeeName,
           d.Name AS DepartmentName,
           ProjectName = p.ProjectCode + '-' + p.ProjectName
    FROM VehicleRentalRequest vr
        LEFT JOIN Employee er
            ON vr.EmployeeRequestID = er.ID
        LEFT JOIN Employee e
            ON vr.EmployeeID = e.ID
        LEFT JOIN Department d
            ON vr.DepartmentID = d.ID
        LEFT JOIN Project p
            ON vr.ProjectID = p.ID
    WHERE (
              vr.IsDeleted IS NULL
              OR vr.IsDeleted = 0
          )
      AND (@StartDate IS NULL OR vr.DateRequest >= @StartDate)
      AND (@EndDate IS NULL OR vr.DateRequest <= @EndDate)
      AND (@EmployeeRequestID = 0 OR vr.EmployeeRequestID = @EmployeeRequestID)
      AND (@EmployeeID = 0 OR vr.EmployeeID = @EmployeeID)
      AND (@DepartmentID = 0 OR vr.DepartmentID = @DepartmentID)
      AND (@Keyword = '' OR (
          vr.PackageName LIKE N'%' + @Keyword + N'%'
          OR vr.DepartureLocation LIKE N'%' + @Keyword + N'%'
          OR vr.AddressLocation LIKE N'%' + @Keyword + N'%'
          OR vr.NameNCC LIKE N'%' + @Keyword + N'%'
          OR er.FullName LIKE N'%' + @Keyword + N'%'
          OR e.FullName LIKE N'%' + @Keyword + N'%'
          OR vr.Note LIKE N'%' + @Keyword + N'%'
      ))
    ORDER BY vr.STT,
             vr.DateRequest DESC;
END;
GO
