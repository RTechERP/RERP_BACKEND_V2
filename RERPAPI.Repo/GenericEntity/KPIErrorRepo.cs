using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIErrorRepo : GenericRepo<KPIError>
    {
        public KPIErrorRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool CheckValidate(KPIError model, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(model.Code))
            {
                message = "Vui lòng nhập Mã lỗi vi phạm!";
                return false;
            }

            if (model.KPIErrorTypeID <= 0)
            {
                message = "Vui lòng chọn Loại lỗi vi phạm!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.Content))
            {
                message = "Vui lòng nhập Nội dung lỗi vi phạm!";
                return false;
            }

            if (model.Monney < 0)
            {
                message = "Tiền phạt không được nhỏ hơn 0!";
                return false;
            }

            var exists = GetAll(x =>
                x.Code == model.Code.Trim()
                && x.ID != model.ID
                && x.IsDelete == false
                && x.DepartmentID == model.DepartmentID
            );

            if (exists.Any())
            {
                message = $"Mã lỗi [{model.Code.Trim()}] đã tồn tại!";
                return false;
            }

            return true;
        }
    }
}
