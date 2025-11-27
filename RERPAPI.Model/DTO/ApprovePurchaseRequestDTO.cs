using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ApprovePurchaseRequestDTO
    {
        public bool IsApproved { get; set; } // true = duyệt, false = hủy
        public DateTime? DeadlineReturnExpected { get; set; } // Chỉ dùng khi IsApproved = true
        public List<ProjectPartList> Items { get; set; }
    }
}
