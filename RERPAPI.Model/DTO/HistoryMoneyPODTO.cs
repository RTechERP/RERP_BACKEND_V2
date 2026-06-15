using RERPAPI.Model.Entities;

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