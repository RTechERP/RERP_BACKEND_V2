using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductRTCRepo : GenericRepo<ProductRTC>
    {
        public ProductRTCRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public bool checkExistProductCodeRTC(ProductRTC model)
        {
            var exist = GetAll(x => x.ProductCode == model.ProductCode && x.ID != model.ID && x.IsDelete != true).Any();
            return exist;
        }
        public bool checkExistSerialRTC(ProductRTC model)
        {
            var exist = GetAll(x => x.SerialNumber == model.SerialNumber && x.ID != model.ID && x.IsDelete != true).Any();
            return exist;
        }
        public bool checkExistPartnumberRTC(ProductRTC model)
        {
            var exist = GetAll(x => x.PartNumber == model.SerialNumber && x.ID != model.ID && x.IsDelete != true).Any();
            return exist;
        }
        public string generateProductCode()
        {
            string numberCodeDefault = "00000001";
            string productCodeRTC = "Z";

            // Lấy tất cả mã sản phẩm có tiền tố "Z"
            var listProductCodes = table
                .Where(x => !string.IsNullOrWhiteSpace(x.ProductCodeRTC) && x.ProductCodeRTC.StartsWith("Z"))
                .Select(x => x.ProductCodeRTC)
                .ToList();

            // Xử lý phần số sau ký tự "Z"
            var listWithSTT = listProductCodes.Select(code =>
            {
                string numberPart = code.Substring(1); // Bỏ chữ Z
                bool success = int.TryParse(numberPart, out int stt);
                return new { Code = code, STT = success ? stt : 0 };
            }).ToList();

            int numberCode = listWithSTT.Count == 0 ? 0 : listWithSTT.Max(x => x.STT);
            string numberCodeText = (++numberCode).ToString();

            // Thêm số 0 vào đầu nếu chưa đủ 8 ký tự
            while (numberCodeText.Length < numberCodeDefault.Length)
            {
                numberCodeText = "0" + numberCodeText;
            }

            productCodeRTC += numberCodeText;
            return productCodeRTC;
        }

    }
}
