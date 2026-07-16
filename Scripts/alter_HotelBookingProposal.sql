-- Chạy script này trong SQL Server để thêm các cột duyệt phương án cho bảng HotelBookingProposal
ALTER TABLE dbo.HotelBookingProposal
ADD IsApprove INT NULL CONSTRAINT DF_HotelBookingProposal_IsApprove DEFAULT 0,
    ApproveID INT NULL,
    ReasonDecline NVARCHAR(550) NULL;
GO
