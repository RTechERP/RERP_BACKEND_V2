using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class JobRequirementApproveDTO
    {
        public int JobRequirementID { get; set; }
        public int Step { get; set; }      // bước duyệt
        public int Status { get; set; }    // 1 = duyệt, 2 = huỷ
        public string ReasonCancel { get; set; }
    }
}
