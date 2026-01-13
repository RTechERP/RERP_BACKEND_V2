using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EconomicContractTypeRepo : GenericRepo<EconomicContractType>
    {
        public EconomicContractTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public APIResponse Validate(EconomicContractType item)
        {
            try
            {

                if (!string.IsNullOrEmpty(item.TypeCode) == false)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập mã loại hợp đồng");
                }
                if (!string.IsNullOrEmpty(item.TypeName) == false)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập tên điều khoản");
                }
                bool exists = GetAll().Any(x => x.TypeCode == item.TypeCode && x.ID != item.ID && x.IsDeleted != true);

                if (exists)
                {
                    return ApiResponseFactory.Fail(null, "Mã loại hợp đồng đã tồn tại");
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
