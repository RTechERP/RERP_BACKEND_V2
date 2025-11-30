using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ApproveBillDTO
    {
        public int BillID { get; set; }
        public bool IsApproved { get; set; } // true: Duyệt, false: Bỏ duyệt
    }
}
