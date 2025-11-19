using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ProjectPartlistParam
    {
        public int ProjectID { get; set; }
        public int PartlistTypeID { get; set; }
        public int IsDeleted { get; set; }
        public string? Keywords { get; set; }
        public int IsApprovedTBP { get; set; }
        public int IsApprovedPurchase { get; set; }
        public int ProjectPartListVersionID { get; set; }
    }
}
