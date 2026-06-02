namespace RERPAPI.Model.DTO
{
    public class BillExportDataDTO
    {
        public BillExportRQPDTO Bill { get; set; }
        public List<BillExportDetailRQPDTO> Details { get; set; }
    }
}