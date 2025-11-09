using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Text.RegularExpressions;

namespace RERPAPI.Repo.GenericEntity
{
    public class PONCCRepo : GenericRepo<PONCC>
    {
        private PONCCDetailRepo _pONCCDetailRepo;
        private PONCCRulePayRepo _repoRulePay;
        private DocumentImportPONCCRepo _repoDocImport;
        private PONCCDetailRequestBuyRepo _repoDetailRequest;
        private BillImportDetailRepo _repoBillImport;
        private PONCCDetailLogRepo _repoDetailLog;
        private SupplierSaleRepo _supplierSaleRepo;

        public PONCCRepo(CurrentUser currentUser, PONCCDetailRepo pONCCDetailRepo, PONCCRulePayRepo pONCCRulePayRepo, DocumentImportPONCCRepo documentImportPONCCRepo, PONCCDetailRequestBuyRepo pONCCDetailRequestBuyRepo, BillImportDetailRepo billImportDetailRepo, PONCCDetailLogRepo pONCCDetailLogRepo, SupplierSaleRepo supplierSaleRepo) : base(currentUser)
        {
            _pONCCDetailRepo = pONCCDetailRepo;
            _repoRulePay = pONCCRulePayRepo;
            _repoDocImport = documentImportPONCCRepo;
            _repoDetailRequest = pONCCDetailRequestBuyRepo;
            _repoBillImport = billImportDetailRepo;
            _repoDetailLog = pONCCDetailLogRepo;
            _supplierSaleRepo = supplierSaleRepo;
        }

        public PONCCRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(PONCCDTO pONCCDTO, out string errorMessage)
        {
            string message = "";
            string pattern = @"^[a-zA-Z0-9_-]+$";
            Regex regex = new Regex(pattern);

            // Kiểm tra mã PO
            if (string.IsNullOrEmpty(pONCCDTO.POCode?.Trim()))
            {
                message = "Vui lòng nhập Mã PO NCC";
                errorMessage = message;
                return false;
            }
            else
            {
                bool isCheck = regex.IsMatch(pONCCDTO.POCode.Trim());
                if (!isCheck)
                {
                    message = "Mã PO NCC chỉ chứa chữ cái tiếng Anh và số!";
                    errorMessage = message;
                    return false;
                }
            }

            // Kiểm tra số đơn hàng
            if (string.IsNullOrEmpty(pONCCDTO.BillCode?.Trim()))
            {
                message = "Vui lòng nhập Số đơn hàng";
                errorMessage = message;
                return false;
            }
            else
            {
                bool isCheck = regex.IsMatch(pONCCDTO.BillCode.Trim());
                if (!isCheck)
                {
                    message = "Số đơn hàng chỉ chứa chữ cái tiếng Anh và số!";
                    errorMessage = message;
                    return false;
                }
            }

            // Kiểm tra trùng mã PO và số đơn hàng
            var existingPO = GetAll(p => p.POCode == pONCCDTO.POCode.Trim() && p.ID != pONCCDTO.ID && p.IsDeleted != true).FirstOrDefault();
            if (existingPO != null)
            {
                message = $"Mã PO NCC [{pONCCDTO.POCode.Trim()}] đã tồn tại!";
                errorMessage = message;
                return false;
            }

            var existingBill = GetAll(p => p.BillCode == pONCCDTO.BillCode.Trim() && p.ID != pONCCDTO.ID && p.IsDeleted != true && p.POType == pONCCDTO.POType).FirstOrDefault();
            if (existingBill != null)
            {
                message = $"Số đơn hàng [{pONCCDTO.BillCode.Trim()}] đã tồn tại!";
                errorMessage = message;
                return false;
            }

            // Kiểm tra nhân viên mua hàng
            if (pONCCDTO.EmployeeID <= 0)
            {
                message = "Vui lòng nhập NV mua hàng";
                errorMessage = message;
                return false;
            }

            // Kiểm tra điều khoản thanh toán
            if (pONCCDTO.lstPONCCRulePay == null || pONCCDTO.lstPONCCRulePay.Count == 0)
            {
                message = "Vui lòng chọn điều khoản thanh toán";
                errorMessage = message;
                return false;
            }

            // Kiểm tra công ty
            if (pONCCDTO.Company <= 0)
            {
                message = "Vui lòng nhập Công ty";
                errorMessage = message;
                return false;
            }

            // Kiểm tra ngày đơn hàng
            if (!pONCCDTO.RequestDate.HasValue)
            {
                message = "Vui lòng nhập Ngày đơn hàng";
                errorMessage = message;
                return false;
            }

            // Kiểm tra tổng tiền PO
            if (pONCCDTO.TotalMoneyPO <= 0)
            {
                message = "Vui lòng nhập Tổng tiền PO";
                errorMessage = message;
                return false;
            }

            // Kiểm tra loại tiền
            if (pONCCDTO.CurrencyID <= 0)
            {
                message = "Vui lòng nhập Loại tiền";
                errorMessage = message;
                return false;
            }

            // Kiểm tra ngày giao hàng
            if (!pONCCDTO.DeliveryDate.HasValue)
            {
                message = "Vui lòng nhập Ngày giao hàng";
                errorMessage = message;
                return false;
            }

            // Kiểm tra chi tiết PO
            if (pONCCDTO.lstPONCCDetail != null && pONCCDTO.lstPONCCDetail.Count > 0)
            {
                foreach (var detail in pONCCDTO.lstPONCCDetail)
                {
                    if (detail.QtyRequest <= 0)
                    {
                        message = $"Số lượng phải lớn hơn 0 (dòng {detail.STT})";
                        errorMessage = message;
                        return false;
                    }
                }
            }

            // Kiểm tra lý do không đạt chất lượng
            if (pONCCDTO.OrderQualityNotMet == true && string.IsNullOrWhiteSpace(pONCCDTO.ReasonForFailure))
            {
                message = "Vui lòng nhập lý do không đạt chất lượng!";
                errorMessage = message;
                return false;
            }

            errorMessage = message;
            return true;
        }

