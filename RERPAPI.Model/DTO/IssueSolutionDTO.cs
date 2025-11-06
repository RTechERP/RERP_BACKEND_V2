using Microsoft.Identity.Client;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class IssueSolutionDTO
    {
        public IssueLogSolution issueSolutionLogs { get; set; } = new IssueLogSolution();
        //public List<int>? DeletedIds { get; set; } = new List<int>();
        public IssueSolutionCauseLink? issueSolutionCauseLink { get; set; } = new IssueSolutionCauseLink();
        public IssueSolutionStatusLink? issueSolutionStatusLink { get; set; } = new IssueSolutionStatusLink();
        //public string? OtherIssueCauseNote { get; set; }
        //public string? ReasonIgnoreStatusText { get; set; }
        public List<IssueSolutionDocument>? issueSolutionDocuments { get; set; } = new List<IssueSolutionDocument>();
        //public IssueSolutionDocument? issueSolutionDocuments { get; set; } = new List<IssueSolutionDocument>();


    }
}
