using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class RegisterContractRepo : GenericRepo<RegisterContract>
    {
        public RegisterContractRepo(CurrentUser currentUser) : base(currentUser)
        {

        }
        public bool Validate(
         RegisterContract data,
         out string message)
        {
            string projectCode = string.Empty;
            message = string.Empty;

            // Kiểm tra null
            if (data == null)
            {
                message = "Không có dữ liệu. Vui lòng kiểm tra lại!";
                return false;
            }

            if (data.TaxCompanyID <= 0)
            {
                message = "Vui lòng nhập tên công ty!";
                return false;
            }
            if (data.ContractTypeID <= 0)
            {
                message = "Vui lòng nhập loại hồ sơ!";
                return false;
            }

            if (!data.RegistedDate.HasValue)
            {
                message = "Vui lòng nhập ngày đăng ký!";
                return false;
            }

            if (data.EmployeeReciveID <= 0)
            {
                message = "Vui lòng nhập người nhận!";
                return false;
            }

            if (data.DocumentTypeID <= 0)
            {
                message = "Vui lòng nhập loại văn bản!";
                return false;
            }

            if (data.DocumentQuantity <= 0)
            {
                message = "Vui lòng nhập số lượng bản!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(data.DocumentName))
            {
                message = "Vui lòng nhập văn bản!";
                return false;
            }
            return true;
        }
    }
}
