using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectHistoryProblemDTO
    {
        public ProjectHistoryProblem? projectHistoryProblem {  get; set; }
        public List<ProjectHistoryProblemDetail>? detail { get; set; }
        public List<int>? deleteIdsMaster { get; set; }
        public List<int>? deletedIdsDetail { get; set; }
    }
}
