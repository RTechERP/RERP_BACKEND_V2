using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class RegisterIdeaDTO : RegisterIdea
    {
        public Int64? STT { get; set; }
        public string? BGDScoreNew { get; set; }
        public string? EmployeeName { get; set; }
        public string? TBPName { get; set; }
        public string? BGDName { get; set; }
        public decimal? DepartmentScore { get; set; }
        public decimal? TBPScore { get; set; }
        public decimal? BGDScore { get; set; }
        public decimal? AvgScore { get; set; }
        public string? RegisterTypeName { get; set; }
        public string? DepartmentOrganization { get; set; }
        public string? Description { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public List<RegisterIdeaDetail>? RegisterIdeaDetails { get; set; }
        public List<RegisterIdeaFile>? ListRegisterFile { get; set; }
        public List<RegisterIdeaScore>? ListRegisterScore { get; set; }
        public List<int>? deletedFileIds { get; set; }
        public string? DepartmentName { get; set; }
        public long? RowNum { get; set; }
        public int? HeadofDepartment { get; set; }


    }
}
