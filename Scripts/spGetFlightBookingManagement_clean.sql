Text
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- =============================================

-- Author:		<Author:NXL>

-- Create date: <23/04/2026>

-- Description:	<L?y danh sách master b?ng booking vé mb>

-- =============================================

CREATE   PROCEDURE [dbo].[spGetFlightBookingManagement]

    @StartDate DATETIME = NULL,

    @EndDate DATETIME = NULL,

    @Keyword NVARCHAR(500) = NULL,

    @EmployeeID INT = 0,

    @ProjectID INT = 0

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

              @Keyword IS NULL

              OR @Keyword = ''

              OR e.FullName LIKE '%' + @Keyword + '%'

              OR fb.Reason LIKE '%' + @Keyword + '%'

              OR fb.DepartureAddress LIKE '%' + @Keyword + '%'

              OR fb.ArrivesAddress LIKE '%' + @Keyword + '%'

          )

    ORDER BY fb.BookedDate DESC;

END;

