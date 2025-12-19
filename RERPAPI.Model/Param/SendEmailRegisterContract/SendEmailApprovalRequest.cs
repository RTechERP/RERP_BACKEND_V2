using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.SendEmailRegisterContract
{
    /// <summary>
    /// Request cho API gửi email xác nhận/hủy
    /// </summary>
    public class SendEmailApprovalRequest
    {

        public int RegisterContractID { get; set; }
        public int Status { get; set; } // 1: Xác nhận, 2: Hủy
        public string ReasonCancel { get; set; }
    }
}
