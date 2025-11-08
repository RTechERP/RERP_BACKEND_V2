using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeBaseRepo : GenericRepo<ProjectTypeBase>
    {

        public ProjectTypeBaseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
