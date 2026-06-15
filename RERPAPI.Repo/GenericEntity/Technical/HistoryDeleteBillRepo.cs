using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class HistoryDeleteBillRepo : GenericRepo<HistoryDeleteBill>
    {
        public HistoryDeleteBillRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}