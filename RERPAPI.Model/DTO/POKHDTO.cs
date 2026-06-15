using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class POKHDTO
    {
        public POKH POKH { get; set; }
        public List<POKHDetail> POKHDetails { get; set; }
        public List<POKHDetailMoney> POKHDetailsMoney { get; set; }
    }
}