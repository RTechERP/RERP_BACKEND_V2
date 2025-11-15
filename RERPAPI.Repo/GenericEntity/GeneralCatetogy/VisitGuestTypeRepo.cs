using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class VisitGuestTypeRepo : GenericRepo<VisitGuestType>
    {

        public VisitGuestTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
