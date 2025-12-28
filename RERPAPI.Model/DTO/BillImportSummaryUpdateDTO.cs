namespace RERPAPI.Model.DTO
{
    public class BillImportSummaryUpdateDTO
    {
        public int IDDetail { get; set; }
        public int DeliverID { get; set; }

        public string? SomeBill { get; set; }
        public DateTime? DateSomeBill { get; set; }
        public int DPO { get; set; }

        public decimal? TaxReduction { get; set; }
        public decimal? COFormE { get; set; }
    }
}
