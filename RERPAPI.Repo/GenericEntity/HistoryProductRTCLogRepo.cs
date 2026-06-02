using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HistoryProductRTCLogRepo : GenericRepo<HistoryProductRTCLog>
    {
        public HistoryProductRTCLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}