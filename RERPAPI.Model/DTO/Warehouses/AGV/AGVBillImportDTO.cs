using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Warehouses.AGV
{
    public class AGVBillImportDTO: AGVBillImport
    {
        public List<AGVBillImportDetail> AGVBillImportDetails { get; set; } = new List<AGVBillImportDetail>();
    }
}
