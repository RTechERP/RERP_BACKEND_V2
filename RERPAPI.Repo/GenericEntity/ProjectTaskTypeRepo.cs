using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskTypeRepo : GenericRepo<Model.Entities.ProjectTaskType>
    {
        public ProjectTaskTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
