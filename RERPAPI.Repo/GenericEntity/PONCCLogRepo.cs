using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class PONCCLogRepo : GenericRepo<PONCCLog>
    {
        #region Khai báo repo
        private CurrentUser _currentUser;
        private SupplierSaleRepo _supplierSaleRepo;
        private UserRepo _userRepo;
        private EmployeeRepo _employeeRepo;
        private SupplierRepo _supplierRepo;
        private CurrencyRepo _currencyRepo;
        private ProductSaleRepo _productSaleRepo;
        private ProductRTCRepo _productRtcRepo;
        private ProjectRepo _projectRepo;
        private TaxCompanyRepo _taxCompanyRepo;

        public PONCCLogRepo(
            CurrentUser currentUser,
            SupplierSaleRepo supplierSaleRepo,
            UserRepo userRepo,
            EmployeeRepo employeeRepo,
            SupplierRepo supplierRepo,
            CurrencyRepo currencyRepo,
            ProductSaleRepo productSaleRepo,
            ProductRTCRepo productRtcRepo,
            ProjectRepo projectRepo,
            TaxCompanyRepo taxCompanyRepo
            ) : base(currentUser)
        {
            _currentUser = currentUser;
            _supplierSaleRepo = supplierSaleRepo;
            _userRepo = userRepo;
            _employeeRepo = employeeRepo;
            _supplierRepo = supplierRepo;
            _currencyRepo = currencyRepo;
            _productSaleRepo = productSaleRepo;
            _productRtcRepo = productRtcRepo;
            _projectRepo = projectRepo;
            _taxCompanyRepo = taxCompanyRepo;
        }
        #endregion

        #region Log chính
        private static readonly Dictionary<string, string> _map = new()
        {
            { "ID", "id" },
            { "IsApproved", "trạng thái duyệt" },
            { "POCode", "mã PO" },
            { "UserNCC", "nhà cung cấp" },
            { "BillCode", "mã hóa đơn" },
            { "ReceivedDatePO", "ngày tạo PO" },
            { "TotalMoneyPO", "tổng tiền PO" },
            { "RequestDate", "ngày yêu cầu giao hàng" },
            { "UserName", "người liên hệ" },
            { "Phone", "số điện thoại" },
            { "Email", "email" },
            { "GroupID", "nhóm" },
            { "SupplierID", "nhà cung cấp" },
            { "UserID", "người dùng" },
            { "DeliveryDate", "ngày giao hàng" },
            { "ExpectedDate", "ngày dự kiến" },
            { "EmployeeID", "nhân viên mua hàng" },
            { "DeliveryTime", "số ngày giao hàng" },
            { "Note", "ghi chú" },
            { "Status", "tình trạng đơn hàng" },
            { "AccountNumber", "số tài khoản" },
            { "CreatedBy", "người tạo" },
            { "CreatedDate", "ngày tạo" },
            { "UpdatedBy", "người cập nhật" },
            { "UpdatedDate", "ngày cập nhật" },
            { "Status_Old", "trạng thái cũ" },
            { "NCCNew", "nhà cung cấp mới" },
            { "RuleIncoterm", "điều khoản incoterm" },
            { "RulePay", "điều khoản thanh toán" },
            { "BankingFee", "phí ngân hàng" },
            { "AddressDelivery", "địa chỉ giao hàng" },
            { "SupplierVoucher", "chứng từ nhà cung cấp" },
            { "Company", "công ty" },
            { "OriginItem", "xuất xứ hàng hóa" },
            { "CurrencyRate", "tỷ giá" },
            { "Currency", "tiền tệ" },
            { "FedexAccount", "tài khoản Fedex" },
            { "SupplierSaleID", "nhà cung cấp" },
            { "DeptSupplier", "công nợ nhà cung cấp" },
            { "AccountNumberSupplier", "số tài khoản nhà cung cấp" },
            { "BankSupplier", "ngân hàng nhà cung cấp" },
            { "BankCharge", "phí ngân hàng" },
            { "OtherTerms", "điều khoản khác" },
            { "OrderTargets", "mục tiêu đơn hàng" },
            { "CurrencyID", "loại tiền tệ" },
            { "IsDeleted", "đã xóa" },
            { "POType", "loại PO" },
            { "ShippingPoint", "điểm giao hàng" },
            { "OrderQualityNotMet", "không đạt chất lượng đơn hàng" },
            { "ReasonForFailure", "lý do không đạt" },
        };

        public static string GetDisplayName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _map.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLog(PONCC oldObj, PONCC newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(PONCC).GetProperties();

            var ignoreFields = new HashSet<string> { "CreatedDate", "UpdatedDate", "UpdatedBy", "CreatedBy", "ID" };


            foreach (var prop in props)
            {
                if (ignoreFields.Contains(prop.Name)) continue;

                var oldVal = prop.GetValue(oldObj);
                var newVal = prop.GetValue(newObj);

                if (oldVal is DateTime oldDate && newVal is DateTime newDate)
                {
                    if (oldDate.Date == newDate.Date) continue;
                }
                else
                {
                    if (Equals(oldVal, newVal)) continue;
                }

                if (oldVal == null || newVal == null) continue;

                string fieldName = GetDisplayName(prop.Name);

                string oldStr = FormatValue(prop.Name, oldVal);
                string newStr = FormatValue(prop.Name, newVal);

                changes.Add($"+ thay đổi {fieldName} từ '{oldStr}' thành '{newStr}'");
            }

            return string.Join("\n", changes);
        }

        #endregion

        #region Log chi tiết
        private static readonly Dictionary<string, string> _mapDetail = new()
        {
            { "ID", "id" },
            { "STT", "số thứ tự" },
            { "PONCCID", "PO nhà cung cấp" },
            { "ProductID", "sản phẩm" },
            { "Qty", "số lượng" },
            { "UnitPrice", "đơn giá" },
            { "IntoMoney", "thành tiền" },
            { "CodeBill", "mã hóa đơn" },
            { "NameBill", "tên hóa đơn" },
            { "RequestDate", "ngày yêu cầu giao hàng" },
            { "ActualDate", "ngày giao hàng thực tế" },
            { "RequestBuyRTCID", "yêu cầu mua RTC" },
            { "QtyRequest", "số lượng yêu cầu" },
            { "QtyReal", "số lượng thực tế" },
            { "Soluongcon", "số lượng còn" },
            { "Price", "giá" },
            { "VAT", "thuế VAT" },
            { "VATMoney", "tiền VAT" },
            { "ThanhTien", "thành tiền" },
            { "TotalPrice", "tổng tiền" },
            { "OrderDate", "ngày đặt hàng" },
            { "ExpectedDate", "ngày dự kiến" },
            { "FeeShip", "phí vận chuyển" },
            { "Note", "ghi chú" },
            { "CreatedDate", "ngày tạo" },
            { "CreatedBy", "người tạo" },
            { "UpdatedDate", "ngày cập nhật" },
            { "UpdatedBy", "người cập nhật" },
            { "PriceSale", "giá bán" },
            { "CurrencyExchange", "tỷ giá" },
            { "Discount", "chiết khấu" },
            { "ProfitRate", "tỷ suất lợi nhuận" },
            { "PriceHistory", "giá lịch sử" },
            { "ProductCodeOfSupplier", "mã sản phẩm nhà cung cấp" },
            { "Status", "trạng thái" },
            { "ProjectPartlistPurchaseRequestID", "yêu cầu mua vật tư" },
            { "DiscountPercent", "phần trăm chiết khấu" },
            { "BiddingPrice", "giá đấu thầu" },
            { "ProductSaleID", "sản phẩm sale" },
            { "ProjectID", "dự án" },
            { "ProductRTCID", "sản phẩm RTC" },
            { "ProjectName", "tên dự án" },
            { "DeadlineDelivery", "hạn giao hàng" },
            { "ProjectPartListID", "partlist dự án" },
            { "IsBill", "có hóa đơn" },
            { "ProductType", "loại hàng hóa" },
            { "DateReturnEstimated", "ngày dự kiến hoàn trả" },
            { "IsStock", "hàng tồn kho" },
            { "UnitName", "đơn vị tính" },
            { "ParentProductCode", "mã sản phẩm cha" },
            { "IsPurchase", "đã mua hàng" },
        };

        public static string GetDisplayNameDetail(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _mapDetail.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLogDetail(PONCCDetail oldObj, PONCCDetail newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(PONCCDetail).GetProperties();

            var ignoreFields = new HashSet<string> { "CreatedDate", "UpdatedDate", "UpdatedBy", "CreatedBy", "ID", "ProjectName" };


            foreach (var prop in props)
            {
                if (ignoreFields.Contains(prop.Name)) continue;

                var oldVal = prop.GetValue(oldObj);
                var newVal = prop.GetValue(newObj);

                if (oldVal is DateTime oldDate && newVal is DateTime newDate)
                {
                    if (oldDate.Date == newDate.Date) continue;
                }
                else
                {
                    if (Equals(oldVal, newVal)) continue;
                }

                string fieldName = GetDisplayNameDetail(prop.Name);

                string oldStr = FormatValue(prop.Name, oldVal);
                string newStr = FormatValue(prop.Name, newVal);

                changes.Add($"+ thay đổi {fieldName} từ '{oldStr}' thành '{newStr}'");
            }

            return string.Join("\n", changes);
        }

        #endregion

        #region Khác
        private string FormatValue(string fieldName, object value)
        {
            if (value == null) return "rỗng";
            string msg = "";

            switch (fieldName)
            {
                case "SupplierSaleID":
                    int supplierSaleID = Convert.ToInt32(value);
                    msg = _supplierSaleRepo.GetByID(supplierSaleID).CodeNCC;
                    return msg;
                case "EmployeeID":
                    int employeeID = Convert.ToInt32(value);
                    msg = _employeeRepo.GetByID(employeeID).FullName;
                    return msg;
                case "ProductSaleID":
                    int productSaleID = Convert.ToInt32(value);
                    msg = _productSaleRepo.GetByID(productSaleID).ProductCode;
                    return msg;
                case "ProductRTCID":
                    int productRTCID = Convert.ToInt32(value);
                    msg = _productRtcRepo.GetByID(productRTCID).ProductCode;
                    return msg;
                case "ProjectID":
                    int projectID = Convert.ToInt32(value);
                    msg = _projectRepo.GetByID(projectID).ProjectCode;
                    return msg;
                case "SupplierID":
                    int SupplierID = Convert.ToInt32(value);
                    msg = _supplierRepo.GetByID(SupplierID).SupplierCode;
                    return msg;
                case "UserID":
                    int UserID = Convert.ToInt32(value);
                    msg = _userRepo.GetByID(UserID).FullName;
                    return msg;
                case "CurrencyID":
                    int CurrencyID = Convert.ToInt32(value);
                    msg = _currencyRepo.GetByID(CurrencyID).Code;
                    return msg;
                case "POType":
                    int POType = Convert.ToInt32(value);
                    if (POType == 1) msg = "PO thương mại";
                    if (POType == 2) msg = "PO mượn";
                    return msg;
                case "Company":
                    int Company = Convert.ToInt32(value);
                    msg = _taxCompanyRepo.GetByID(Company).Code;
                    return msg;
                default:
                    break;
            }

            // ===== BOOL =====
            if (value is bool b)
            {
                return b ? "True" : "False";
            }
            
            // ===== DATE =====
            if (value is DateTime dt)
            {
                return dt.ToString("dd/MM/yyyy");
            }

            // ===== NUMBER =====
            if (value is decimal || value is double || value is float)
            {
                return string.Format("{0:N0}", value);
            }

            return value.ToString();
        }

        public async Task AddLog(int ponccID, string logContent, string typeLog)
        {
            PONCCLog log = new PONCCLog();
            log.PONCCID = ponccID;
            log.TypeLog = typeLog;
            log.ContentLog = logContent;
            log.CreatedBy = _currentUser.LoginName;
            log.CreatedDate = DateTime.Now;
            log.IsDeleted = false;

            await CreateAsync(log);
        }

        #endregion

    }
}
