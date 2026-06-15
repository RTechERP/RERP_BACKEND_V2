using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class OfficeSupplyDTO : OfficeSupply
    {
        public string Unit { get; set; }
        public string TypeName { get; set; }
    }
}