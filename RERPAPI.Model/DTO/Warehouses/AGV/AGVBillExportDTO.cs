using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Warehouses.AGV
{
    public class AGVBillExportDTO: AGVBillExport
    {
        public List<AGVBillExportDetail> AGVBillExportDetails { get; set; } = new List<AGVBillExportDetail>();
    }
}
