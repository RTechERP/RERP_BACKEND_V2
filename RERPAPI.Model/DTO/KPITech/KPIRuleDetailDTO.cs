using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.KPITech
{
    public class KPIRuleDetailDTO
    {
        // Các cột trả về từ SP: spGetEmployeeRulePointByKPIEmpPointIDNew
        public int ID { get; set; } // Đây là KPIEvaluationRuleDetailID
        public string EvaluationCode { get; set; } // Mã để map dữ liệu
        public decimal FirstMonth { get; set; }
        public decimal SecondMonth { get; set; }
        public decimal ThirdMonth { get; set; }
        public decimal PercentBonus { get; set; }
        public decimal PercentRemaining { get; set; }
        // ... thêm các thuộc tính khác nếu SP trả về nhiều hơn
    }
}
