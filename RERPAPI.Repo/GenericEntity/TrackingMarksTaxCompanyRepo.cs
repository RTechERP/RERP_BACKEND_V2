using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TrackingMarksTaxCompanyRepo : GenericRepo<TrackingMarksTaxCompany>
    {
        public TrackingMarksTaxCompanyRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public void DeleteByTrackingMarkId(int trackingMarkId)
        {
            var ids = GetAll()
                .Where(x => x.TrackingMartkID == trackingMarkId)
                .Select(x => x.ID)
                .ToList();

            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        public void CreateListByTrackingMarkId(List<TrackingMarksTaxCompany> lst, int TrackingMarkId)
        {
            //SQLHelper<TrackingMarksTaxCompany>.SqlToModel($"DELETE FROM TrackingMarksTaxCompany WHERE TrackingMartkID = {TrackingMarkId}");
            var oldItems = GetAll()
                .Where(x => x.TrackingMartkID == TrackingMarkId)
                .ToList();
            if (oldItems.Any())
            {
                DeleteRange(oldItems);
            }
            foreach (var item in lst)
            {
                TrackingMarksTaxCompany newModel = new TrackingMarksTaxCompany();
                newModel.TrackingMartkID = TrackingMarkId;
                newModel.TaxCompanyID = item.TaxCompanyID;
                Create(newModel);
            }
        }
    }
}