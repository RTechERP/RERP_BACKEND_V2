using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng lưu cấu hình quy tắc file đính kèm cho từng checklist công đoạn
    /// </summary>
    public partial class ProjectGateStepCheckListDetail
    {
        public int ID { get; set; }
        public int ProjectGateStepCheckListID { get; set; }
        public string? FileRule { get; set; }
        public string? FileFormat { get; set; }
        public int FileQuantity { get; set; }
        public bool IsCheck { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
