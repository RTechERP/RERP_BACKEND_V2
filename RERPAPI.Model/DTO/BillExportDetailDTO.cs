using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class BillExportDetailDTO
    {
        public BillExportDetail billExportDetail { get; set; } = new BillExportDetail();
        public List<BillExportDetailSerialNumber> SerialNumbers { get; set; } = new List<BillExportDetailSerialNumber>();
    }


    public class BillExportDetailExtendedDTO : BillExportDetail
    {
        public string? ChosenInventoryProject { get; set; }
    }
}
