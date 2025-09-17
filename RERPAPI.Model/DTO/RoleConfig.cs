using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class RoleConfig
    {
        public List<int> TBPEmployeeIds { get; set; } = new();
        public List<string> PBPPositionCodes { get; set; } = new();
    }

}
