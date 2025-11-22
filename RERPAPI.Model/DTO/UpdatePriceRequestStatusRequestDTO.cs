using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class UpdatePriceRequestStatusRequestDTO
    {
        public List<ProjectPartlistPriceRequest> ListModel { get; set; }
        public List<dynamic>? ListDataMail { get; set; }
    }
}
