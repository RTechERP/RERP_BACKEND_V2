using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class BillImportTechnicalProtectiveGearDTO
    {
        public BillImportTechnical BillImportTechnical { get; set; }
        public List<BillImportDetailTechnical>? BillImportDetailTechnical { get; set; }
        public List<int>? DeletedDetailIds { get; set; }
    }
}
