SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[spGetFlightBookingExportExcel]
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
    FROM FlightBookingManagement m
        LEFT JOIN Employee e ON m.EmployeeID = e.ID
        LEFT JOIN Project p ON m.ProjectID = p.ID
        LEFT JOIN Employee eb ON m.EmployeeBookerID = eb.ID
    WHERE ISNULL(m.IsDeleted, 0) = 0
          AND (
              @StartDate IS NULL
              OR CAST(m.BookedDate AS DATE) >= CAST(@StartDate AS DATE)
          )
          AND (
              @EndDate IS NULL
              OR CAST(m.BookedDate AS DATE) <= CAST(@EndDate AS DATE)
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
              OR EXISTS (
                  SELECT 1
                  FROM dbo.FlightBookingPassenger fbp
                  LEFT JOIN dbo.Employee emp ON fbp.EmployeeID = emp.ID
                  WHERE fbp.FlightBookingManagementID = m.ID
                    AND (
                        emp.FullName LIKE '%' + @Keyword + '%'
                        OR fbp.FullName LIKE '%' + @Keyword + '%'
                    )
              )
          );

    -- Table 0: Master and Proposal details
    SELECT m.ID AS MasterID,
           m.EmployeeID,
           eb.FullName AS RequesterName,
           m.Reason,
           ProjectName = p.ProjectCode + '-' + p.ProjectName,
           PassengerName = (
               SELECT STRING_AGG(COALESCE(emp.FullName, CAST(fbp.FullName AS NVARCHAR(250))), ', ')
               FROM dbo.FlightBookingPassenger fbp
               LEFT JOIN dbo.Employee emp ON fbp.EmployeeID = emp.ID
               WHERE fbp.FlightBookingManagementID = m.ID
           ),
           m.DepartureAddress,
           m.ArrivesAddress,
           pr.DepartureTime,
           pr.DepartureDate,
           pr.Airline,
           pr.Price,
           pr.Baggage,
           pr.IsApprove,
           pr.HCNSProposal,
           eb.FullName AS BookerName,
           m.CreatedDate AS BookedDate,
           m.Note,
           ApproverName = ep.FullName,
           ReasonHCNSProposal = pr.ReasonHCNSProposal,
           m.IsRoundTrip,
           pr.ReturnDate,
           pr.ReturnTime
    FROM FlightBookingManagement m
        INNER JOIN #Matched mt ON m.ID = mt.MasterID
        LEFT JOIN Employee e ON m.EmployeeID = e.ID
        LEFT JOIN Project p ON m.ProjectID = p.ID
        LEFT JOIN FlightBookingProposal pr ON m.ID = pr.FlightBookingManagementID AND ISNULL(pr.IsDeleted, 0) = 0
        LEFT JOIN Employee eb ON m.EmployeeBookerID = eb.ID
        LEFT JOIN Employee ep ON ep.ID = pr.ApproveID
    ORDER BY m.EmployeeID, m.ID, pr.ID;

    -- Table 1: Passenger list
    SELECT fbp.FlightBookingManagementID,
           PassengerName = COALESCE(emp.FullName, fbp.FullName),
           PositionName = cv.Name,
           DepartmentName = d.Name
    FROM dbo.FlightBookingPassenger fbp
        INNER JOIN #Matched mt ON fbp.FlightBookingManagementID = mt.MasterID
        LEFT JOIN dbo.Employee emp ON fbp.EmployeeID = emp.ID
        LEFT JOIN dbo.Department d ON emp.DepartmentID = d.ID
        LEFT JOIN dbo.EmployeeChucVu cv ON emp.ChuVuID = cv.ID
    ORDER BY fbp.FlightBookingManagementID, fbp.ID;

    DROP TABLE #Matched;
END;
GO
