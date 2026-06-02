using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HandoverMinutesRepo : GenericRepo<HandoverMinute>
    {
        public HandoverMinutesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}