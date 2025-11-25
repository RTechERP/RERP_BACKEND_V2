using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillExportDataDTO
    {
        public BillExportRQPDTO  Bill { get; set; }
        public List<BillExportDetailRQPDTO> Details { get; set; }
    }
}
