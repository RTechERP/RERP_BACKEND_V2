using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes
{
    public class MeetingMinutesAttendanceRepo : GenericRepo<MeetingMinutesAttendance>
    {
        public MeetingMinutesAttendanceRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}