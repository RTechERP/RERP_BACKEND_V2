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
        public PONCC poncc { get; set; }
        public List<PONCCDetailDTO> lstPONCCDetail { get; set; }
        public List<ProjectPartlistPurchaseRequestDTO> lstPrjPartlistPurchaseRequest { get; set; }
        public List<int> lstBillImportId { get; set; }
    }

}
