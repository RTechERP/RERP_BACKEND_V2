using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
   public  class PhasedAllocationPersonDetailDTO: PhasedAllocationPersonDetail
    {
        public string EmployeeCode { get; set; } = string.Empty;
    }
}
