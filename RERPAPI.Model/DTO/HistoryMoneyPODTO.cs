using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class HistoryMoneyPODTO
    {
        public List<HistoryMoneyPO> historyMoneyPOs { get; set; }
        public int pokhDetailId { get; set; }

        public int pokhId { get; set; }
        public decimal totalMoneyIncludeVAT { get; set; }
        public List<int> listIdsDel { get; set; }
    }
}
