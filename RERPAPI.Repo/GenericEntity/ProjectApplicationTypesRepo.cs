using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Project
{
    public class ProjectApplicationTypesRepo: GenericRepo<ProjectApplicationType>
    {
        public ProjectApplicationTypesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

    }
}
