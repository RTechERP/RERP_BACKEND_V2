using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectTypeDTO
    {
        public int ID { get; set; }//ProjectId

        public string? ProjectTypeCode { get; set; }//projectCode

        public string? ProjectTypeName { get; set; }//projectTypeName

        public int? ParentID { get; set; }// parentProjectType

        public string? RootFolder { get; set; }

        public int? ApprovedTBPID { get; set; }
        public bool isDelete { get; set; } = false;
    }
}