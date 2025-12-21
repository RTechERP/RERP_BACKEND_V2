using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeFoodOrderRepo : GenericRepo<EmployeeFoodOrder>
    {
        private PhasedAllocationPersonRepo phaseRepo;
        private PhasedAllocationPersonDetailRepo phaseDetailRepo;
        public EmployeeFoodOrderRepo(CurrentUser currentUser, PhasedAllocationPersonRepo phaseRepo, PhasedAllocationPersonDetailRepo phaseDetailRepo) : base(currentUser)
        {
            this.phaseRepo = phaseRepo;
            this.phaseDetailRepo = phaseDetailRepo;
        }
       
      

    }
}
