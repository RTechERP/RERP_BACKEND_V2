using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    /// <summary>
    /// DTO lưu một dòng CheckListDetail (FileRule, FileFormat, FileQuantity, IsCheck)
    /// dùng khi POST save-details hoặc save-by-step
    /// </summary>
    public class CheckListDetailSaveDto
    {
        public int ID { get; set; }
        public string? FileRule { get; set; }
        public string? FileFormat { get; set; }
        public int FileQuantity { get; set; } = 0;
        public bool IsCheck { get; set; }
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// DTO chứa danh sách lưu mới/cập nhật và danh sách ID cần xóa mềm
    /// </summary>
    public class SaveCheckListDetailsDto
    {
        public List<CheckListDetailSaveDto> Details { get; set; } = new();
        public List<int> DeletedIds { get; set; } = new();
    }

    /// <summary>
    /// DTO lưu một CheckList kèm danh sách Detail bên trong
    /// dùng khi POST save-by-step/{stepId}
    /// </summary>
    public class CheckListWithDetailsDto
    {
        public string? Type { get; set; }
        public int? ProjectGateCheckListType { get; set; }
        public string? Description { get; set; }
        public string? PathFolder { get; set; }
        public bool IsRequired { get; set; }
        public List<CheckListDetailSaveDto> Details { get; set; } = new();
    }
}
