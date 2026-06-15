using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPIEvaluationLogRepo : GenericRepo<KPIEvaluationLog>
    {
        public KPIEvaluationLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
