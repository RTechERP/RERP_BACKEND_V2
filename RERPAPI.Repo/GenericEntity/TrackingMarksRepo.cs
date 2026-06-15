using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TrackingMarksRepo : GenericRepo<TrackingMark>
    {
        public TrackingMarksRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}