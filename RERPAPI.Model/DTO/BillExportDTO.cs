using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillExportDTO
    {
        public BillExport? billExport { get; set; }
        public List<BillExportDetail>? billExportDetail { get; set; }
        public List<int>? DeletedDetailIDs { get; set; }
    }
}
