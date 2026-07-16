-- 1. Xóa default constraint của cột ReturnDate nếu có
DECLARE @ConstraintName NVARCHAR(250)
SELECT @ConstraintName = name
FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('dbo.FlightBookingProposal')
  AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('dbo.FlightBookingProposal'), 'ReturnDate', 'ColumnId')

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE dbo.FlightBookingProposal DROP CONSTRAINT ' + @ConstraintName)
END

-- 2. Thay đổi kiểu dữ liệu của ReturnDate sang DATETIME để lưu trữ ngày tháng
ALTER TABLE dbo.FlightBookingProposal ALTER COLUMN ReturnDate DATETIME NULL;
GO
