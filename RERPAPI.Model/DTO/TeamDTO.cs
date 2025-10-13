using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class TeamDTO
    {
        public int ID { get; set; }
        public int LeaderID { get; set; }
        public int DepartmentID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public int ProjectTyepID { get; set; }
        public string Leader { get; set; }
        public string TypeName { get; set; }
    }
}
