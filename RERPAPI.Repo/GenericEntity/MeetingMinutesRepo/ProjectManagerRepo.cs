using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes
{
    public class ProjectManagerRepo : GenericRepo<RERPAPI.Model.Entities.Project>
    {
        public ProjectManagerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}