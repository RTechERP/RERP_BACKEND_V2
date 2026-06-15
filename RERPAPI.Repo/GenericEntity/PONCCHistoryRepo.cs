using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PONCCHistoryRepo : GenericRepo<PONCCHistory>
    {
        public PONCCHistoryRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}