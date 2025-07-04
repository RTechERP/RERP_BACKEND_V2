using Microsoft.AspNetCore.Http;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectSurveyDTO
    {
        public ProjectSurvey projectSurvey { get; set; }
        public List<ProjectSurveyDetail> projectSurveyDetails { get; set; }
        public List<int> deletedFiles { get; set; }
    }

}
