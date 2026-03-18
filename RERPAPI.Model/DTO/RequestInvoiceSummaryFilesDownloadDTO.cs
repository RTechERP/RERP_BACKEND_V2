using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class RequestInvoiceSummaryFilesDownloadDTO
    {
        public int RequestInvoiceID { get; set; }
        public int? POKHId { get; set; }
        public string CompanyText { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
