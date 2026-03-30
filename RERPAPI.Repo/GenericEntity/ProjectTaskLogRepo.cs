using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskLogRepo : GenericRepo<Model.Entities.ProjectTaskLog>
    {
        public ProjectTaskLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
