using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskSettingRepo : GenericRepo<ProjectTaskSetting>
    {
        public ProjectTaskSettingRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}