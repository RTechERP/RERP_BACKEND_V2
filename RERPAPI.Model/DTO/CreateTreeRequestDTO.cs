using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class CreateTreeRequestDTO
    {
        public int ProjectId { get; set; }
        public List<int> SelectedProjectTypeIds { get; set; }
    }
}
