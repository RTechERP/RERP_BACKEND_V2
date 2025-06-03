using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ModulaLocationDTO: ModulaLocation
    {
        public List<dynamic> ModulaLocationDetails { get; set; }
        public List<ModulaLocationDetail> LocationDetails { get; set; }

        public class SerialNumberModulaLocation
        {
            public int ModulaLocationDetailID { get; set; }
            public int BillImportDetailID { get; set; }
            public int BillExportDetailID { get; set; }
            public string Name { get; set; }
            public string SerialNumber { get; set; }
            public int Quantity { get; set; }
            public string CreatedBy { get; set; }
        }
    }
}
