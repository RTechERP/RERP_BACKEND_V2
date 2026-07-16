ALTER PROCEDURE [dbo].[spGetFlightBookingManagement]
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @Keyword NVARCHAR(500) = NULL,
    @EmployeeID INT = 0,
    @ProjectID INT = 0,
    @EmployeeBookerID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT fb.ID,
           fb.EmployeeID,
           e.FullName AS RequesterName,
           e.FullName AS PassengerName,
           e.Code,
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
           fb.CreatedBy
    FROM dbo.FlightBookingManagement fb
        LEFT JOIN dbo.Employee e
            ON fb.EmployeeID = e.ID
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
              OR fb.EmployeeID = @EmployeeID
          )
          AND
          (
              @ProjectID = 0
              OR fb.ProjectID = @ProjectID
          )
          AND
          (
              @EmployeeBookerID IS NULL
              OR fb.EmployeeBookerID = @EmployeeBookerID
          )
          AND
          (
              @Keyword IS NULL
              OR @Keyword = ''
              OR e.FullName LIKE '%' + @Keyword + '%'
              OR fb.Reason LIKE '%' + @Keyword + '%'
              OR fb.DepartureAddress LIKE '%' + @Keyword + '%'
              OR fb.ArrivesAddress LIKE '%' + @Keyword + '%'
          )
    ORDER BY fb.BookedDate DESC;
END;
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

    SELECT m.ID AS MasterID,
           m.EmployeeID,
           eb.FullName AS RequesterName,
           m.Reason,
           ProjectName=p.ProjectCode+'-'+p.ProjectName,
           e.FullName AS PassengerName,
           cv.Name AS PositionName,
           d.Name AS DepartmentName,
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
		   ReasonHCNSProposal = pr.ReasonHCNSProposal
    FROM FlightBookingManagement m
        LEFT JOIN Employee e
            ON m.EmployeeID = e.ID
        LEFT JOIN Project p
            ON m.ProjectID = p.ID
        LEFT JOIN Department d
            ON e.DepartmentID = d.ID
        LEFT JOIN dbo.EmployeeChucVu cv
            ON e.ChuVuID = cv.ID
        LEFT JOIN FlightBookingProposal pr
            ON m.ID = pr.FlightBookingManagementID
               AND ISNULL(pr.IsDeleted, 0) = 0
        LEFT JOIN Employee eb
            ON m.EmployeeBookerID = eb.ID
        LEFT JOIN Employee ep
            ON ep.ID = pr.ApproveID
    WHERE ISNULL(m.IsDeleted, 0) = 0
          AND
          (
              @SelectedIDs IS NULL
              OR @SelectedIDs = ''
              OR m.ID IN
                 (
                     SELECT CAST(value AS INT)FROM STRING_SPLIT(@SelectedIDs, ',')
                 )
          )
          AND
          (
              @ProjectID IS NULL
              OR @ProjectID = 0
              OR m.ProjectID = @ProjectID
          )
          AND
          (
              @EmployeeBookerID IS NULL
              OR m.EmployeeBookerID = @EmployeeBookerID
          )
          AND
          (
              @Keyword IS NULL
              OR @Keyword = ''
              OR e.FullName LIKE '%' + @Keyword + '%'
              OR m.Reason LIKE '%' + @Keyword + '%'
              OR p.ProjectName LIKE '%' + @Keyword + '%'
          )
    ORDER BY m.EmployeeID,
             m.ID,
             pr.ID;
END;
GO
