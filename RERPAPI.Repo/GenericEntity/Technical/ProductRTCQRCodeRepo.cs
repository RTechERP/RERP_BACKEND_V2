using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Warehouses.AGV;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class ProductRTCQRCodeRepo : GenericRepo<ProductRTCQRCode>
    {
        public ProductRTCQRCodeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public APIResponse Validate(ProductRTCQRCode productRTCQRCode)
            {
            try
            {

                if (productRTCQRCode.ProductRTCID <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng chọn tên thiết bị");
                }

                if (string.IsNullOrWhiteSpace(productRTCQRCode.ProductQRCode))
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập QR Code!");
                }
                else
                {
                    var codeExist = GetAll(x => x.ProductQRCode!.ToLower().Trim() == productRTCQRCode.ProductQRCode.ToLower().Trim() && x.WarehouseID == productRTCQRCode.WarehouseID&&x.ID!=productRTCQRCode.ID);
                    if(codeExist.Count>0)
                    {
                        return ApiResponseFactory.Fail(null, $"Mã QrCode {productRTCQRCode.ProductQRCode} đã được sử dụng vui lòng kiểm tra lại !");
                    }    
                }
                //else
                //{
                //    var codeExist1 = GetAll(x => x.SerialNumber!.ToLower().Trim() == productRTCQRCode.SerialNumber.ToLower().Trim() && x.WarehouseID == productRTCQRCode.WarehouseID && x.ID != productRTCQRCode.ID);
                //    if (codeExist1.Count>0)
                //    {
                //        return ApiResponseFactory.Fail(null, $"Mã SerialNumber {productRTCQRCode.SerialNumber}   đã được sử dụng vui lòng kiểm tra lại !");
                //    }
                //}
                return ApiResponseFactory.Success(null, "");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}
