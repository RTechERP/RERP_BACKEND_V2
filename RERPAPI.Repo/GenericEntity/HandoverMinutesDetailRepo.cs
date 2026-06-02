using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HandoverMinutesDetailRepo : GenericRepo<HandoverMinutesDetail>
    {
        public HandoverMinutesDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}