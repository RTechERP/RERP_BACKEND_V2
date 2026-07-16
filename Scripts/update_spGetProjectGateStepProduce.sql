CREATE OR ALTER PROCEDURE [dbo].[spGetProjectGateStepProduce]
AS
BEGIN
    SET NOCOUNT ON;

    -- Query 1: Gates
    SELECT [ID], [GateCode], [GateName], [Type]
    FROM [dbo].[ProjectGate]
    ORDER BY ISNULL([STT], 2147483647);

    -- Query 2: Departments
    SELECT [ID], [Code], [Name]
    FROM [dbo].[Department]
    WHERE [IsDeleted] <> 1 OR [IsDeleted] IS NULL
    ORDER BY [STT];

    -- Query 3: Positions (EmployeeChucVu)
    SELECT [ID], [Code], [Name]
    FROM [dbo].[EmployeeChucVu]
    WHERE [IsDeleted] <> 1 OR [IsDeleted] IS NULL
    ORDER BY [PriorityOrder];

    -- Query 4: Templates (ProjectGateStepTemplate)
    SELECT 
        t.[ID], 
        t.[Code], 
        t.[Name], 
        t.[ProjectTypeDepartmentID],
        ptd.[ProjectTypeID],
        ptd.[DepartmentID],
        ISNULL(d.[Name], '') AS [DepartmentName],
        ISNULL(pt.[ProjectTypeName], '') AS [ProjectTypeName]
    FROM [dbo].[ProjectGateStepTemplate] t
    LEFT JOIN [dbo].[ProjectTypeDepartment] ptd ON t.[ProjectTypeDepartmentID] = ptd.[ID]
    LEFT JOIN [dbo].[Department] d ON ptd.[DepartmentID] = d.[ID]
    LEFT JOIN [dbo].[ProjectType] pt ON ptd.[ProjectTypeID] = pt.[ID]
    ORDER BY t.[Code];
END
GO
