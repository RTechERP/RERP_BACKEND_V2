using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.ESL;

namespace RERPAPI.Repo.GenericEntity.ESL
{
    public class ESLTestTableRegistrationDetailRepo : GenericRepo<ESLTestTableRegistrationDetail>
    {
        public ESLTestTableRegistrationDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

