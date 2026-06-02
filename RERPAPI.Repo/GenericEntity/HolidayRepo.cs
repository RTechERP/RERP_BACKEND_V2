using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HolidayRepo : GenericRepo<Holiday>
    {
        public HolidayRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}