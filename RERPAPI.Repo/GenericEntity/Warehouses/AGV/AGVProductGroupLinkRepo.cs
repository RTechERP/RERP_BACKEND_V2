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


        public APIResponse Validate(AGVProductGroupLink groupLink)
        {
            try
            {
                var response = ApiResponseFactory.Success(null, "");
                if (groupLink.AGVProductID <= 0)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng chọn Mã sản phẩm!");
                }

                if (groupLink.AGVProductGroupID <= 0)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng chọn Nhóm sản phẩm!");
                }

                var groupLinks = GetAll(x => x.AGVProductID  == groupLink.AGVProductID && x.AGVProductGroupID == groupLink.AGVProductGroupID &&
                                                x.IsDeleted != true &&
                                                x.ID != groupLink.ID);
                if (groupLinks.Count() > 0)
                {
                    response = ApiResponseFactory.Fail(null, $"Mã sản phẩm [{groupLink.AGVProductID}] đã tồn tại trong nhóm thiết bị [{groupLink.AGVProductGroupID}]. Vui lòng kiểm tra lại!");
                }


                return response;
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}
