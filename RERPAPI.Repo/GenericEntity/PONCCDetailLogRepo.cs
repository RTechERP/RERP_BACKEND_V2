using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PONCCDetailLogRepo : GenericRepo<PONCCDetailLog>
    {
        public PONCCDetailLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}