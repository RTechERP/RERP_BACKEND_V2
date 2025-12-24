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
    public class MenuAppRepo : GenericRepo<MenuApp>
    {
        public MenuAppRepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        public APIResponse Validate(MenuAppDTO menu)
        {

            try
            {
                var menus = GetAll(x => x.IsDeleted != true && x.ID != menu.ID);
                var response = ApiResponseFactory.Success(null, "");

                if (menu.STT <= 0)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập STT!");
                }

                if (string.IsNullOrWhiteSpace(menu.Code))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Mã!");
                }
                else
                {
                    var isCode = menus.Any(x => x.Code.Trim().ToLower() == menu.Code.Trim().ToLower());
                    if (isCode)
                    {
                        response = ApiResponseFactory.Fail(null, $"Mã [{menu.Code}] đã tồn tại!");
                    }
                }

                if (string.IsNullOrWhiteSpace(menu.Title))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Tiêu đề!");
                }

                if (!string.IsNullOrWhiteSpace(menu.Router))
                {
                    var isRouter = menus.Any(x => x.Router.Trim().ToLower() == menu.Router.Trim().ToLower());
                    if (isRouter)
                    {
                        response = ApiResponseFactory.Fail(null, $"Router [{menu.Router}] đã tồn tại!");
                    }
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
