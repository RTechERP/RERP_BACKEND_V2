using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.ESL
{
    public class ESLTestTableRegistrationRepo : GenericRepo<RERPAPI.Model.Entities.ESLTestTableRegistration>
    {
        public ESLTestTableRegistrationRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
