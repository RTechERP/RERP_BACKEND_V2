using Microsoft.AspNetCore.Http;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class HRRecruitmentCandidateDTO : HRRecruitmentCandidate
    {
        public bool? isApproved { get; set; }
        public List<int>? listIds { get; set; }
        public IFormFile? FileCV { get; set; }
        public string? NoteLog { get; set; }
    }
}
