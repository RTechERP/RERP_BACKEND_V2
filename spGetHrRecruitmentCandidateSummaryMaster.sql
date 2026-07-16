SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- ==========================================================================================
-- Author:      Antigravity Orchestrator
-- Create date: 2026-06-13
-- Description: Get master summary list of recruitment candidates with completion flags for:
--              1. Application Form (Tờ khai)
--              2. Interview Assessment (Đánh giá phỏng vấn)
--              3. Recruitment Proposal (Tờ trình tuyển dụng)
--              4. Exams/Tests (Bài test)
-- ==========================================================================================
ALTER PROCEDURE [dbo].[spGetHrRecruitmentCandidateSummaryMaster]
    @ID INT,
    @Status INT,
    @EmployeeRequestID INT,
    @DepartmentID INT,
    @DateStart DATETIME = NULL,
    @DateEnd DATETIME = NULL,
    @FilterText NVARCHAR(250) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
          c.ID
         ,c.STT
         ,c.FullName
         ,c.UserName
         ,c.PositionName
         ,c.Status
         ,CASE c.Status
             WHEN 0 THEN N'Ứng tuyển'
             WHEN 1 THEN N'Gửi thư mời'
             WHEN 2 THEN N'Xác nhận phỏng vấn'
             WHEN 3 THEN N'Đã phỏng vấn'
             WHEN 4 THEN N'Kết quả không đạt'
             WHEN 5 THEN N'Kết quả đạt'
             WHEN 6 THEN N'Trình phê duyệt'
             WHEN 7 THEN N'Gửi thư mời nhận việc'
             WHEN 8 THEN N'Xác nhận thư mời'
             WHEN 9 THEN N'Nhận việc'
             ELSE N'Không xác định'
          END AS StatusName
         ,c.DateApply
         ,c.CreatedDate
         ,c.InterviewerName
         ,c.ServerPath
         ,c.FileCVName
         ,d.Name AS DepartmentName
         ,CASE WHEN EXISTS (SELECT 1 FROM HRHiringCandidateInformationForm f WHERE f.HRRecruitmentCandidateID = c.ID AND f.IsDeleted = 0) THEN 1 ELSE 0 END AS HasApplicationForm
         ,CASE WHEN EXISTS (SELECT 1 FROM HRRecruitmentInterviewAssessmentForm a WHERE a.HRRecruitmentCandidateID = c.ID AND a.IsDeleted = 0) THEN 1 ELSE 0 END AS HasInterviewForm
         ,CASE WHEN EXISTS (
            SELECT 1 
            FROM HRRecruitmentApprove ap 
            JOIN HRHiringCandidateInformationForm f ON ap.HRRecruitmentApplicationFormID = f.ID 
            WHERE f.HRRecruitmentCandidateID = c.ID AND ap.IsDeleted = 0 AND f.IsDeleted = 0
          ) THEN 1 ELSE 0 END AS HasApproveForm
         ,CASE WHEN EXISTS (SELECT 1 FROM HRRecruitmentExamResult e WHERE e.EmployeeID = c.ID AND e.IsDeleted = 0) THEN 1 ELSE 0 END AS HasExam
    FROM HRRecruitmentCandidate c
    LEFT JOIN EmployeeChucVuHD pos ON c.EmployeeChucVuHDID = pos.ID
    LEFT JOIN HRHiringRequest req ON c.HRHiringRequestID = req.ID
    LEFT JOIN Department d ON req.DepartmentID = d.ID
    WHERE c.IsDeleted = 0
      AND (@ID = 0 OR c.ID = @ID)
      AND (@Status = -1 OR c.Status = @Status)
      AND (@EmployeeRequestID = -1 OR req.EmployeeRequestID = @EmployeeRequestID OR c.InterviewerID = @EmployeeRequestID)
      AND (@DepartmentID = 0 OR req.DepartmentID = @DepartmentID)
      AND (@DateStart IS NULL OR c.CreatedDate >= @DateStart)
      AND (@DateEnd IS NULL OR c.CreatedDate <= @DateEnd)
      AND (@FilterText = '' OR c.FullName LIKE '%' + @FilterText + '%' OR c.UserName LIKE '%' + @FilterText + '%')
    ORDER BY c.CreatedDate DESC;
END
GO
