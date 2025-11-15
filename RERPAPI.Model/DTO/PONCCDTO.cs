using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class PONCCDTO : PONCC
    {
        // Chi tiết PO
        public List<PONCCDetail> lstPONCCDetail { get; set; }

        // Điều khoản thanh toán
        public List<PONCCRulePay> lstPONCCRulePay { get; set; }

        // Chứng từ nhập khẩu
        public List<DocumentImportPONCC> lstDocumentImportPONCC { get; set; }

        // Liên kết chi tiết PO <-> Request Buy
        public List<PONCCDetailRequestBuy> lstPONCCDetailRequestBuy { get; set; }

        // Bill Import
        public List<BillImportDetail> lstBillImportDetail { get; set; }

        // Log thay đổi chi tiết PO
        public List<PONCCDetailLog> lstPONCCDetailLog { get; set; }

        public bool? OrderQualityNotMet { get; set; }

        public string? ReasonForFailure { get; set; }

    }

}