        public bool ValidateApproved(PONCC poncc, out string errorMessage)
        {
            string message = "";

            if (poncc == null || poncc.ID <= 0)
            {
                message = "Không tìm thấy thông tin PO!";
                errorMessage = message;
                return false;
            }

            errorMessage = message;
            return true;
        }

        public bool ValidateDelete(PONCC poncc, out string errorMessage)
        {
            string message = "";

            if (poncc == null || poncc.ID <= 0)
            {
                message = "Không tìm thấy thông tin PO!";
                errorMessage = message;
                return false;
            }

            // Kiểm tra nếu PO đã được duyệt thì không được xóa
            if (poncc.IsApproved == true)
            {
                message = "PO đã được duyệt, không thể xóa!";
                errorMessage = message;
                return false;
            }

            errorMessage = message;
            return true;
        }

        //public int GetSTT()
        //{
        //    var maxSTT = GetAll(p => p.IsDeleted != true).Max(p => p.STT);
        //    return maxSTT.HasValue ? maxSTT.Value + 1 : 1;
        //}
        public string GetPOCode(int supplierSaleID)
        {
            string code = "";
            SupplierSale supplier = _supplierSaleRepo.GetByID(supplierSaleID);
            if (supplier != null && supplier.ID > 0)
            {
                code = $"{DateTime.Now.ToString("MMyyyy")}-{supplier.CodeNCC}-";
                string currentCode = string.Empty;
                var data = SQLHelper<dynamic>.ProcedureToList("spGetPOCodeInPONCC",
                    new string[] { "@Value" },
                    new object[] { code });
                var dt = SQLHelper<dynamic>.GetListData(data, 0);
                if (dt.Count > 0)
                    currentCode = (dt[0].CurrentCode ?? "").ToString();
                int stt = 1;
                if (!string.IsNullOrEmpty(currentCode.Trim()))
                {
                    stt = Convert.ToInt32(currentCode.Substring(code.Length));
                    stt++;
                }

                string sttText = stt.ToString();
                while (sttText.Length < 3)
                {
                    sttText = $"0{sttText}";
                }
                code += sttText;
            }

            return code;
        }
        public string GetBillCode(PONCC model)
        {
            string code = model.POType == 0 ? "DMH" : "DEMO";

            var listPO = GetAll(x => !string.IsNullOrEmpty(x.BillCode) && x.BillCode.StartsWith(code))
                .Select(x => new
                {
                    ID = x.ID,
                    BillCode = x.BillCode,
                    STT = Convert.ToInt32(x.BillCode.Substring(code.Length))
                }).ToList();
            int stt = listPO.Count <= 0 ? 0 : listPO.Max(x => x.STT);
            stt++;

            string sttText = stt.ToString();
            while (sttText.Length < 5)
            {
                sttText = $"0{sttText}";
            }
            code += sttText;

            return code;
        }
    }
}