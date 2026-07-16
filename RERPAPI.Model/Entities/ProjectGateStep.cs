using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng khai báo các bước/công đoạn thực hiện theo Gate của dự án
    /// </summary>
    public partial class ProjectGateStep
    {
        /// <summary>
        /// Khóa chính tự tăng của bảng ProjectGateStep
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID bảng ProjectGate
        /// </summary>
        public int? ProjectGateID { get; set; }

        /// <summary>
        /// ID bảng ProjectGateDepartment (FK dự phòng theo schema)
        /// </summary>
        public int? ProjectGateDepartmentID { get; set; }

        /// <summary>
        /// Nội dung công việc, bước thực hiện hoặc yêu cầu cần xử lý trong Gate
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// ID chức vụ thực hiện hoặc phụ trách bước/công đoạn này
        /// </summary>
        public int? ChucVuID { get; set; }

        /// <summary>
        /// Thứ tự hiển thị của bước/công đoạn trên giao diện hoặc báo cáo
        /// </summary>
        public string? TT { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp dữ liệu của bước/công đoạn
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Đánh dấu bước/công đoạn có được lặp lại hay không: 0 - Không, 1 - Có
        /// </summary>
        public bool? IsRepeat { get; set; }

        /// <summary>
        /// Ngày cập nhật gần nhất của bản ghi
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// ID người cập nhật gần nhất bản ghi
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// ID người tạo bản ghi
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// ID bảng ProjectGateStepTemplate
        /// </summary>
        public int? ProjectGateStepTemplateID { get; set; }

        /// <summary>
        /// ID phòng ban thực hiện công đoạn
        /// </summary>
        public int? DepartmentID { get; set; }
    }
}
