using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class UpdatePriceRequestStatusRequestDTO
    {
        public List<ProjectPartlistPriceRequest> ListModel { get; set; }
        public List<ProductMailInfo>? ListDataMail { get; set; }
    }
    public class ProductMailInfo
    {
        public int EmployeeID { get; set; }
        public string ProjectCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Manufacturer { get; set; }
        public string UnitCount { get; set; }
        public int Quantity { get; set; }
        public DateTime DateRequest { get; set; }
        public DateTime Deadline { get; set; }
    }
}
