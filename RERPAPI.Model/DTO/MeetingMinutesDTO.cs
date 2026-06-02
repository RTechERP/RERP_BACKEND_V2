using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class MeetingMinutesDTO
    {
        public MeetingMinute MeetingMinute { get; set; }
        public List<MeetingMinutesDetail>? MeetingMinutesDetail { get; set; }
        public List<MeetingMinutesAttendance>? MeetingMinutesAttendance { get; set; }
        public List<ProjectHistoryProblem>? ProjectHistoryProblem { get; set; }
        public List<int> DeletedMeetingMinutesAttendance { get; set; }
        public List<int> DeletedMeetingMinutesDetails { get; set; }
        public List<MeetingMinutesFile>? MeetingMinutesFile { get; set; }
        public List<int>? DeletedFile { get; set; }

        //public IFormFile[]? Files { get; set; } = Array.Empty<IFormFile>();
    }
}