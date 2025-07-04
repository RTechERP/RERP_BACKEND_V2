using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectTypeLinkDTO 
    {
        public int ProjectID { get; set; }
        public int ProjectStatus { get; set; }
        public int GlobalEmployeeId { get; set; }

        public List<prjTypeDTO> prjTypeLinks { get; set; }
        
        public string Situlator { get; set; }
    }

    public class prjTypeDTO
    {
        public int ProjectTypeLinkID { get; set; }
        public int ID { get; set; }
        public int LeaderID { get; set; }
        public Boolean Selected { get; set; }
    }
}
