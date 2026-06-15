using System.Text.Json;

namespace RERPAPI.Model.Param.HRM.JobPerfomanceEvaluation
{
    public class ConfirmRequest
    {
        public JsonElement Id { get; set; }

        public string? Role { get; set; }

        public int IsApprove { get; set; }

        public string? Reason { get; set; }
    }
}