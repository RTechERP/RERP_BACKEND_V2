using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.MeetingMinutesRepo
{
    public class MeetingMinutesFileRepo : GenericRepo<MeetingMinutesFile>
    {
        public MeetingMinutesFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}