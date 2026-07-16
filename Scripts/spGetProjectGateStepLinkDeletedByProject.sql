SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [dbo].[spGetProjectGateStepLinkDeletedByProject]
    @ProjectID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Query 1: Get deleted step links
    SELECT 
        l.ID,
        l.ProjectID,
        l.ProjectGateStepID,
        l.ProjectTypeID,
        l.StartDate,
        l.IsRepeat
    FROM ProjectGateStepLink l
    WHERE l.ProjectID = @ProjectID AND l.IsDeleted = 1
    ORDER BY l.ProjectTypeID, l.ProjectGateStepID;

    -- Query 2: Get workers for deleted links
    SELECT 
        w.ID,
        w.ProjectGateStepLinkID,
        w.EmployeeID,
        w.DayCount,
        w.UnitPrice,
        w.TotalAmount,
        e.FullName AS EmployeeName
    FROM ProjectGateStepWorker w
    INNER JOIN ProjectGateStepLink l ON w.ProjectGateStepLinkID = l.ID
    LEFT JOIN Employee e ON w.EmployeeID = e.ID
    WHERE l.ProjectID = @ProjectID AND l.IsDeleted = 1;
END;
GO
