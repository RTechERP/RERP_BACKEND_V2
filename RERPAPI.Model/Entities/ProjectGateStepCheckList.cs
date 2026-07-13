using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng khai báo danh sách checklist của từng bước (Gate Step) trong quy trình dự án.
    /// </summary>
    public partial class ProjectGateStepCheckList
    {
        /// <summary>
        /// Khóa chính tự tăng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID bước công việc thuộc quy trình Gate
        /// </summary>
        public int? ProjectGateStepID { get; set; }

        /// <summary>
        /// Loại checklist: File, PartList, Document, Folder, Form...
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Đường dẫn thư mục hoặc file cần sử dụng cho checklist
        /// </summary>
        public string? PathFolder { get; set; }

        /// <summary>
        /// Mô tả của checklist
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Ngày cập nhật
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
    }
}
