using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class SaveProjectWorkerVersionDTO
    {
        public ProjectWorkerVersion? ProjectWorkerVersion { get; set; }
        public List<int> ProjectHistoryProblemIds { get; set; } = new List<int>();
    }
}
