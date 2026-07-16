SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[spGetHotelBookingManagementByID]
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Table 0: Master booking request
    SELECT fb.ID,
           fb.STT,
           fb.EmployeeRequestID,
           er.FullName AS RequesterName,
           er.Code,
           fb.Reason,
           fb.ProjectID,
           p.ProjectName,
           fb.Location,
           fb.CheckinDate,
           fb.CheckOutDate,
           fb.EmployeeApproverID,
           fb.EmployeeBookerID,
           eb.FullName AS BookerName,
           fb.DateRequest,
           fb.Note,
           fb.CreatedDate,
           fb.CreatedBy
    FROM dbo.HotelBookingManagement fb
        LEFT JOIN dbo.Employee er
            ON fb.EmployeeRequestID = er.ID
        LEFT JOIN dbo.Project p
            ON fb.ProjectID = p.ID
        LEFT JOIN dbo.Employee eb
            ON fb.EmployeeBookerID = eb.ID
    WHERE fb.ID = @ID AND ISNULL(fb.IsDeleted, 0) = 0;

    -- Table 1: Proposals
    SELECT pr.ID,
           pr.HotelBookingManagementID,
           pr.TypeRoom,
           pr.Quantity,
           pr.UnitPrice,
           pr.TotalAmount,
           pr.Note,
           pr.IsHCNSProposal,
           pr.ReasonHCNSProposal,
           pr.IsApprove,
           pr.ApproveID,
           ep.FullName AS ApproverName,
           pr.ReasonDecline
    FROM dbo.HotelBookingProposal pr
        LEFT JOIN dbo.Employee ep ON pr.ApproveID = ep.ID
    WHERE pr.HotelBookingManagementID = @ID AND ISNULL(pr.IsDeleted, 0) = 0
    ORDER BY pr.ID;
END;
GO
