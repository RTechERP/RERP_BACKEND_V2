using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO.Project;
namespace RERPAPI.Model.DTO.ProjectAGV
{
    public class    ProjectAGVResponseDTO
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("code")]
        public string? ProjectCode { get; set; }
        [JsonPropertyName("name")]
        public string? ProjectName { get; set; }
        [JsonPropertyName("description")]
        public string? description { get; set; } = "RERP Không có";
        [JsonPropertyName("status")]
        public string? ProjectStatusName { get; set; }

        [JsonPropertyName("priority")]

        public Decimal? Priotity { get; set; }
        [JsonPropertyName("progress")]
        public string? progress { get; set; } = "RERP Không có";
        [JsonPropertyName("start_date")]
        public DateTime? ActualDateStart { get; set; }
        [JsonPropertyName("end_date")]
        public DateTime? ActualDateEnd { get; set; }

        [JsonPropertyName("tags")]
        public string? tags { get; set; }  = "RERP Không có";
        [JsonPropertyName("project_type")]
        public int? ProjectType { get; set; }
        [JsonPropertyName("business_field")]
        public int? BusinessFieldID { get; set; }
        [JsonPropertyName("category")]
        public int? TypeProject { get; set; }
        [JsonPropertyName("customer_id")]
        public int? CustomerID { get; set; }
        [JsonPropertyName("pm_id")]
        public int? ProjectManager { get; set; }
        [JsonPropertyName("pic_id")]
        public int? ContactID { get; set; }
        [JsonPropertyName("sale_id")]
        public int? SaleID { get; set; }
        [JsonPropertyName("pic_ids")]
        public string? pic_ids { get; set; } = "RERP Không có";
        [JsonPropertyName("note")]
        public string? Note { get; set; }
        [JsonPropertyName("created_by")]
        public string? CreatedBy { get; set; }
        [JsonPropertyName("update_at")]
        public DateTime? UpdatedDate { get; set; }
        [JsonPropertyName("create_at")]
        public DateTime? CreatedDate { get; set; }
        [JsonPropertyName("tasks")]
        public List<ProjectItemDTO>? tasks { get; set; }
        [JsonPropertyName("documents")]
        public List<ProjectDocumentDTO>? documents { get; set; }
        [JsonPropertyName("issues")]
        public List<ProjectIssueDTO>? issues { get; set; }
        [JsonPropertyName("is_deleted")]
        public string? is_deleted { get; set; } = "RERP Không có";
        [JsonPropertyName("project_status")]

        public int? ProjectStatus { get; set; }
        //   public List<ProjectItemDTO>? projectItemDTO { get; set; }
    }
}
