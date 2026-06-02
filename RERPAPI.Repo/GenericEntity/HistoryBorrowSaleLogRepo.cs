using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HistoryBorrowSaleLogRepo : GenericRepo<HistoryBorrowSaleLog>
    {
        public HistoryBorrowSaleLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}