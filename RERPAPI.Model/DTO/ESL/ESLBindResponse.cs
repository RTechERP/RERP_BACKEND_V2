using System.Text.Json.Serialization;

namespace RERPAPI.Model.DTO.ESL
{
    public class ESLBindResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public class EslDevice
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("esl_code")]
        public string EslCode { get; set; }

        [JsonPropertyName("esl_version")]
        public string EslVersion { get; set; }

        [JsonPropertyName("action")]
        public int Action { get; set; }

        [JsonPropertyName("action_from")]
        public string ActionFrom { get; set; }

        [JsonPropertyName("online")]
        public int Online { get; set; }

        [JsonPropertyName("esl_battery")]
        public int EslBattery { get; set; }


        // Computed helper
        public bool IsOnline => Online == 1;
        public double BatteryVoltage => EslBattery / 1000.0;
    }
}
