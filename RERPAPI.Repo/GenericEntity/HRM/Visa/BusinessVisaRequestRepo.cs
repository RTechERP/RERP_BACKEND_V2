using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.Visa
{
    public class BusinessVisaRequestRepo : GenericRepo<BusinessVisaRequest>
    {
        public BusinessVisaRequestRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
