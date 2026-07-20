using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng lưu danh sách file đã upload cho từng mục checklist của bước dự án
    /// </summary>
    public partial class ProjectGateStepFile
    {
        /// <summary>
        /// Khóa chính tự tăng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID liên kết tới bản ghi ProjectGateStepCheckListDetailLink
        /// </summary>
        public int ProjectGateStepCheckListDetailLinkID { get; set; }

        /// <summary>
        /// Tên file gốc (ví dụ: bao_gia.pdf)
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Đường dẫn đầy đủ tới file trên server (ví dụ: \\192.168.1.190\duan\projects\...)
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Kích thước file tính bằng bytes
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// MIME type của file (ví dụ: application/pdf, image/png)
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Cờ xóa mềm: true = đã xóa
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Người tạo/upload file
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Ngày tạo/upload
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Người cập nhật thông tin file gần nhất
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Ngày cập nhật thông tin file gần nhất
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// ID nhân viên upload file
        /// </summary>
        public int? EmployeeID { get; set; }
    }
}
