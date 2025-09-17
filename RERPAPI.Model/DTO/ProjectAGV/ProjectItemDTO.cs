using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace RERPAPI.Model.DTO.ProjectAGV
{
    public class ProjectItemDTO
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("project_id")]
        public int ProjectID { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Mission { get; set; }
        [JsonPropertyName("status")]
        public int? Status { get; set; }
        [JsonPropertyName("type")]
        public int? TypeProjectItem { get; set; }
        [JsonPropertyName("priority")]
        public decimal? Priority { get; set; } = 0;// k có trong DB
        [JsonPropertyName("progress")]
        public decimal? PercentItem { get; set; }
        [JsonPropertyName("steps")]
        public string? Steps { get; set; }// K CO TRONG DB
        [JsonPropertyName("start_date")]

        public DateTime? PlanStartDate { get; set; }
        [JsonPropertyName("due_date")]

        public DateTime? PlanEndDate { get; set; }
        [JsonPropertyName("completed_date")]

        public DateTime? ActualEndDate { get; set; }
        [JsonPropertyName("pic_employee_code")]
        // Chưa 
        public string? PICEmployeeCode { get; set; } = "Không có trong db";// k có trong DB
        [JsonPropertyName("supports")]
        public string? Supports { get; set; } = "Không có trong R_ERP";// k có trong DB
        [JsonPropertyName("file_paths")]
        public string? FilePaths { get; set; } = "";// k có trong DB
        [JsonPropertyName("created_at")]
        public DateTime? CreatedDate { get; set; } = null;
        [JsonPropertyName("update_at")]
        public DateTime? UpdatedDate { get; set; } = null;
        [JsonPropertyName("is_deleted")]

        //   public bool IsDeleted { get; set; }
        public bool IsDeleted { get; set; } = false;
        [JsonPropertyName("user_id")]

        //   public bool IsDeleted { get; set; }
        public int? UserID { get; set; }
    }
}
