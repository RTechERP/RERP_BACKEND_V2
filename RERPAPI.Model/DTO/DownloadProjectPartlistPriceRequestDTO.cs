using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class DownloadProjectPartlistPriceRequestDTO
    {
        public int ProjectId { get; set; }
        public int PartListId { get; set; }
        public string ProductCode { get; set; }
    }
}
