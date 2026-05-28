using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.ESL;

namespace RERPAPI.Repo.GenericEntity.ESL
{
    public class ESLConfigRepo : GenericRepo<ESLConfig>
    {
        public ESLConfigRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
