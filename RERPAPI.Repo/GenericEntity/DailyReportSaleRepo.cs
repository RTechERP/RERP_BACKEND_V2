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
    public class DailyReportSaleRepo : GenericRepo<DailyReportSale>
    {
       
        public DailyReportSaleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
