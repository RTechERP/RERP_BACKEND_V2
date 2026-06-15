using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class UserGroupRepo : GenericRepo<UserGroup>
    {
        public UserGroupRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<APIResponse> Validate(UserGroup data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Code)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Mã nhóm!");
                if (string.IsNullOrWhiteSpace(data.Name)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Tên nhóm!");

                bool exists = await ExistsAsync(x => x.Code == data.Code && x.ID != data.ID);
                if (exists)
                {
                    return ApiResponseFactory.Fail(null, "Mã nhóm này đã tồn tại!");
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