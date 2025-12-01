using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProjectSurveyResultDTO
    {
        public string result { get; set; }
        public int projectId { get; set; }
        public int projectSurveyDetailId { get; set; }
        public int projectTypeId { get; set; }
        public List<ProjectSurveyFile>? file { get; set; }
        public List<int>? deletedFileID { get; set; }
    }
}
