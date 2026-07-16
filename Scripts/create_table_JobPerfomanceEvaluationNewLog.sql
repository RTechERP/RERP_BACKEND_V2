-- ============================================================
-- Script: Tạo bảng JobPerfomanceEvaluationNewLog
-- Mục đích: Lưu lịch sử thao tác của phiếu đánh giá chuyển hợp đồng
--           (HR tạo phiếu, NLĐ/TBP/HR/BGĐ xác nhận, sửa điểm, xóa phiếu, ...)
-- Chạy trên database: RTC_BackupKhanhTest (hoặc DB production tương ứng)
-- ============================================================

USE [RTC_BackupKhanhTest]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JobPerfomanceEvaluationNewLog](
    [ID]                              [INT] IDENTITY(1,1) NOT NULL,
    [JobPerfomanceEvaluationNewID]    [INT] NULL,
    [EmployeeID]                      [INT] NULL,
    [ActionType]                      [NVARCHAR](100) NULL,
    [ContentLog]                      [NVARCHAR](MAX) NULL,
    [CreatedBy]                       [NVARCHAR](200) NULL,
    [CreatedDate]                     [DATETIME] NULL,
    [UpdatedBy]                       [NVARCHAR](200) NULL,
    [UpdatedDate]                     [DATETIME] NULL,
    [IsDeleted]                       [BIT] NULL,
    CONSTRAINT [PK__JobPerfo__3214EC27_LOG_NEW] PRIMARY KEY CLUSTERED
    (
        [ID] ASC
    ) WITH (
        PAD_INDEX = OFF,
        STATISTICS_NORECOMPUTE = OFF,
        IGNORE_DUP_KEY = OFF,
        ALLOW_ROW_LOCKS = ON,
        ALLOW_PAGE_LOCKS = ON,
        OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
    ) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[JobPerfomanceEvaluationNewLog]
ADD DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[JobPerfomanceEvaluationNewLog]
ADD DEFAULT (getdate()) FOR [CreatedDate]
GO

-- Comment cho bảng và các cột
EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Bảng lưu lịch sử các thao tác của phiếu đánh giá chuyển hợp đồng',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'JobPerfomanceEvaluationNewLog'
GO

EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'ID phiếu đánh giá chuyển hợp đồng (JobPerfomanceEvaluationNew.ID)',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'JobPerfomanceEvaluationNewLog',
    @level2type = N'COLUMN', @level2name = N'JobPerfomanceEvaluationNewID'
GO

EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'ID nhân viên được đánh giá',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'JobPerfomanceEvaluationNewLog',
    @level2type = N'COLUMN', @level2name = N'EmployeeID'
GO

EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Loại thao tác: HR tạo phiếu, NLĐ/TBP/HR/BGĐ xác nhận, ...',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'JobPerfomanceEvaluationNewLog',
    @level2type = N'COLUMN', @level2name = N'ActionType'
GO

EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Nội dung chi tiết của thao tác được ghi nhận',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'JobPerfomanceEvaluationNewLog',
    @level2type = N'COLUMN', @level2name = N'ContentLog'
GO

EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Cờ đánh dấu xóa mềm: 0 - Đang sử dụng, 1 - Đã xóa',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'JobPerfomanceEvaluationNewLog',
    @level2type = N'COLUMN', @level2name = N'IsDeleted'
GO

-- Index gợi ý để truy vấn nhanh theo phiếu + thời gian
CREATE NONCLUSTERED INDEX [IX_JobPerfomanceEvaluationNewLog_FormID_CreatedDate]
    ON [dbo].[JobPerfomanceEvaluationNewLog]
    (
        [JobPerfomanceEvaluationNewID] ASC,
        [CreatedDate] DESC
    )
    INCLUDE ([ActionType], [ContentLog], [CreatedBy], [EmployeeID])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF,
          DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
    ON [PRIMARY]
GO

PRINT N'Tạo bảng JobPerfomanceEvaluationNewLog thành công!'
GO