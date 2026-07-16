SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [dbo].[spResetProjectGateStepLink]
    @ProjectID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Soft-delete các ProjectItem được tạo từ GateStep
    UPDATE pi 
    SET pi.IsDeleted = 1
    FROM ProjectItem pi
    INNER JOIN ProjectGateStepLink l ON pi.ID = l.ProjectTaskID
    WHERE l.ProjectID = @ProjectID AND l.ProjectTaskID IS NOT NULL AND (l.IsDeleted = 0 OR l.IsDeleted IS NULL);

    -- Cập nhật IsDeleted = 1 cho các ProjectGateStepLink cũ
    UPDATE ProjectGateStepLink 
    SET IsDeleted = 1
    WHERE ProjectID = @ProjectID AND (IsDeleted = 0 OR IsDeleted IS NULL);
END;
GO
