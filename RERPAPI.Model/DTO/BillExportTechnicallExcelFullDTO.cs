namespace RERPAPI.Model.DTO
{
    public class BillExportTechnicallExcelFullDTO
    {
        public BillExportTechnicalDTO? Master { set; get; }
        public List<BillExportTechnicalDetailDTO>? Details { set; get; }
    }
}