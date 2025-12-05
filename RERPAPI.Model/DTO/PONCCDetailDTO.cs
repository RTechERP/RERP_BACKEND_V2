using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class PONCCDetailDTO : PONCCDetail
    {
        public string? PONCCDetailRequestBuyID { get; set; }

        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductNewCode { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int ProductGroupID { get; set; }
    }
}
