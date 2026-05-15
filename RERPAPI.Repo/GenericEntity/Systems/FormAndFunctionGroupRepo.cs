using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Linq;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class FormAndFunctionGroupRepo : GenericRepo<FormAndFunctionGroup>
    {
        public FormAndFunctionGroupRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public APIResponse Validate(FormAndFunctionGroup model)
        {
            try
            {
                var existing = GetAll(x => !x.IsHide && x.ID != model.ID);

                if (string.IsNullOrWhiteSpace(model.Code))
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập Mã nhóm!");

                if (existing.Any(x => x.Code != null && x.Code.Trim().ToLower() == model.Code.Trim().ToLower()))
                    return ApiResponseFactory.Fail(null, $"Mã [{model.Code}] đã tồn tại!");

                if (string.IsNullOrWhiteSpace(model.Name))
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập Tên nhóm!");

                return ApiResponseFactory.Success(null, "");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}
