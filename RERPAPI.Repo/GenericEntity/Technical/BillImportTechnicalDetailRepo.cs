using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class BillImportTechnicalDetailRepo : GenericRepo<BillImportDetailTechnical>
    {
        public BillImportTechnicalDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
