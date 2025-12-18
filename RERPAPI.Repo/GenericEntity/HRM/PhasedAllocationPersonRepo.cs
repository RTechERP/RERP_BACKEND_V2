using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class PhasedAllocationPersonRepo : GenericRepo<PhasedAllocationPerson>
    {
        public PhasedAllocationPersonRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public string GenPhaseAllocationCode(DateTime? phaseDate)
        {
            var date = phaseDate.Value.Date;
            string phaseCode = $"ComCa{date:ddMMyyyy}";
            return phaseCode;
        }
    }
}
