using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class BillExportTechnicalProtectiveGearDTO
    {
        public BillExportTechnical BillExportTechnical { get; set; }
        public List<BillExportDetailTechnical>? BillExportDetailTechnical { get; set; }
        public List<int>? DeletedDetailIds { get; set; }
    }
}
