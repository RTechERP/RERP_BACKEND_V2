using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProjectPartlistPriceRequestDTO
    {
        public List<ProjectPartlistPriceRequest> lstModel { get; set; }
        public List<int> lstID { get; set; }
    }
}