using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Warehouses.AGV;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EconomicContractTermRepo : GenericRepo<EconomicContractTerm>
    {
        public EconomicContractTermRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public APIResponse Validate(EconomicContractTerm item)
        {
            try
            {

                if (!string.IsNullOrEmpty(item.TermCode) == false)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập mã điều khoản");
                }
                if (!string.IsNullOrEmpty(item.TermName) == false)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập tên điều khoản");
                }
                bool exists = GetAll().Any(x => x.TermCode == item.TermCode && x.ID != item.ID && x.IsDeleted != true);

                if (exists)
                {
                    return ApiResponseFactory.Fail(null, "Mã điều khoản hợp đồng đã tồn tại");
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
