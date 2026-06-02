using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ApprovePurchaseRequestDTO
    {
        public bool IsApproved { get; set; } // true = duyệt, false = hủy
        public DateTime? DeadlineReturnExpected { get; set; } // Chỉ dùng khi IsApproved = true
        public List<ProjectPartList> Items { get; set; }
    }
}