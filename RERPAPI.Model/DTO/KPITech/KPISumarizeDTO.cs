using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.KPITech
{
    public class KPISumarizeDTO
    {
        public string EvaluationCode { get; set; }
        public decimal FirstMonth { get; set; }
        public decimal SecondMonth { get; set; }
        public decimal ThirdMonth { get; set; }
    }
}
