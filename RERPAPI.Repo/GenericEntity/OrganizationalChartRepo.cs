using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class OrganizationalChartRepo : GenericRepo<OrganizationalChart>
    {
        public OrganizationalChartRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
