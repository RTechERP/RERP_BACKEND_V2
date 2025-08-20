using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillExportTechnicallExcelFullDTO
    {
        public BillExportTechnicalDTO? Master { set; get; }
        public List<BillExportTechnicalDetailDTO>? Details { set; get; }
    }
}
