namespace RERPAPI.Model.DTO
{
    public class ApproveBillDTO
    {
        public int BillID { get; set; }
        public bool IsApproved { get; set; } // true: Duyệt, false: Bỏ duyệt
    }
}