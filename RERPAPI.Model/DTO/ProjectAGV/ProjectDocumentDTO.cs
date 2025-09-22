using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectDocumentDTO
    {

        [JsonPropertyName("id")]
        public int? ID { get; set; }

        [JsonPropertyName("project_id")]
        public int? ProjectID { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public byte? Type { get; set; }

        [JsonPropertyName("url")]
        public string? FilePath { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("size")]
        public decimal? Size { get; set; }

        // Thông tin người thao tác
        [JsonPropertyName("uploaded_by")]
        public string? CreateBy { get; set; }

        [JsonPropertyName("uploaded_at")]
        public DateTime? CreateDate { get; set; }

        [JsonPropertyName("updated_by")]
        public string? UpdatedBy { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedDate { get; set; }

        // Xoá mềm: isDeleted = 1 và ID > 0
        [JsonPropertyName("isDeleted")]
        public bool? IsDeleted { get; set; }
    }
}
