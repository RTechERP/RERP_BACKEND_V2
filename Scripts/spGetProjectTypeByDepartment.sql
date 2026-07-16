CREATE OR ALTER PROCEDURE [dbo].[spGetProjectTypeByDepartment]
	@DepartmentID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		pt.[ID],
		pt.[ProjectTypeCode],
		pt.[ProjectTypeName],
		pt.[ParentID],
		pt.[RootFolder],
		pt.[ApprovedTBPID],
		pt.[IsDeleted],
		pt.[IsHide],
		pt.[IsHidePartlist],
		pt.[IsPurchase],
		pt.[IsProject],
		pt.[DepartmentID],
		ptd.[ID] AS [ProjectTypeDepartmentID]
	FROM [dbo].[ProjectTypeDepartment] ptd
	INNER JOIN [dbo].[ProjectType] pt ON ptd.[ProjectTypeID] = pt.[ID]
	WHERE ptd.[DepartmentID] = @DepartmentID
	  AND ptd.[IsDeleted] = 0
	  AND pt.[IsDeleted] = 0;
END
GO
