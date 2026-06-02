using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HistoryMoneyPORepo : GenericRepo<HistoryMoneyPO>
    {
        public HistoryMoneyPORepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}