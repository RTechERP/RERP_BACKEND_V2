using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskWorkRepo : GenericRepo<Model.Entities.ProjectTaskWork>
    {
        public ProjectTaskWorkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
