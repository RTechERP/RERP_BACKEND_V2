using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskApproveRepo : GenericRepo<Model.Entities.ProjectTaskApprove>
    {
        public ProjectTaskApproveRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
