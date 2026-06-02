namespace RERPAPI.Model.DTO
{
    public class SupplierSaleWithSelectionDTO
    {
        public int ID { get; set; }
        public string CodeNCC { get; set; }
        public string NameNCC { get; set; }
        public string NVPhuTrach { get; set; }
        public string LoaiHangHoa { get; set; }
        public int IsSelected { get; set; }
        public string Note { get; set; }
        public string MatHang { get; set; }
        public int TotalCount { get; set; }
    }
}