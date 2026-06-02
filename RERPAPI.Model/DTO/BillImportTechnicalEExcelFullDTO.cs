namespace RERPAPI.Model.DTO
{
    public class BillImportTechnicalEExcelFullDTO
    {
        public BillImportTechnicalDTO Master { set; get; }
        public List<BillImportDetailTechnicalDTO> Details { set; get; }
    }
}