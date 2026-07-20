using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Danh mục loại checklist của Project Gate
    /// </summary>
    public partial class ProjectGateCheckListType
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Số thứ tự hiển thị
        /// </summary>
        public int? STT { get; set; }

        /// <summary>
        /// Mã loại checklist
        /// </summary>
        public string TypeCode { get; set; } = null!;

        /// <summary>
        /// Mô tả loại checklist
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Cờ đánh dấu xóa mềm (0: Hoạt động, 1: Đã xóa)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Ngày cập nhật gần nhất
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Người cập nhật gần nhất
        /// </summary>
        public string? UpdatedBy { get; set; }
    }
}
