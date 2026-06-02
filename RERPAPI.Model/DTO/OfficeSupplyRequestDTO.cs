using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class OfficeSupplyRequestDTO
    {
        public OfficeSupplyRequest1? officeSupplyRequest { get; set; }
        public List<OfficeSupplyRequestsDetail>? officeSupplyRequestsDetails { get; set; }
    }
}