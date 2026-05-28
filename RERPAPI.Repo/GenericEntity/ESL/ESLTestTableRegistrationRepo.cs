using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.ESL;

namespace RERPAPI.Repo.GenericEntity.ESL
{
    public class ESLTestTableRegistrationRepo : GenericRepo<ESLTestTableRegistration>
    {
        public ESLTestTableRegistrationRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
