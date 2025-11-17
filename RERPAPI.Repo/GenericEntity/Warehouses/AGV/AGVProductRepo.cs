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
    public class AGVProductRepo : GenericRepo<AGVProduct>
    {
        public AGVProductRepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        public APIResponse Validate(AGVProduct product)
        {
            try
            {
                var response = ApiResponseFactory.Success(null, "");
                if (string.IsNullOrWhiteSpace(product.ProductCode))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Mã sản phẩm!");
                }
                else
                {
                    var products = GetAll(x => x.ProductCode.Trim().ToLower() == product.ProductCode.Trim().ToLower() &&
                                                x.IsDeleted != true &&
                                                x.ID != product.ID);
                    if (products.Count() > 0)
                    {
                        response = ApiResponseFactory.Fail(null, $"Mã sản phẩm [{product.ProductCode}] đã tồn tại. Vui lòng kiểm tra lại!");
                    }
                }

                if (string.IsNullOrWhiteSpace(product.ProductName))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Tên sản phẩm!");
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
