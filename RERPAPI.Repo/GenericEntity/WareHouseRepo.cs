using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class WarehouseRepo : GenericRepo<Warehouse>
    {
        public WarehouseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public APIResponse Validate(Warehouse item)
        {
            try
            {
                bool exists = GetAll().Any(x => x.WarehouseCode == item.WarehouseCode && x.ID != item.ID && x.IsDeleted != true);
                if (exists)
                {
                    return ApiResponseFactory.Fail(null, "Mã kho đã tồn tại");
                }
                return ApiResponseFactory.Success(null, "");
            }
            catch(Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}
