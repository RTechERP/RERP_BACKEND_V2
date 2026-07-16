using System;
using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    /// <summary>
    /// DTO cho ProjectGateStep — bổ sung DepartmentIDs (save) và DepartmentNames (hiển thị, stuffed)
    /// </summary>
    public class ProjectGateStepDTO
    {
        public int ID { get; set; }

        /// <summary>ID của Gate cha</summary>
        public int? ProjectGateID { get; set; }

        /// <summary>Tên Gate cha (join, dùng để hiển thị)</summary>
        public string? GateName { get; set; }

        /// <summary>Mã Gate cha (join, dùng để hiển thị)</summary>
        public string? GateCode { get; set; }

        /// <summary>Nội dung công việc / bước thực hiện</summary>
        public string? Content { get; set; }

        /// <summary>ID chức vụ phụ trách</summary>
        public int? ChucVuID { get; set; }

        /// <summary>Tên chức vụ (join, dùng để hiển thị)</summary>
        public string? ChucVuName { get; set; }

        /// <summary>Thứ tự hiển thị</summary>
        public string? TT { get; set; }

        /// <summary>Thứ tự sắp xếp</summary>
        public int? SortOrder { get; set; }

        /// <summary>Có lặp lại không</summary>
        public bool? IsRepeat { get; set; }

        /// <summary>ID của Template</summary>
        public int? ProjectGateStepTemplateID { get; set; }

        public string? TemplateCode { get; set; }
        public string? TemplateName { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        /// <summary>Danh sách ID phòng ban — dùng khi lưu (multi-select FE gửi lên)</summary>
        public List<int> DepartmentIDs { get; set; } = new();

        /// <summary>Tên phòng ban stuffed thành 1 chuỗi — dùng để hiển thị và tìm kiếm</summary>
        public string? DepartmentNames { get; set; }

        /// <summary>Danh sách ID chức vụ — dùng khi lưu (multi-select FE gửi lên)</summary>
        public List<int> PositionIDs { get; set; } = new();

        /// <summary>Tên chức vụ stuffed thành 1 chuỗi — dùng để hiển thị và tìm kiếm</summary>
        public string? PositionNames { get; set; }

        /// <summary>Danh sách mô tả checklist stuffed thành 1 chuỗi — dùng để hiển thị</summary>
        public string? CheckListNames { get; set; }

        /// <summary>Danh sách checklist yêu cầu hoàn thành của bước này</summary>
        public List<ProjectGateStepCheckListDTO> CheckLists { get; set; } = new();

        // Intermediate properties for single-query DB mapping
        public string? DepartmentIDsStr { get; set; }
        public string? PositionIDsStr { get; set; }
        public string? ChecklistsJson { get; set; }
    }

    /// <summary>
    /// DTO phụ trợ để mapping dữ liệu phòng ban liên kết từ Stored Procedure
    /// </summary>
    public class ProjectGateStepDepartmentDTO
    {
        public int ProjectGateStepID { get; set; }
        public int DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
    }

    /// <summary>
    /// DTO phụ trợ để mapping dữ liệu chức vụ liên kết từ Stored Procedure
    /// </summary>
    public class ProjectGateStepPositionDTO
    {
        public int ProjectGateStepID { get; set; }
        public int ChucVuID { get; set; }
        public string? ChucVuName { get; set; }
    }

    /// <summary>
    /// DTO cho ProjectGateStepCheckList
    /// </summary>
    public class ProjectGateStepCheckListDTO
    {
        public int ID { get; set; }
        public int? ProjectGateStepID { get; set; }
        public string? Type { get; set; }
        public string? PathFolder { get; set; }
        public string? Description { get; set; }
    }
}
