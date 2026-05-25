using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
        public class DownloadPaymentOrderDTO
        {
            public int PaymentOrderId { get; set; }
            public string PaymentOrderCode { get; set; }
            public List<string> FilePath { get; set; }
        }
}
