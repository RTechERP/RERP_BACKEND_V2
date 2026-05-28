using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.ESL;

namespace RERPAPI.Repo.GenericEntity.ESL
{
    public class ESLTestTableRepo : GenericRepo<ESLTestTable>
    {
        public ESLTestTableRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
