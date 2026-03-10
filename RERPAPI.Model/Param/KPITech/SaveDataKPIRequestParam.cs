using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.KPITech
{
    public class SaveDataKPIRequestParam
    {
        public int KPISessionID { get; set; }
        public int KPIExamID { get; set; }
        public int employeeID { get; set; }
        public int typePoint { get; set; } // 1: cá nhân, 2: tbp, 3: BGĐ, 4 admin
        public int departmentID { get; set; } // 2: kỹ thuật , 10: cơ khí
        public List<KPIEvaluationPoint> kpiKyNang { get; set; }
        public List<KPIEvaluationPoint> kpiChung { get; set; }
        public List<KPIEvaluationPoint> kpiChuyenMon { get; set; }
        public List<KPISumaryEvaluation> kpiSumaryEvaluation { get; set; }
    }
}
