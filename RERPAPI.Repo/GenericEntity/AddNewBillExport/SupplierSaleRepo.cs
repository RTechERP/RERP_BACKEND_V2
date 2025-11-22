using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Text.RegularExpressions;

namespace RERPAPI.Repo.GenericEntity
{
    public class SupplierSaleRepo : GenericRepo<SupplierSale>
    {

        public SupplierSaleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(SupplierSale item, out string message)
        {
            message = "";

            // Regex pattern
            string patternCode = @"^[^àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴ]+$";
            string patternPhone = @"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$";

            Regex regexCode = new Regex(patternCode);
            Regex regexPhone = new Regex(patternPhone);

            // 1. Validate Company
            if (item.Company <= 0)
            {
                message = "Vui lòng chọn Công ty!";
                return false;
            }

            // 2. Validate CodeNCC
            if (string.IsNullOrWhiteSpace(item.CodeNCC))
            {
                message = "Vui lòng nhập Mã NCC!";
                return false;
            }

            if (!regexCode.IsMatch(item.CodeNCC))
            {
                message = "Mã NCC chỉ chứa chữ cái tiếng Anh và số (không dấu)!";
                return false;
            }

            // Kiểm tra trùng mã
            var existCode = GetAll()
                .FirstOrDefault(x => x.CodeNCC == item.CodeNCC && x.ID != item.ID && x.IsDeleted != true);

            if (existCode != null)
            {
                message = $"Mã NCC [{item.CodeNCC}] đã tồn tại!";
                return false;
            }

            // 3. Validate Name
            if (string.IsNullOrWhiteSpace(item.NameNCC))
            {
                message = "Chưa có Tên NCC! Vui lòng kiểm tra lại.";
                return false;
            }

            // 5. Validate Address
            if (string.IsNullOrWhiteSpace(item.AddressNCC))
            {
                message = $"Chưa có Địa chỉ NCC {item.NameNCC}! Vui lòng kiểm tra lại.";
                return false;
            }

            // 6. Validate BankName
            if (string.IsNullOrWhiteSpace(item.NganHang))
            {
                message = $"Chưa có Ngân hàng NCC {item.NameNCC}! Vui lòng kiểm tra lại.";
                return false;
            }

            return true;
        }
    }
}
