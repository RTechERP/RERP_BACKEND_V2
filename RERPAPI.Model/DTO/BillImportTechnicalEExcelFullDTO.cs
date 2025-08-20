using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillImportTechnicalEExcelFullDTO
    {
        public BillImportTechnicalDTO Master { set; get; }
      public  List<BillImportDetailTechnicalDTO> Details { set; get; }
    }
}
