IF COL_LENGTH(N'dbo.PollQuestion', N'DataSourceType') IS NULL
BEGIN
    ALTER TABLE dbo.PollQuestion ADD DataSourceType NVARCHAR(50) NULL;
END;
GO

IF COL_LENGTH(N'dbo.PollQuestion', N'DataSourceField') IS NULL
BEGIN
    ALTER TABLE dbo.PollQuestion ADD DataSourceField NVARCHAR(100) NULL;
END;
GO
