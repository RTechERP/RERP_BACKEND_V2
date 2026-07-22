using System;
using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    public class SaveProjectGateStepRequestDto
    {
        public int ProjectID { get; set; }
        public List<ProjectGateStepLinkDto> Steps { get; set; }
    }

    public class ProjectGateStepLinkDto
    {
        public int ID { get; set; }
        public int ProjectGateStepID { get; set; }
        public int ProjectTypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsRepeat { get; set; }
        public decimal? DayCount { get; set; }
        public int? PeopleCount { get; set; }
        public string? Content { get; set; }
        public List<ProjectGateStepWorkerDto> Workers { get; set; }
        public List<ProjectGateStepCheckListLinkDto> CheckLists { get; set; }
        public bool? IsApproved { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovalComment { get; set; }
        public int? ProjectGateStepTemplateID { get; set; }
        public int? DepartmentID { get; set; }
        public int? ProjectTaskID { get; set; }
    }

    public class ProjectGateStepCheckListLinkDto
    {
        public int ID { get; set; }
        public int? ProjectGateStepCheckListID { get; set; }
        public string? PathFolder { get; set; }
        public bool? IsPass { get; set; }
        public bool IsRequired { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        
        // Custom rule-level attributes
        public bool IsFile { get; set; } = true;
        public int? STT { get; set; }
        public string? FileName { get; set; }
        
        public int FileQuantity { get; set; }
        
        // Rule-level approvals
        public int IsApprovedTBP { get; set; } = 0;
        public int? ApprovedTBPBy { get; set; }
        public DateTime? ApprovedTBPDate { get; set; }
        
        public List<ProjectGateStepFileDto> Files { get; set; } = new();
    }

    public class ProjectGateStepFileDto
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ProjectGateStepCheckListDetailLinkID { get; set; }
        public string? CreatorFullName { get; set; }
        public int? EmployeeID { get; set; }
    }

    public class ProjectGateStepWorkerDto
    {
        public int EmployeeID { get; set; }
        public decimal? DayCount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    // ── Flat DTOs cho Dapper mapping từ stored procedure ──

    /// <summary>
    /// DTO phẳng map từ Query 1 của spGetProjectGateStepLinkByProject
    /// </summary>
    public class ProjectGateStepLinkResultDto
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public int ProjectGateStepID { get; set; }
        public int ProjectTypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsRepeat { get; set; }
        public int? ProjectTaskID { get; set; }
        public bool? IsApproved { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovalComment { get; set; }
        public int? ProjectGateStepTemplateID { get; set; }
        public int? DepartmentID { get; set; }
    }

    /// <summary>
    /// DTO phẳng map từ Query 2 của spGetProjectGateStepLinkByProject
    /// </summary>
    public class ProjectGateStepWorkerResultDto
    {
        public int ID { get; set; }
        public int ProjectGateStepLinkID { get; set; }
        public int EmployeeID { get; set; }

        public decimal? DayCount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public string EmployeeName { get; set; }
    }

    /// <summary>
    /// DTO phẳng map từ Query 3 của spGetProjectGateStepLinkByProject
    /// (mỗi dòng = 1 checklist + 1 file, có thể NULL nếu checklist chưa có file)
    /// </summary>
    public class ProjectGateStepCheckListFileResultDto
    {
        public int CheckListLinkID { get; set; }
        public int ProjectGateStepLinkID { get; set; }
        public int? ProjectGateStepCheckListID { get; set; }
        public string? PathFolder { get; set; }
        public bool? IsPass { get; set; }
        public bool IsRequired { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        
        // Custom rule-level attributes
        public bool IsFile { get; set; }
        public int? STT { get; set; }
        public string? StandardFileName { get; set; }
        
        // Rule-level approvals
        public int IsApprovedTBP { get; set; }
        public int? ApprovedTBPBy { get; set; }
        public DateTime? ApprovedTBPDate { get; set; }

        // File info (có thể NULL nếu chưa upload file)
        public int? FileID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CompleteRuleDto
    {
        public List<int> DetailLinkIDs { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class ApproveRuleDto
    {
        public int IsApprovedTBP { get; set; }
        public int ApprovedTBPBy { get; set; }
    }

    public class FileCheckViolationDto
    {
        public int DetailLinkID { get; set; }
        public string? Description { get; set; }
        public string? FileName { get; set; }
        public int RequiredQuantity { get; set; }
        public int UploadedQuantity { get; set; }
    }

    /// <summary>
    /// DTO cho duyệt/hủy duyệt nhiều công đoạn
    /// </summary>
    public class ApproveMultipleDto
    {
        public List<int> LinkIDs { get; set; } = new();
        public bool IsApproved { get; set; }
        /// <summary>
        /// Nếu true: bỏ qua cảnh báo checklist TBP chưa duyệt và duyệt luôn
        /// </summary>
        public bool ForceApprove { get; set; } = false;
    }

    /// <summary>
    /// Kết quả trả về khi check pending TBP trước khi duyệt
    /// </summary>
    public class ApproveMultipleResultDto
    {
        public bool Success { get; set; }
        public bool HasPendingTBP { get; set; }
        public string? Message { get; set; }
    }

    public class ProjectGateStepPendingCheckDto
    {
        public int PendingCount { get; set; }
    }
}
