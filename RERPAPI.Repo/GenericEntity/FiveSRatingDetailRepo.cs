using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    /// <summary>
    /// Repository cho bảng chi tiết chấm điểm FiveS
    /// </summary>
    public class FiveSRatingDetailRepo : GenericRepo<FiveSRatingDetail>
    {
        public FiveSRatingDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
