using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class RequestExportRequestDTO
    {
        public List<ProjectPartListExportDTO> ListItem { get; set; }
        public string WarehouseCode { get; set; }
    }
}
