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
    public class KPIErrorTypeRepo : GenericRepo<KPIErrorType>
    {
        public KPIErrorTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool CheckValidate(KPIErrorType model, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(model.Code))
            {
                message = "Vui lòng nhập Mã loại lỗi!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                message = "Vui lòng nhập Tên loại lỗi!";
                return false;
            }

            var existCode = GetAll(x =>
                x.ID != model.ID &&
                x.Code == model.Code.Trim() &&
                x.IsDelete == false
            ).FirstOrDefault();

            if (existCode != null)
            {
                message = $"Mã loại lỗi [{model.Code.Trim()}] đã tồn tại!";
                return false;
            }

            var existSTT = GetAll(x =>
                x.ID != model.ID &&
                x.STT == model.STT &&
                x.IsDelete == false
            ).FirstOrDefault();

            if (existSTT != null)
            {
                message = $"STT [{model.STT}] đã tồn tại!";
                return false;
            }

            return true;
        }

    }
}
