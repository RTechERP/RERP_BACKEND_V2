using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class RulePayDTO
    {
        public RulePay? Data { get; set; }
        public List<int>? DeleteIds { get; set; }

    }
}
