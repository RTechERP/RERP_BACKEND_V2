using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillImportDTO
    {
        public BillImport? billImport { get; set; }
        public List<BillImportDetail>? billImportDetail { get; set; } 
        public List<int>? DeletedDetailIDs { get; set; } 
    }
}
