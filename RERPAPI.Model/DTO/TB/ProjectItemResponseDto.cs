using System.Text.Json.Serialization;

namespace RERPAPI.Model.DTO.TB
{
    public class ProjectItemResponseDto
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        [JsonPropertyName("project_id")]
        public int? ProjectID { get; set; }

        [JsonPropertyName("user_id")]
        public int? UserID { get; set; }

        public string? Keyword { get; set; }
        public string? Status { get; set; }
    }
}