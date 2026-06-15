using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class RulePayDTO
    {
        public RulePay? Data { get; set; }
        public List<int>? DeleteIds { get; set; }
    }
}