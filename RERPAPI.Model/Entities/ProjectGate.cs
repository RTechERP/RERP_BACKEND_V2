using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng khai báo các Gate kiểm soát trong quy trình dự án
    /// </summary>
    public partial class ProjectGate
    {
        /// <summary>
        /// Khóa chính tự tăng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Số thứ tự hiển thị
        /// </summary>
        public int? STT { get; set; }

        /// <summary>
        /// Mã Gate
        /// </summary>
        public string? GateCode { get; set; }

        /// <summary>
        /// Tên Gate
        /// </summary>
        public string? GateName { get; set; }

        /// <summary>
        /// Tên bước trong quy trình
        /// </summary>
        public string? StepName { get; set; }

        /// <summary>
        /// Mục tiêu của Gate
        /// </summary>
        public string? Target { get; set; }

        /// <summary>
        /// Các đầu vào bắt buộc trước khi duyệt Gate
        /// </summary>
        public string? RequireInput { get; set; }

        /// <summary>
        /// Các đầu ra bắt buộc sau khi hoàn thành Gate
        /// </summary>
        public string? RequireOuput { get; set; } // mapped exact typo from database: RequireOuput

        /// <summary>
        /// Vai trò hoặc nhóm người phê duyệt
        /// </summary>
        public string? ApproverRole { get; set; }

        /// <summary>
        /// Hành động xử lý khi Gate không đạt
        /// </summary>
        public string? ActionIfRejected { get; set; }

        /// <summary>
        /// Ngày cập nhật gần nhất
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Loại gate: 1 = Giải pháp, 2 = Triển khai
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// Gate không bắt buộc (tùy chọn)
        /// </summary>
        public bool? Optional { get; set; }

        /// <summary>
        /// ID gate cha (dùng khi Optional = true, để gom nhóm theo cây)
        /// </summary>
        public int? ParentID { get; set; }
    }
}
