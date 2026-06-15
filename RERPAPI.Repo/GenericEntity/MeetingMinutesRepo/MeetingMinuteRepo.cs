using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes
{
    public class MeetingMinuteRepo : GenericRepo<MeetingMinute>
    {
        public MeetingMinuteRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}