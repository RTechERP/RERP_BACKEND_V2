using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class PaymentOrderDTO:PaymentOrder
    {
        public int ApprovedTBPID { get; set; }
        public int ApprovedBGDID { get; set; }
        public List<PaymentOrderDetail> PaymentOrderDetails { get; set; } = new List<PaymentOrderDetail>();
        public string PaymentOrderFiles { get; set; } = string.Empty;
        public List<PaymentOrderLog> PaymentOrderLogs { get; set; } = new List<PaymentOrderLog>();
    }
}
