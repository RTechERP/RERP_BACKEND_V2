ALTER PROCEDURE [dbo].[spGetProjectGateStepLinkByProject]
    @ProjectID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Query 1: Get step links
    SELECT 
        l.ID,
        l.ProjectID,
        l.ProjectGateStepID,
        l.ProjectTypeID,
        l.StartDate,
        l.IsRepeat,
        l.IsApproved,
        l.ApprovedBy,
        l.ApprovedDate,
        l.ApprovalComment
    FROM ProjectGateStepLink l
    WHERE l.ProjectID = @ProjectID AND (l.IsDeleted = 0 OR l.IsDeleted IS NULL)
    ORDER BY l.ProjectTypeID, l.ProjectGateStepID;

    -- Query 2: Get workers for those links
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
    WHERE l.ProjectID = @ProjectID AND (l.IsDeleted = 0 OR l.IsDeleted IS NULL);

    -- Query 3: Get checklists + uploaded files for those links
    SELECT 
        ISNULL(cl.ID, 0)                    AS CheckListLinkID,
        l.ID                                AS ProjectGateStepLinkID,
        c.ID                                AS ProjectGateStepCheckListID,
        cl.PathFolder,
        c.IsRequired,
        c.Description,
        c.Type,
        CAST(
            CASE 
                WHEN c.Type IN (N'File_Path', N'File') THEN 
                    CASE WHEN EXISTS (
                        SELECT 1 
                        FROM ProjectGateStepFile f2 
                        WHERE f2.ProjectGateStepCheckListLinkID = cl.ID 
                          AND (f2.IsDeleted = 0 OR f2.IsDeleted IS NULL)
                    ) THEN 1 ELSE 0 END
                WHEN c.Type IN (N'Part_list', N'PartList') THEN 
                    CASE WHEN EXISTS (
                        SELECT 1 
                        FROM ProjectPartList ppl 
                        WHERE ppl.ProjectID = l.ProjectID 
                          AND (ppl.IsDeleted = 0 OR ppl.IsDeleted IS NULL)
                    ) THEN 1 ELSE 0 END
                ELSE ISNULL(cl.IsPass, 0)
            END AS BIT) AS IsPass,
        f.ID                                AS FileID,
        f.FileName,
        f.FilePath,
        f.FileSize,
        f.ContentType,
        f.CreatedBy,
        f.CreatedDate
    FROM ProjectGateStepLink l
    INNER JOIN ProjectGateStepCheckList c ON c.ProjectGateStepID = l.ProjectGateStepID
        AND (c.ProjectTypeID IS NULL OR c.ProjectTypeID = l.ProjectTypeID)
        AND (c.DepartmentID IS NULL OR c.DepartmentID IN (
            SELECT pgd.DepartmentID 
            FROM ProjectGateDepartment pgd 
            WHERE pgd.ProjectGateStepID = l.ProjectGateStepID
        ))
    LEFT JOIN ProjectGateStepCheckListLink cl 
           ON cl.ProjectGateStepLinkID = l.ID 
          AND cl.ProjectGateStepCheckListID = c.ID
    LEFT JOIN ProjectGateStepFile f 
           ON f.ProjectGateStepCheckListLinkID = cl.ID 
          AND (f.IsDeleted = 0 OR f.IsDeleted IS NULL)
    WHERE l.ProjectID = @ProjectID AND (l.IsDeleted = 0 OR l.IsDeleted IS NULL)
    ORDER BY l.ID, c.ID, f.CreatedDate;
END;