using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductGroupWareHouseRepo : GenericRepo<ProductGroupWarehouse>
    {
        RTCContext db = new RTCContext();

        public ProductGroupWareHouseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<ProductGroupWarehouse> FindByGroupAndWarehouseAsync(int groupId, int warehouseId)
        {
            try
            {
                return await Task.FromResult(
                    db.ProductGroupWarehouses
                    .FirstOrDefault(x => x.ProductGroupID == groupId && x.WarehouseID == warehouseId)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching group-warehouse mapping: " + ex.Message, ex);
            }
        }
    }
}
