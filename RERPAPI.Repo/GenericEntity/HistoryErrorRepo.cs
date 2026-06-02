using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HistoryErrorRepo : GenericRepo<HistoryError>
    {
        public HistoryErrorRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}