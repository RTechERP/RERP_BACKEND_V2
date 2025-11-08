using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class RulePayRepo : GenericRepo<RulePay>
    {
        public RulePayRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
