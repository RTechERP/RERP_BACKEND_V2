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
    public class AGVProductGroupRepo : GenericRepo<AGVProductGroup>
    {
        public AGVProductGroupRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public APIResponse Validate(AGVProductGroup group)
        {
            try
            {
                var response = ApiResponseFactory.Success(null, "");
                if (string.IsNullOrWhiteSpace(group.AGVProductGroupNo))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Mã loại!");
                }
                else
                {
                    var products = GetAll(x => x.AGVProductGroupNo.Trim().ToLower() == group.AGVProductGroupNo.Trim().ToLower() &&
                                                x.IsDeleted != true &&
                                                x.ID != group.ID);
                    if (products.Count() > 0)
                    {
                        response = ApiResponseFactory.Fail(null, $"Mã loại [{group.AGVProductGroupNo}] đã tồn tại. Vui lòng kiểm tra lại!");
                    }
                }

                if (string.IsNullOrWhiteSpace(group.AGVProductGroupName))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Tên loại!");
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
