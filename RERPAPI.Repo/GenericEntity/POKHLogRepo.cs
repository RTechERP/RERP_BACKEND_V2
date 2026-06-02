using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class POKHLogRepo : GenericRepo<POKHLog>
    {
        private CurrentUser _currentUser;
        private CustomerRepo _customerRepo;
        private ProjectRepo _projectRepo;
        private UserRepo _userRepo;
        private CurrencyRepo _currencyRepo;
        private CustomerPartsRepo _customerPartsRepo;
        private ProductSaleRepo _productSaleRepo;

        //private po _userRepo;
        public POKHLogRepo(
            CurrentUser currentUser,
            CustomerRepo customerRepo,
            ProjectRepo projectRepo,
            UserRepo userRepo,
            CurrencyRepo currencyRepo,
            CustomerPartsRepo customerPartsRepo,
            ProductSaleRepo productSaleRepo
            ) : base(currentUser)
        {
            _currentUser = currentUser;
            _customerRepo = customerRepo;
            _projectRepo = projectRepo;
            _userRepo = userRepo;
            _currencyRepo = currencyRepo;
            _customerPartsRepo = customerPartsRepo;
            _productSaleRepo = productSaleRepo;
        }

        #region Log chính

        private static readonly Dictionary<string, string> _map = new()
        {
            { "Status", "trạng thái" },
            { "POCode", "mã PO" },
            { "UserName", "người phụ trách" },
            { "ProjectID", "dự án" },
            { "BillCode", "mã hóa đơn" },
            { "ReceivedDatePO", "ngày nhận PO" },
            { "TotalMoneyPO", "tổng tiền PO" },
            { "StartDate", "ngày bắt đầu" },
            { "EndDate", "ngày kết thúc giao hàng" },
            { "DeliveryStatus", "tình trạng giao hàng" },
            { "ImportStatus", "tình trạng nhập kho" },
            { "ExportStatus", "tình trạng xuất kho" },
            { "Note", "ghi chú" },
            { "CustomerID", "khách hàng" },
            { "EndUserID", "người dùng cuối" },
            { "EndUser", "người dùng cuối" },
            { "DealerID", "đại lý" },
            { "IsApproved", "đã duyệt" },
            { "POType", "loại PO" },
            { "Month", "tháng" },
            { "Year", "năm" },
            { "UserID", "người tạo" },
            { "CreatedDate", "ngày tạo" },
            { "UpdatedDate", "ngày cập nhật" },
            { "ReceiveMoney", "tiền đã nhận" },
            { "IsPay", "đã thanh toán" },
            { "IsShip", "đã giao hàng" },
            { "IsExport", "đã xuất kho" },
            { "IsBill", "trạng thái hóa đơn" },
            { "TotalMoneyKoVAT", "tổng tiền (không VAT)" },
            { "PaymentStatus", "tình trạng thanh toán" },
            { "Discount", "chiết khấu (%)" },
            { "TotalMoneyDiscount", "tổng tiền sau CK" },
            { "CurrencyID", "loại tiền" },
            { "PartID", "bộ phận" },
            { "MoneyDiscount", "tổng tiền chiết khấu" },
            { "AccountType", "loại tài khoản" },
        };

        public static string GetDisplayName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _map.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLog(POKH oldObj, POKH newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(POKH).GetProperties();

            var ignoreFields = new HashSet<string> { "CreatedDate", "UpdatedDate" };

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

                string fieldName = GetDisplayName(prop.Name);

                string oldStr = FormatValueMaster(prop.Name, oldVal);
                string newStr = FormatValueMaster(prop.Name, newVal);

                changes.Add($"+ thay đổi {fieldName} từ '{oldStr}' thành '{newStr}'");
            }

            return string.Join("\n", changes);
        }

        #endregion Log chính

        #region Log chi tiết

        private static readonly Dictionary<string, string> _mapDetail = new()
        {
            { "ProductID", "sản phẩm" },
            { "KHID", "khách hàng" },
            { "Qty", "số lượng" },
            { "UnitPrice", "đơn giá" },
            { "IntoMoney", "thành tiền" },
            { "FilmSize", "kích thước film" },
            { "IndexPO", "index PO" },
            { "BillNumber", "số hóa đơn" },
            { "VAT", "VAT (%)" },
            { "TotalPriceIncludeVAT", "tổng tiền (có VAT)" },
            { "RecivedMoneyDate", "ngày nhận tiền" },
            { "BillDate", "ngày hóa đơn" },
            { "ActualDeliveryDate", "ngày giao thực tế" },
            { "DeliveryRequestedDate", "ngày yêu cầu giao" },
            { "PayDate", "ngày thanh toán" },
            { "EstimatedPay", "giá trị dự kiến thanh toán" },
            { "STT", "số thứ tự" },
            { "IsOder", "đã đặt hàng" },
            { "GroupPO", "nhóm PO" },
            { "GuestCode", "mã khách" },
            { "QtyTT", "số lượng thực tế" },
            { "QtyCL", "số lượng còn lại" },
            { "IsExport", "đã xuất kho" },
            { "Debt", "công nợ" },
            { "UserReceiver", "người nhận tiền" },
            { "QtyRequest", "số lượng yêu cầu" },
            { "NetUnitPrice", "đơn giá net" },
            { "Note", "ghi chú" },
            { "CurrencyID", "loại tiền tệ" },
            { "Spec", "quy cách" },
            { "IsDelivered", "đã giao hàng" },
            { "AccountType", "loại tài khoản" },
        };

        public static string GetDisplayNameDetail(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _mapDetail.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLogDetail(POKHDetail oldObj, POKHDetail newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(POKHDetail).GetProperties();

            var ignoreFields = new HashSet<string> { "CreatedDate", "UpdatedDate", "QuotationDetailID" };

            foreach (var prop in props)
            {
                if (ignoreFields.Contains(prop.Name)) continue;

                var oldVal = prop.GetValue(oldObj);
                var newVal = prop.GetValue(newObj);

                if (oldVal == null || newVal == null) continue;

                if (Equals(oldVal, newVal)) continue;

                string fieldName = GetDisplayNameDetail(prop.Name);

                string oldStr = FormatValueMaster(prop.Name, oldVal);
                string newStr = FormatValueMaster(prop.Name, newVal);

                changes.Add($"+ thay đổi {fieldName} từ '{oldStr}' thành '{newStr}'");
            }

            return string.Join("\n", changes);
        }

        #endregion Log chi tiết

        #region Log người phụ trách

        private static readonly Dictionary<string, string> _mapUser = new()
            {
                { "UserID", "người nhận" },
                { "MoneyUser", "số tiền phân bổ" },
                { "PercentUser", "tỷ lệ (%)" },
                { "ReceiveMoney", "số tiền đã nhận" },
                { "Month", "tháng" },
                { "Year", "năm" },
                { "STT", "số thứ tự" },
                { "RowHandle", "dòng xử lý" }
            };

        public static string GetDisplayNameUser(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _mapUser.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLogUser(POKHDetailMoney oldObj, POKHDetailMoney newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(POKHDetailMoney).GetProperties();

            var ignoreFields = new HashSet<string> { "CreatedDate", "UpdatedDate" };

            foreach (var prop in props)
            {
                if (ignoreFields.Contains(prop.Name)) continue;

                var oldVal = prop.GetValue(oldObj);
                var newVal = prop.GetValue(newObj);

                if (Equals(oldVal, newVal)) continue;

                string fieldName = GetDisplayNameUser(prop.Name);

                string oldStr = FormatValueMaster(prop.Name, oldVal);
                string newStr = FormatValueMaster(prop.Name, newVal);

                changes.Add($"- thay đổi {fieldName} từ {oldStr} thành {newStr}.");
            }

            return string.Join("\n", changes);
        }

        #endregion Log người phụ trách

        #region Khác

        private string FormatValueMaster(string fieldName, object value)
        {
            if (value == null) return "rỗng";
            string msg = "";

            switch (fieldName)
            {
                case "CustomerID":
                    int customerID = Convert.ToInt32(value);
                    msg = _customerRepo.GetByID(customerID).CustomerName;
                    return msg;

                case "KHID":
                    int KHID = Convert.ToInt32(value);
                    msg = _customerRepo.GetByID(KHID).CustomerName;
                    return msg;

                case "UserID":
                    int userID = Convert.ToInt32(value);
                    msg = _userRepo.GetByID(userID).FullName;
                    return msg;

                case "ProjectID":
                    int projectID = Convert.ToInt32(value);
                    msg = _projectRepo.GetByID(projectID).ProjectName;
                    return msg;

                case "DealerID":
                    int dealerID = Convert.ToInt32(value);
                    msg = _userRepo.GetByID(dealerID).FullName;
                    return msg;

                case "EndUserID":
                    int endUserID = Convert.ToInt32(value);
                    msg = _userRepo.GetByID(endUserID).FullName;
                    return msg;

                case "CurrencyID":
                    int currencyID = Convert.ToInt32(value);
                    msg = _currencyRepo.GetByID(currencyID).Code;
                    return msg;

                case "PartID":
                    int partID = Convert.ToInt32(value);
                    msg = _customerPartsRepo.GetByID(partID).PartName;
                    return msg;

                case "ProductID":
                    int ProductID = Convert.ToInt32(value);
                    msg = _productSaleRepo.GetByID(ProductID).ProductName;
                    return msg;

                case "AccountType":
                    int AccountType = Convert.ToInt32(value);
                    if (AccountType == 1) msg = "Big Account";
                    if (AccountType == 2) msg = "Minor Account";
                    return msg;

                case "POType":
                    List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetMainIndex", new string[] { "@Type" }, new object[] { 0 });
                    var data = SQLHelper<dynamic>.GetListData(list, 0);

                    int poType = Convert.ToInt32(value);

                    var item = data.FirstOrDefault(x => x.ID == poType);
                    msg = item.MainIndex?.ToString();
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

        public async Task AddLog(int poKHID, string logContent, string typeLog)
        {
            POKHLog log = new POKHLog();
            log.POKHID = poKHID;
            log.TypeLog = typeLog;
            log.ContentLog = logContent;
            log.CreatedBy = _currentUser.LoginName;
            log.CreatedDate = DateTime.Now;
            log.IsDeleted = false;

            await CreateAsync(log);
        }

        #endregion Khác
    }
}