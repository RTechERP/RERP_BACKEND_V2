SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO

ALTER PROCEDURE [dbo].[spGetProjectGateSteps]
    @GateID INT = NULL,
    @DepartmentID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.ID,
        s.ProjectGateID,
        g.GateCode,
        g.GateName,
        s.Content,
        s.ChucVuID,
        s.TT,
        s.SortOrder,
        s.IsRepeat,
        s.ProjectGateStepTemplateID,
        t.Code AS TemplateCode,
        t.Name AS TemplateName,
        s.UpdatedDate,
        TRY_CAST(s.UpdatedBy AS INT) AS UpdatedBy,
        s.CreatedDate,
        TRY_CAST(s.CreatedBy AS INT) AS CreatedBy,
        
        -- Department names and IDs
        (
            SELECT STRING_AGG(d.Name, ', ') 
            FROM ProjectGateDepartment pgd
            INNER JOIN Department d ON pgd.DepartmentID = d.ID
            WHERE pgd.ProjectGateStepID = s.ID AND (d.IsDeleted = 0 OR d.IsDeleted IS NULL)
        ) AS DepartmentNames,
        (
            SELECT STRING_AGG(CAST(pgd.DepartmentID AS VARCHAR), ',') 
            FROM ProjectGateDepartment pgd
            INNER JOIN Department d ON pgd.DepartmentID = d.ID
            WHERE pgd.ProjectGateStepID = s.ID AND (d.IsDeleted = 0 OR d.IsDeleted IS NULL)
        ) AS DepartmentIDsStr,
        
        -- Position names and IDs
        (
            SELECT STRING_AGG(cv.Name, ', ') 
            FROM ProjectGateStepPosition pgp
            INNER JOIN EmployeeChucVu cv ON pgp.ChucVuID = cv.ID
            WHERE pgp.ProjectGateStepID = s.ID AND (cv.IsDeleted = 0 OR cv.IsDeleted IS NULL)
        ) AS PositionNames,
        (
            SELECT STRING_AGG(CAST(pgp.ChucVuID AS VARCHAR), ',') 
            FROM ProjectGateStepPosition pgp
            INNER JOIN EmployeeChucVu cv ON pgp.ChucVuID = cv.ID
            WHERE pgp.ProjectGateStepID = s.ID AND (cv.IsDeleted = 0 OR cv.IsDeleted IS NULL)
        ) AS PositionIDsStr,

        -- Checklist names and serialized checklists list
        (
            SELECT STRING_AGG(c.Description, ', ') 
            FROM ProjectGateStepCheckList c
            WHERE c.ProjectGateStepID = s.ID AND c.Description IS NOT NULL AND c.Description <> ''
        ) AS CheckListNames,
        (
            SELECT c.ID, c.ProjectGateStepID, c.Type, c.PathFolder, c.Description
            FROM ProjectGateStepCheckList c
            WHERE c.ProjectGateStepID = s.ID
            FOR JSON PATH
        ) AS ChecklistsJson

    FROM ProjectGateStep s
    LEFT JOIN ProjectGate g ON s.ProjectGateID = g.ID
    LEFT JOIN ProjectGateStepTemplate t ON s.ProjectGateStepTemplateID = t.ID
    WHERE (@GateID IS NULL OR s.ProjectGateID = @GateID)
      AND (@DepartmentID IS NULL OR s.ID IN (SELECT ProjectGateStepID FROM ProjectGateDepartment WHERE DepartmentID = @DepartmentID))
    ORDER BY COALESCE(s.SortOrder, 2147483647), s.TT;
END;
GO
