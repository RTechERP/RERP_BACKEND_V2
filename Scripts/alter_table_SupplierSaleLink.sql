-- Add Website column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SupplierSaleLink]') AND name = N'Website')
BEGIN
    ALTER TABLE dbo.SupplierSaleLink ADD Website NVARCHAR(550) NULL;
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'Website của đại lý/nhà cung cấp',
        @level0type = N'SCHEMA', @level0name = 'dbo',
        @level1type = N'TABLE',  @level1name = 'SupplierSaleLink',
        @level2type = N'COLUMN', @level2name = 'Website';
END
GO

-- Add AgencyTime column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SupplierSaleLink]') AND name = N'AgencyTime')
BEGIN
    ALTER TABLE dbo.SupplierSaleLink ADD AgencyTime NVARCHAR(550) NULL;
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'Thời điểm trở thành đại lý (nhập tự do)',
        @level0type = N'SCHEMA', @level0name = 'dbo',
        @level1type = N'TABLE',  @level1name = 'SupplierSaleLink',
        @level2type = N'COLUMN', @level2name = 'AgencyTime';
END
GO

-- Add IsAgencyCertified column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SupplierSaleLink]') AND name = N'IsAgencyCertified')
BEGIN
    ALTER TABLE dbo.SupplierSaleLink ADD IsAgencyCertified BIT NULL;
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'Đánh dấu đại lý có chứng nhận hay không (Yes/No)',
        @level0type = N'SCHEMA', @level0name = 'dbo',
        @level1type = N'TABLE',  @level1name = 'SupplierSaleLink',
        @level2type = N'COLUMN', @level2name = 'IsAgencyCertified';
END
GO
