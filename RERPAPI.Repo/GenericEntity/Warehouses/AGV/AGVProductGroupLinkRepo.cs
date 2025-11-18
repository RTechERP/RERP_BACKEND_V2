using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Warehouses.AGV
{
    public class AGVProductGroupLinkRepo : GenericRepo<AGVProductGroupLink>
    {
        public AGVProductGroupLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        public APIResponse Validate(List<AGVProductGroupLink> groupLinks)
        {
            try
            {
                foreach (var groupLink in groupLinks)
                {
                    if (groupLink.AGVProductID <= 0)
                    {
                        return ApiResponseFactory.Fail(null, "Vui lòng chọn Mã sản phẩm!");
                    }

                    if (groupLink.AGVProductGroupID <= 0)
                    {
                        return ApiResponseFactory.Fail(null, "Vui lòng chọn Loại sản phẩm!");
                    }

                    var dataGroupLinks = GetAll(x => x.AGVProductID == groupLink.AGVProductID &&
                                                    x.AGVProductGroupID == groupLink.AGVProductGroupID &&
                                                    x.WarehouseID == groupLink.WarehouseID &&
                                                    x.IsDeleted != true &&
                                                    x.ID != groupLink.ID);
                    if (dataGroupLinks.Count() > 0)
                    {
                        return ApiResponseFactory.Fail(null, $"Sản phẩm [{groupLink.AGVProductID}] đã tồn tại trong Loại [{groupLink.AGVProductGroupID}] kho [{groupLink.WarehouseID}]. Vui lòng kiểm tra lại!");
                    }
                }
                


                return ApiResponseFactory.Success(null, "");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}
