namespace RERPAPI.Model.Param
{
    public class ApproveOrCancelRequest
    {
        public int ID { get; set; }
        public int Status { get; set; } // 1: Hoàn thành, 2: Hủy
        public string ReasonCancel { get; set; }
    }
}