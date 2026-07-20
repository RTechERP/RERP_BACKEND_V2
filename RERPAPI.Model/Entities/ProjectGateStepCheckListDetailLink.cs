using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng lưu vết việc nạp và duyệt từng quy tắc file chi tiết của công đoạn dự án
    /// </summary>
    public partial class ProjectGateStepCheckListDetailLink
    {
        public int ID { get; set; }
        public int ProjectGateStepLinkID { get; set; }
        public int ProjectGateStepCheckListDetailID { get; set; }
        public bool IsCompleted { get; set; }
        public int IsApprovedTBP { get; set; }
        public int? ApprovedTBPBy { get; set; }
        public DateTime? ApprovedTBPDate { get; set; }
        //public string? PathFolder { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
