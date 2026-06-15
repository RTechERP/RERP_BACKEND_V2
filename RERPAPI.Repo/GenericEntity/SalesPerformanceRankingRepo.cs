using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class SalesPerformanceRankingRepo : GenericRepo<SalesPerformanceRanking>
    {
        public SalesPerformanceRankingRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}