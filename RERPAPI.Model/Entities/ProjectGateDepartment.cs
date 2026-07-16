namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng khai báo phòng ban liên kết với từng bước/công đoạn trong Gate của dự án
    /// </summary>
    public partial class ProjectGateDepartment
    {
        /// <summary>
        /// Khóa chính tự tăng của bảng ProjectGateDepartment
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID bảng Department, dùng để xác định phòng ban phụ trách hoặc tham gia bước/công đoạn
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// ID bảng ProjectGateStep, dùng để liên kết phòng ban với từng bước/công đoạn trong Gate
        /// </summary>
        public int? ProjectGateStepID { get; set; }
    }
}
