using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace RERPAPI.Model.Param.Project
{
    public class ProjectItemRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
     
        public int? ProjectID { get; set; }
        public int? UserID { get; set; }
        public string? Keyword { get; set; }
        public string? Status { get; set; }

    }
}
