using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverSubordinateRepo : GenericRepo<HandoverSubordinate>
    {
        public HandoverSubordinateRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
