using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng khai báo chức vụ được phép thực hiện hoặc phụ trách một bước (Step) trong quy trình Gate của dự án
    /// </summary>
    public partial class ProjectGateStepPosition
    {
        public int ID { get; set; }
        public int ProjectGateStepID { get; set; }
        public int ChucVuID { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
