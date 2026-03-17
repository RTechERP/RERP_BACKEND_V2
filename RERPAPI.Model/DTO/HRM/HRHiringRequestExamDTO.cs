using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class HRHiringRequestExamDTO
    {
        public bool IsActiveExam { get; set; }
        public int HiringRequestID { get; set; }
        public List<int> listHiringRequestIDExam { get; set; }

        public List<int> deletedHiringRequestIDExam { get; set; }
    }
}
