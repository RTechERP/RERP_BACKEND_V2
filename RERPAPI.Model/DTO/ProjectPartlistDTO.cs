using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectPartlistDTO:ProjectPartList
    {
        public int? Mode { get; set; }//1:ApproveTBP; 2:ApproveNewCode
    }
}
