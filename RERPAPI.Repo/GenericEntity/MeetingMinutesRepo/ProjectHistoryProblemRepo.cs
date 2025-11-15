using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes
{
    public class ProjectHistoryProblemRepo : GenericRepo<ProjectHistoryProblem>
    {
        public ProjectHistoryProblemRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
