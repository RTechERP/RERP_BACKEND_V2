using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class PersonalPropertyDTO
    {
      
        public int AssetID { get; set; }
   
        public int AssetCategory { get; set; }
        public bool IsApprove { get; set; }
    }
}
