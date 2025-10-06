using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillExportDetailDTO
    {
        public BillExportDetail billExportDetail { get; set; } = new BillExportDetail();
        public List<BillExportDetailSerialNumber> SerialNumbers { get; set; } = new List<BillExportDetailSerialNumber>();
    }
}
