SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[spGetHotelBookingManagement]
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
           fb.STT,
           fb.EmployeeRequestID,
           er.FullName AS RequesterName,
           (
               SELECT STRING_AGG(COALESCE(emp.FullName, CAST(hbe.FullName AS NVARCHAR(250))), ', ')
               FROM dbo.HotelBookingEmployee hbe
               LEFT JOIN dbo.Employee emp ON hbe.EmployeeID = emp.ID
               WHERE hbe.HotelBookingManagementID = fb.ID AND ISNULL(hbe.IsDeleted, 0) = 0
           ) AS PassengerName,
           er.Code,
           fb.Reason,
           fb.ProjectID,
           p.ProjectName,
           fb.Location,
           fb.CheckinDate,
           fb.CheckOutDate,
           fb.EmployeeBookerID,
           eb.FullName AS BookerName,
           fb.DateRequest,
           fb.Note,
           fb.CreatedDate,
           fb.CreatedBy,
           fb.EmployeeApproverID
    FROM dbo.HotelBookingManagement fb
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
              OR CAST(COALESCE(fb.DateRequest, fb.CreatedDate) AS DATE) >= CAST(@StartDate AS DATE)
          )
          AND
          (
              @EndDate IS NULL
              OR CAST(COALESCE(fb.DateRequest, fb.CreatedDate) AS DATE) <= CAST(@EndDate AS DATE)
          )
          AND
          (
              @EmployeeID = 0
              OR EXISTS (
                  SELECT 1 
                  FROM dbo.HotelBookingEmployee hbe
                  WHERE hbe.HotelBookingManagementID = fb.ID AND ISNULL(hbe.IsDeleted, 0) = 0
                    AND hbe.EmployeeID = @EmployeeID
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
              OR fb.Location LIKE '%' + @Keyword + '%'
              OR EXISTS (
                  SELECT 1
                  FROM dbo.HotelBookingEmployee hbe
                  LEFT JOIN dbo.Employee emp ON hbe.EmployeeID = emp.ID
                  WHERE hbe.HotelBookingManagementID = fb.ID AND ISNULL(hbe.IsDeleted, 0) = 0
                    AND (
                        emp.FullName LIKE '%' + @Keyword + '%'
                        OR hbe.FullName LIKE '%' + @Keyword + '%'
                    )
              )
          )
    ORDER BY COALESCE(fb.DateRequest, fb.CreatedDate) DESC;
END;
GO
