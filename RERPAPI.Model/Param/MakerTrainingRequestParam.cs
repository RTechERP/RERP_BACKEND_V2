using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class MakerTrainingRequestParam
    {
        public string Keyword { get; set; }
        public int DepartmentID { get; set; }
        public int FirmID { get; set; }
        public int MakerTrainingTypeID { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
