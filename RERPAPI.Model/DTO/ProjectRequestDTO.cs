using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectRequestDTO: ProjectRequest
    {
        public List<ProjectRequestFile>? projectRequestFile { get; set; }
        public List<int>? deletedFileID { get; set; }
    }
}
