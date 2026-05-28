using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.ESL;

namespace RERPAPI.Repo.GenericEntity.ESL
{
    public class ESLTestTableRegistrationLogRepo : GenericRepo<ESLTestTableRegistrationLog>
    {
        public ESLTestTableRegistrationLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
