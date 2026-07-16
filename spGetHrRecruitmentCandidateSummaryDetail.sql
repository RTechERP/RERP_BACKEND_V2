SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

ALTER PROCEDURE [dbo].[spGetHrRecruitmentCandidateSummaryDetail]
    @HRRecruitmentCandidateID INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FormID INT;
    SELECT TOP 1 @FormID = ID FROM dbo.HRRecruitmentApplicationForm WHERE HRRecruitmentCandidateID = @HRRecruitmentCandidateID AND IsDeleted = 0;

    -- 1-7. Tờ khai Master (0) & các chi tiết (1-6)
    EXEC dbo.spGetHRRecruitmentApplicationForm @HRRecruitmentCandidateID;

    -- 8. Đánh giá phỏng vấn Master (Dataset 7)
    SELECT * FROM dbo.HRRecruitmentInterviewAssessmentForm WHERE HRRecruitmentCandidateID = @HRRecruitmentCandidateID AND IsDeleted = 0;

    -- 9. Đánh giá phỏng vấn Candidate Info (Dataset 8)
    SELECT 
        c.ID AS HRRecruitmentCandidateID,
        c.FullName,
        c.DateOfBirth,
        c.Gender,
        c.PhoneNumber,
        c.Email,
        c.PositionName,
        c.DateInterview,
        c.Status AS CandidateStatus,
        c.InterviewerID,
        emp.FullName AS FullNameInterview,
        cv.Name AS ChucVuInterview
    FROM dbo.HRRecruitmentCandidate c
    LEFT JOIN dbo.Employee emp ON c.InterviewerID = emp.ID
    LEFT JOIN dbo.EmployeeChucVuHD cv ON emp.ChucVuHDID = cv.ID
    WHERE c.ID = @HRRecruitmentCandidateID AND c.IsDeleted = 0;

    -- 10. Tờ trình phê duyệt tuyển dụng (Dataset 9)
    SELECT * FROM dbo.HRRecruitmentApprove WHERE HRRecruitmentApplicationFormID = @FormID AND IsDeleted = 0;

    -- 11. Kết quả bài test (Dataset 10)
    EXEC dbo.spGetExamByCandidate @HRRecruitmentCandidateID;
END
GO
