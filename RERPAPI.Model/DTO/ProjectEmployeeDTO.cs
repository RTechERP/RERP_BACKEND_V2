using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectEmployeeDTO
    {
        public List<int> deletedIds { get; set; }
        public int ProjectID { get; set; }
        public List<ProjectEmployee> prjEms { get; set; }
    }

}
