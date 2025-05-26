using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectDTO
    {
        public int ID { get; set; }
        public string ProjectCode { get; set; }
        public int? UserID { get; set; }
        public int? ContactID { get; set; }
        public int? CustomerID { get; set; }
        public string ProjectName { get; set; }
        public string PO { get; set; }
    }

}
