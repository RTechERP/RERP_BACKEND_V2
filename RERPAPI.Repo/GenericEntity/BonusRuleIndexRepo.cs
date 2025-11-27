using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class BonusRuleIndexRepo : GenericRepo<BonusRuleIndex>
    {
       
        public BonusRuleIndexRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
