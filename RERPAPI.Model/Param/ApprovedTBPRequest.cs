using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ApprovedTBPRequest
    {
        public List<int> ProjectPartListID { get; set; }
        public bool Approved { get; set; }
    }
}
