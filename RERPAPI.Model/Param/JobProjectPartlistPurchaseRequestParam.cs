namespace RERPAPI.Model.Param
{
    public class JobProjectPartlistPurchaseRequestParam
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string? KeyWord { get; set; }
        public int JobRequirementID { get; set; }
    }
}