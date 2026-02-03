using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.HRM.Course
{
    public class SaveCoureCategoryParam
    {
        public int ID { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int DepartmentID { get; set; }

        public bool DeleteFlag { get; set; }
        public int STT { get; set; }
        public int CatalogType { get; set; }
        public List<int> ProjectTypeIDs { get; set; }
        public bool IsDeleted { get; set; }


    }
}
