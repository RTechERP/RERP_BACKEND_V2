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
    public class TrackingMarksSealRepo : GenericRepo<TrackingMarksSeal>
    {
       
        public TrackingMarksSealRepo(CurrentUser currentUser) : base(currentUser)
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


        public void CreateListByTrackingMarkId(List<TrackingMarksSeal> lst, int TrackingMarkId)
        {
            //SQLHelper<TrackingMarksSeal>.SqlToModel($"DELETE FROM TrackingMarksSeal WHERE TrackingMartkID = {TrackingMarkId}");
            var oldItems = GetAll()
                .Where(x => x.TrackingMartkID == TrackingMarkId)
                .ToList();

            if (oldItems.Any())
            {
                DeleteRange(oldItems);
            }

            foreach (var item in lst)
            {
                TrackingMarksSeal newModel = new TrackingMarksSeal();
                newModel.TrackingMartkID = TrackingMarkId;
                newModel.SealID = item.SealID;
                Create(newModel);
            }
        }
    }
}
