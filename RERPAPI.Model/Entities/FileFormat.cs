using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng khai báo danh mục các định dạng file
    /// </summary>
    public partial class FileFormat
    {
        public int ID { get; set; }
        public int? STT { get; set; }
        public string? FormatName { get; set; }
        public string? Extension { get; set; }
        public bool? IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
