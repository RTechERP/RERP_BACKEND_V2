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
        public int ApprovedTBPID { get; set; } = 0;
        public int ApprovedBGDID { get; set; } = 0;
        public int? Step { get; set; } = 0;
        public int? CurrentApproved { get; set; } = 0;
        public PaymentOrderAction Action { get; set; } = new PaymentOrderAction();
        public string ReasonCancel { get; set; } = string.Empty;
        public List<PaymentOrderDetailDTO> PaymentOrderDetails { get; set; } = new List<PaymentOrderDetailDTO>();
        public string PaymentOrderFiles { get; set; } = string.Empty;
        public List<PaymentOrderLog> PaymentOrderLogs { get; set; } = new List<PaymentOrderLog>();
        public PaymentOrderLog PaymentOrderLog { get; set; } = new PaymentOrderLog();
    }

    public class PaymentOrderAction
    {
        public string ButtonActionGroup { get; set; } = string.Empty;
        public string ButtonActionName { get; set; } = string.Empty;
        public string ButtonActionText { get; set; } = string.Empty;
    }
}
