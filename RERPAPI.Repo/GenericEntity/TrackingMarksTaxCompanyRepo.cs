using RERPAPI.Model.Common;
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
