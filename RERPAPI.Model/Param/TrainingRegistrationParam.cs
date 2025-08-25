using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class TrainingRegistrationParam
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int DepartmentID { get; set; }
        public int TrainingCategoryID { get; set; }
    }
}
