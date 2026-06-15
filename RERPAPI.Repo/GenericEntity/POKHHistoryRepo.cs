using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class POKHHistoryRepo : GenericRepo<POKHHistory>
    {
        public POKHHistoryRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}