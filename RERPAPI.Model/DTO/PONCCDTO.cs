using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class PONCCDTO
    {
        public int? RulePayID { get; set; }
        public bool? IsCheckTotalMoneyPO { get; set; }
        public PONCC poncc { get; set; } = new PONCC();
        public List<PONCCDetailDTO> lstPONCCDetail { get; set; } = new List<PONCCDetailDTO>();
        public List<ProjectPartlistPurchaseRequestDTO> lstPrjPartlistPurchaseRequest { get; set; } = new List<ProjectPartlistPurchaseRequestDTO>();
        public List<int> lstBillImportId { get; set; } = new List<int>();
    }

}
