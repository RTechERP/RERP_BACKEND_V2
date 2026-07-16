-- Up: Add Description column to ProjectGateStepCheckList
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[ProjectGateStepCheckList]') 
      AND name = N'Description'
)
BEGIN
    ALTER TABLE dbo.ProjectGateStepCheckList ADD Description NVARCHAR(550) NULL;
END
GO

-- Down: Rollback change
-- ALTER TABLE dbo.ProjectGateStepCheckList DROP COLUMN Description;
-- GO
