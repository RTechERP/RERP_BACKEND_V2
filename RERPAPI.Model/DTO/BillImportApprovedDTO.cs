using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillImportApprovedDTO:BillImport
    {
        public string? CurrencyList { get; set; }
        public string? PONCCCodeList { get; set; }
        public decimal? VAT { get; set; }
        public int? PONCCDetailID { get; set; }
        public List<BillImportDetail> billImportDetails { get; set; }

    }
}
