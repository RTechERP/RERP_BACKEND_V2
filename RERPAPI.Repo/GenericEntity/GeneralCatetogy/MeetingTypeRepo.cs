using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class MeetingTypeRepo : GenericRepo<MeetingType>
    {
        public MeetingTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
