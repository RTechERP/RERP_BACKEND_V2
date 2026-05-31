using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class ConfigNotificationKeyRepo:GenericRepo<ConfigNotificationKey>
    {
        public ConfigNotificationKeyRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public APIResponse Validate(ConfigNotificationKey model)
        {
            try
            {
                if (model.KeyContent?.Length > 550)
                    return ApiResponseFactory.Fail(null, "Nội dung không được vượt quá 550 ký tự");
                if (model.KeyName?.Length > 500)
                    return ApiResponseFactory.Fail(null, "Tên không được vượt quá 500 ký tự");
                if (string.IsNullOrEmpty(model.KeyCode))
                    return ApiResponseFactory.Fail(null, "Mã key không được để trống");
                if (model.KeyCode?.Length > 200)
                    return ApiResponseFactory.Fail(null, "Mã key không được vượt quá 200 ký tự");

                var isDuplicate = GetAll().Any(x => x.KeyCode == model.KeyCode && x.ID != model.ID && x.IsDeleted != true);
                if (isDuplicate)
                    return ApiResponseFactory.Fail(null, "Mã key đã tồn tại trong hệ thống");

                return ApiResponseFactory.Success(null, "");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}
