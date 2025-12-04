using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Data;
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
        private ProjectPartlistPurchaseRequestRepo _prjPartListRepo;
        CurrentUser _currentUser;

        public PONCCRepo(CurrentUser currentUser,
            PONCCDetailRepo pONCCDetailRepo,
            PONCCRulePayRepo pONCCRulePayRepo,
            DocumentImportPONCCRepo documentImportPONCCRepo,
            PONCCDetailRequestBuyRepo pONCCDetailRequestBuyRepo,
            BillImportDetailRepo billImportDetailRepo,
            PONCCDetailLogRepo pONCCDetailLogRepo,
            ProjectPartlistPurchaseRequestRepo prjPartListRepo,
            SupplierSaleRepo supplierSaleRepo) : base(currentUser)
        {

            _currentUser = currentUser;
            _pONCCDetailRepo = pONCCDetailRepo;
            _repoRulePay = pONCCRulePayRepo;
            _repoDocImport = documentImportPONCCRepo;
            _repoDetailRequest = pONCCDetailRequestBuyRepo;
            _repoBillImport = billImportDetailRepo;
            _repoDetailLog = pONCCDetailLogRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _prjPartListRepo = prjPartListRepo;
        }

        public PONCCRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(PONCCDTO pONCCDTO, out string message)
        {
            message = "";
            string pattern = @"^[a-zA-Z0-9_-]+$";
            Regex regex = new Regex(pattern);
            PONCC model = pONCCDTO.poncc;
            var lstPONCCDetail = pONCCDTO.lstPONCCDetail;

            #region validate cho poncc bao gôm rulepayId
            if (model.ID > 0)
            {
                if (!(model.Status == 0 || model.Status == 5) && !(_currentUser.EmployeeID == 178 || _currentUser.IsAdmin))
                {
                    var oldStatus = model.Status == 0 ? "Đang tiến hành" : "Đã Y/c nhập kho";
                    message = $"Trạng thái PONCC [{oldStatus}]! Không được sửa";
                    return false;
                }
            }

            if (model?.SupplierSaleID <= 0)
            {
                message = "Vui lòng chọn nhà cung cấp!";
                return false;
            }

            if (string.IsNullOrEmpty(model.POCode?.Trim()))
            {
                message = "Vui lòng nhập Mã PO NCC";
                return false;
            }
            else
            {
                bool isCheck = regex.IsMatch(model.POCode.Trim());
                if (!isCheck)
                {
                    message = "Mã PO NCC chỉ chứa chữ cái tiếng Anh và số!";
                    return false;
                }
            }

            // Kiểm tra số đơn hàng
            if (string.IsNullOrEmpty(model.BillCode?.Trim()))
            {
                message = "Vui lòng nhập Số đơn hàng";
                return false;
            }
            else
            {
                bool isCheck = regex.IsMatch(model.BillCode.Trim());
                if (!isCheck)
                {
                    message = "Số đơn hàng chỉ chứa chữ cái tiếng Anh và số!";
                    return false;
                }
            }

            var existingPO = GetAll(p => p.POCode == model.POCode.Trim() && p.ID != model.ID && p.IsDeleted != true).FirstOrDefault();
            if (existingPO != null)
            {
                message = $"Mã PO NCC [{model.POCode.Trim()}] đã tồn tại!";
                return false;
            }

            var existingBill = GetAll(p => p.BillCode == model.BillCode.Trim() && p.ID != model.ID && p.IsDeleted != true && p.POType == model.POType).FirstOrDefault();
            if (existingBill != null)
            {
                message = $"Số đơn hàng [{model.BillCode.Trim()}] đã tồn tại!";
                return false;
            }

            if (model.EmployeeID <= 0)
            {
                message = "Vui lòng nhập NV mua hàng";
                return false;
            }

            if (pONCCDTO.RulePayID <= 0)
            {
                message = "Vui lòng chọn điều khoản thanh toán!";
                return false;
            }

            // Kiểm tra công ty
            if (model.Company <= 0)
            {
                message = "Vui lòng nhập Công ty";
                return false;
            }

            // Kiểm tra ngày đơn hàng
            if (!model.RequestDate.HasValue)
            {
                message = "Vui lòng nhập Ngày đơn hàng";
                return false;
            }

            // Kiểm tra tổng tiền PO
            if (model.TotalMoneyPO <= 0)
            {
                message = "Tổng tiền PO cần lớn hơn 0. Vui lòng chọn hoặc chỉnh sửa tổng tiền sản phẩm mua!";
                return false;
            }

            // Kiểm tra loại tiền
            if (model.CurrencyID <= 0)
            {
                message = "Vui lòng nhập Loại tiền";
                return false;
            }

            // Kiểm tra ngày giao hàng
            if (!model.DeliveryDate.HasValue)
            {
                message = "Vui lòng nhập Ngày giao hàng";
                return false;
            }
            #endregion

            if (pONCCDTO.lstPrjPartlistPurchaseRequest.Count() > 0)
            {
                if ((bool)pONCCDTO.IsCheckTotalMoneyPO)
                {
                    decimal totalPrice = (decimal)pONCCDTO.poncc.TotalMoneyPO;
                    decimal totalPriceRequest = pONCCDTO.lstPrjPartlistPurchaseRequest.Sum(x =>
                    (Convert.ToDecimal(x.Quantity) * Convert.ToDecimal(x.UnitPrice)));

                    if (totalPrice > totalPriceRequest)
                    {
                        message = $@"Tổng Thành tiền không được lớn hơn tổng Thành tiền duyệt mua ({totalPriceRequest.ToString("n2")}). 
                                Vui lòng kiểm tra lại!";
                        return false;
                    }
                }
                else
                {
                    foreach (var item in lstPONCCDetail)
                    {
                        int purchaseRequestId = Convert.ToInt32(item.ProjectPartlistPurchaseRequestID);
                        ProjectPartlistPurchaseRequest purchaseRequest = _prjPartListRepo.GetByID(purchaseRequestId);
                        if (purchaseRequest == null || purchaseRequest.ID <= 0) continue;

                        decimal unitPrice = Convert.ToDecimal(item.UnitPrice);
                        int stt = Convert.ToInt32(item.STT);

                        if (unitPrice > purchaseRequest.UnitPrice)
                        {
                            message = $"Đơn giá mua không được lớn hơn đơn giá duyệt mua.\nVui lòng kiểm tra lại (Stt: {stt})";
                            return false;
                        }
                    }
                }
            }

            //check lại sau 
            if ((bool)model.OrderQualityNotMet && string.IsNullOrWhiteSpace(model.ReasonForFailure))
            {
                message = $"Vui lòng nhập lý do không đạt chất lượng!";
                return false;
            }

            return true;
        }

        public bool ValidateApproved(PONCC poncc, out string message)
        {
            message = "";

            if (poncc == null || poncc.ID <= 0)
            {
                message = "Không tìm thấy thông tin PO!";

                return false;
            }
            return true;
        }

        public bool ValidateDelete(PONCC poncc, out string message)
        {
            message = "";

            if (poncc == null || poncc.ID <= 0)
            {
                message = "Không tìm thấy thông tin PO!";

                return false;
            }

            // Kiểm tra nếu PO đã được duyệt thì không được xóa
            if (poncc.IsApproved == true)
            {
                message = "PO đã được duyệt, không thể xóa!";

                return false;
            }


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
        public async Task UpdateTinhHinhDonHang(BillImportApprovedDTO lstModels, bool isapproved)
        {
            int PONCCID = 0;
            if (lstModels.billImportDetails.Count <= 0) return;
            foreach (var item in lstModels.billImportDetails)
            {
                int PONCCDetailID = item.PONCCDetailID ?? 0;
                if (PONCCDetailID <= 0) return;
                PONCCDetail model = _pONCCDetailRepo.GetByID(PONCCDetailID);
                if (isapproved)
                {
                    model.QtyReal = model.QtyReal + item.Qty;
                    model.Soluongcon = model.QtyRequest - model.QtyReal;
                }
                else
                {
                    model.QtyReal = model.QtyReal - item.Qty;
                    model.Soluongcon = model.QtyRequest - model.QtyReal;
                }
                model.Status = model.Soluongcon == 0 ? 1 : 0;
                PONCCID = model.PONCCID ?? 0;
                await _pONCCDetailRepo.UpdateAsync(model);
                List<PONCCDetail> pONCCDetails = _pONCCDetailRepo.GetAll(x => x.PONCCID == PONCCID);
                if (pONCCDetails.Count <= 0) return;
                PONCC pONCC = GetByID(PONCCID);//Đang tiến hành 0//Hoàn thành 1//Huỷ 2
                if (CheckPONCCDetaila(pONCCDetails))
                {
                    if (pONCC.Status == 0)
                    {
                        pONCC.Status = 1;
                    }
                }
                else
                {
                    pONCC.Status = pONCC.Status == 1 ? 0 : pONCC.Status_Old;
                }
                await UpdateAsync(pONCC);
            }
        }
        private bool CheckPONCCDetaila(List<PONCCDetail> dt)
        {
            int dem = 0;
            for (int i = 0; i < dt.Count; i++)
            {
                dem = dt[i].Soluongcon == 0 ? dem + 1 : dem;

            }
            if (dem == dt.Count) return true;
            return false;
        }
    }
}