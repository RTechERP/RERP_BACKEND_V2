using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class POKHDTO
    {
        public POKH POKH { get; set; }
        public List<POKHDetail> POKHDetails { get; set; }
        public List<POKHDetailMoney> POKHDetailsMoney { get; set; }
    }
}
