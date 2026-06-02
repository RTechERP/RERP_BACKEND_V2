using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TrackingMarksFileRepo : GenericRepo<TrackingMarksFile>
    {
        public TrackingMarksFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public void DeleteByTrackingMarkId(int trackingMarkId)
        {
            var ids = GetAll()
                .Where(x => x.TrackingMarksID == trackingMarkId)
                .Select(x => x.ID)
                .ToList();

            foreach (var id in ids)
            {
                Delete(id);
            }
        }
    }
}