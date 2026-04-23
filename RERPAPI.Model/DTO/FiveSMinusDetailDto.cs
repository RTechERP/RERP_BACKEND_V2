using System;

namespace RERPAPI.Model.DTO
{
    public class FiveSMinusDetailDto
    {
        public int ID { get; set; }
        public int? FiveSRatingDetailID { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int FiveSRatingTicketID { get; set; }
        public string TicketName { get; set; }
        public int FiveSErrorID { get; set; }
        public string ErrorName { get; set; }
        public decimal? Point { get; set; }
        public int? Type { get; set; }
        public string Note { get; set; }
        public DateTime? DateMinus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
