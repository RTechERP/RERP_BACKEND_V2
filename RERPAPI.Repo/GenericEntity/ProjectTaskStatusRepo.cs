using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskStatusRepo : GenericRepo<Model.Entities.ProjectTaskStatus>
    {
        public ProjectTaskStatusRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
