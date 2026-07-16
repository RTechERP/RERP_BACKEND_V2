SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[spGetHotelBookingExportExcel]
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @Keyword NVARCHAR(500) = NULL,
    @ProjectID INT = NULL,
    @SelectedIDs VARCHAR(MAX) = NULL,
    @EmployeeBookerID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Store matched MasterIDs in a temp table
    SELECT DISTINCT m.ID AS MasterID
    INTO #Matched
    FROM dbo.HotelBookingManagement m
        LEFT JOIN dbo.Employee e ON m.EmployeeRequestID = e.ID
        LEFT JOIN dbo.Project p ON m.ProjectID = p.ID
        LEFT JOIN dbo.Employee eb ON m.EmployeeBookerID = eb.ID
    WHERE ISNULL(m.IsDeleted, 0) = 0
          AND (
              @StartDate IS NULL
              OR CAST(COALESCE(m.DateRequest, m.CreatedDate) AS DATE) >= CAST(@StartDate AS DATE)
          )
          AND (
              @EndDate IS NULL
              OR CAST(COALESCE(m.DateRequest, m.CreatedDate) AS DATE) <= CAST(@EndDate AS DATE)
          )
          AND (
              @SelectedIDs IS NULL
              OR @SelectedIDs = ''
              OR m.ID IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(@SelectedIDs, ','))
          )
          AND (
              @ProjectID IS NULL
              OR @ProjectID = 0
              OR m.ProjectID = @ProjectID
          )
          AND (
              @EmployeeBookerID IS NULL
              OR @EmployeeBookerID = 0
              OR m.EmployeeBookerID = @EmployeeBookerID
          )
          AND (
              @Keyword IS NULL
              OR @Keyword = ''
              OR m.Reason LIKE '%' + @Keyword + '%'
              OR p.ProjectName LIKE '%' + @Keyword + '%'
              OR e.FullName LIKE '%' + @Keyword + '%'
              OR m.Location LIKE '%' + @Keyword + '%'
              OR EXISTS (
                  SELECT 1
                  FROM dbo.HotelBookingEmployee hbe
                  LEFT JOIN dbo.Employee emp ON hbe.EmployeeID = emp.ID
                  WHERE hbe.HotelBookingManagementID = m.ID AND ISNULL(hbe.IsDeleted, 0) = 0
                    AND (
                        emp.FullName LIKE '%' + @Keyword + '%'
                        OR hbe.FullName LIKE '%' + @Keyword + '%'
                    )
              )
          );

    -- Table 0: Master and Proposal details
    SELECT m.ID AS MasterID,
           m.EmployeeRequestID,
           e.FullName AS RequesterName,
           m.Reason,
           ProjectName = p.ProjectCode + '-' + p.ProjectName,
           PassengerName = (
               SELECT STRING_AGG(COALESCE(emp.FullName, CAST(hbe.FullName AS NVARCHAR(250))), ', ')
               FROM dbo.HotelBookingEmployee hbe
               LEFT JOIN dbo.Employee emp ON hbe.EmployeeID = emp.ID
               WHERE hbe.HotelBookingManagementID = m.ID AND ISNULL(hbe.IsDeleted, 0) = 0
           ),
           m.Location,
           m.CheckinDate,
           m.CheckOutDate,
           pr.UnitPrice,
           pr.Quantity,
           pr.TotalAmount,
           pr.IsApprove,
           HCNSProposal = pr.IsHCNSProposal,
           eb.FullName AS BookerName,
           BookedDate = COALESCE(m.DateRequest, m.CreatedDate),
           m.Note,
           ApproverName = ep.FullName,
           ReasonHCNSProposal = pr.ReasonHCNSProposal,
           pr.TypeRoom,
           ProposalID = pr.ID
    FROM dbo.HotelBookingManagement m
        INNER JOIN #Matched mt ON m.ID = mt.MasterID
        LEFT JOIN dbo.Employee e ON m.EmployeeRequestID = e.ID
        LEFT JOIN dbo.Project p ON m.ProjectID = p.ID
        LEFT JOIN dbo.HotelBookingProposal pr ON m.ID = pr.HotelBookingManagementID AND ISNULL(pr.IsDeleted, 0) = 0
        LEFT JOIN dbo.Employee eb ON m.EmployeeBookerID = eb.ID
        LEFT JOIN dbo.Employee ep ON ep.ID = pr.ApproveID
    ORDER BY m.EmployeeRequestID, m.ID, pr.ID;

    -- Table 1: Passenger list
    SELECT hbe.HotelBookingManagementID,
           PassengerName = COALESCE(emp.FullName, hbe.FullName),
           PositionName = cv.Name,
           DepartmentName = d.Name
    FROM dbo.HotelBookingEmployee hbe
        INNER JOIN #Matched mt ON hbe.HotelBookingManagementID = mt.MasterID
        LEFT JOIN dbo.Employee emp ON hbe.EmployeeID = emp.ID
        LEFT JOIN dbo.Department d ON emp.DepartmentID = d.ID
        LEFT JOIN dbo.EmployeeChucVu cv ON emp.ChuVuID = cv.ID
    WHERE ISNULL(hbe.IsDeleted, 0) = 0
    ORDER BY hbe.HotelBookingManagementID, hbe.ID;

    DROP TABLE #Matched;
END;
GO
