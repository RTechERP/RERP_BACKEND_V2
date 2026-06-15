namespace RERPAPI.Model.DTO
{
    public class CustomerContactDTO
    {
        public int idConCus { get; set; }
        public int ID { get; set; }

        //public int? CustomerID { get; set; }

        public string? ContactName { get; set; }

        public string? ContactPhone { get; set; }

        public string? ContactEmail { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public string? CustomerTeam { get; set; }

        public string? CustomerPart { get; set; }

        public string? CustomerPosition { get; set; }
    }
}