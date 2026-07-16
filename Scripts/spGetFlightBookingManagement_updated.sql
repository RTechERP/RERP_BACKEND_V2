SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[spGetFlightBookingManagement]
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @Keyword NVARCHAR(500) = NULL,
    @EmployeeID INT = 0,
    @ProjectID INT = 0,
    @EmployeeBookerID INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT fb.ID,
           fb.EmployeeID,
           er.FullName AS RequesterName,
           (
               SELECT STRING_AGG(COALESCE(emp.FullName, CAST(fbp.FullName AS NVARCHAR(250))), ', ')
               FROM dbo.FlightBookingPassenger fbp
               LEFT JOIN dbo.Employee emp ON fbp.EmployeeID = emp.ID
               WHERE fbp.FlightBookingManagementID = fb.ID
           ) AS PassengerName,
           er.Code,
           fb.Reason,
           fb.ProjectID,
           p.ProjectName,
           fb.DepartureAddress,
           fb.ArrivesAddress,
           fb.DepartureDate,
           fb.DepartureTime,
           fb.EmployeeBookerID,
           eb.FullName AS BookerName,
           fb.BookedDate,
           fb.Note,
           fb.CreatedDate,
           fb.CreatedBy,
           fb.IsRoundTrip
    FROM dbo.FlightBookingManagement fb
        LEFT JOIN dbo.Employee er
            ON fb.EmployeeRequestID = er.ID
        LEFT JOIN dbo.Project p
            ON fb.ProjectID = p.ID
        LEFT JOIN dbo.Employee eb
            ON fb.EmployeeBookerID = eb.ID
    WHERE ISNULL(fb.IsDeleted, 0) = 0
          AND
          (
              @StartDate IS NULL
              OR CAST(fb.BookedDate AS DATE) >= CAST(@StartDate AS DATE)
          )
          AND
          (
              @EndDate IS NULL
              OR CAST(fb.BookedDate AS DATE) <= CAST(@EndDate AS DATE)
          )
          AND
          (
              @EmployeeID = 0
              OR EXISTS (
                  SELECT 1 
                  FROM dbo.FlightBookingPassenger fbp
                  WHERE fbp.FlightBookingManagementID = fb.ID
                    AND fbp.EmployeeID = @EmployeeID
              )
          )
          AND
          (
              @ProjectID = 0
              OR fb.ProjectID = @ProjectID
          )
          AND
          (
              @EmployeeBookerID = 0
              OR fb.EmployeeBookerID = @EmployeeBookerID
          )
          AND
          (
              @Keyword IS NULL
              OR @Keyword = ''
              OR er.FullName LIKE '%' + @Keyword + '%'
              OR fb.Reason LIKE '%' + @Keyword + '%'
              OR fb.DepartureAddress LIKE '%' + @Keyword + '%'
              OR fb.ArrivesAddress LIKE '%' + @Keyword + '%'
              OR EXISTS (
                  SELECT 1
                  FROM dbo.FlightBookingPassenger fbp
                  LEFT JOIN dbo.Employee emp ON fbp.EmployeeID = emp.ID
                  WHERE fbp.FlightBookingManagementID = fb.ID
                    AND (
                        emp.FullName LIKE '%' + @Keyword + '%'
                        OR fbp.FullName LIKE '%' + @Keyword + '%'
                    )
              )
          )
    ORDER BY fb.BookedDate DESC;
END;
GO
