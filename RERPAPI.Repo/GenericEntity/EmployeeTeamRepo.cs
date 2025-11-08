using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeTeamRepo : GenericRepo<EmployeeTeam>
    {
        public EmployeeTeamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
