using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Project
{
    public class ProjectIssueDTO
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("project_id")]
        public int ProjectID { get; set; }  

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("probability")]
        public byte? Probability { get; set; }

        [JsonPropertyName("impact")]
        public byte? Impact { get; set; }

        [JsonPropertyName("status")]
        public byte? Status { get; set; }

        [JsonPropertyName("solution")]
        public string? Solution { get; set; }

        [JsonPropertyName("mitigation_plan")]
        public string? MitigationPlan { get; set; }

        [JsonPropertyName("assigned_to")]
        public string? EmployeeCode { get; set; }

        [JsonPropertyName("file_paths")]
        public string? FilePath { get; set; }

        [JsonPropertyName("create_at")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("update_at")]
        public DateTime? UpdatedDate { get; set; }

    }

}
