using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillDetailSerialNumberDTO
    {
        public List<BillExportDetailSerialNumber>? billExportDetailSerialNumbers { get; set; }
        public List<BillImportDetailSerialNumber>? billImportDetailSerialNumbers { get; set; }
        public List<BillExportTechDetailSerialDTO>? billExportTechDetailSerials { get; set; }
        public List<BillImportTechDetailSerialDTO>? billImportTechDetailSerials { get; set; }
        public int type { get; set; }
        public List<int> lsDeleted { get; set; }
    }

    public class BillImportTechDetailSerialDTO : BillImportTechDetailSerial
    {
        public int ModulaLocationDetailID { get; set; }
    }

    public class BillExportTechDetailSerialDTO : BillExportTechDetailSerial
    {
        public int ModulaLocationDetailID { get; set; }
    }
}
