namespace RERPAPI.Model.Param.Asset
{
    public class AssetTransferRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? IsApproved { get; set; }
        public int? DeliverID { get; set; }
        public int? ReceiverID { get; set; }
        public string? TextFilter { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNumber { get; set; } = 10000;
    }
}