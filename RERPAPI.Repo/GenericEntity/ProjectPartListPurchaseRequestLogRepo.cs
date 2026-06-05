using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistPurchaseRequestLogRepo : GenericRepo<ProjectPartListPurchaseRequestLog>
    {
        #region Khai báo repo

        private CurrentUser _currentUser;
        private EmployeeRepo _employeeRepo;
        private CurrencyRepo _currencyRepo;

        //private ProjectPartlistPurchaseRequestRepo _projectPartlistPurchaseRequestRepo;
        private UnitCountRepo _unitCountRepo;

        private SupplierSaleRepo _supplierSaleRepo;
        private ProductSaleRepo _productSaleRepo;
        private ProductGroupRepo _productGroupRepo;
        private ProductGroupRTCRepo _productGroupRTCRepo;
        private ProductRTCRepo _productRTCRepo;
        private WarehouseRepo _warehouseRepo;
        private ProjectPartlistPriceRequestTypeRepo _projectPartlistPriceRequestTypeRepo;

        public ProjectPartlistPurchaseRequestLogRepo(
            CurrentUser currentUser,
            EmployeeRepo employeeRepo,
            CurrencyRepo currencyRepo,
            //ProjectPartlistPurchaseRequestRepo projectPartlistPurchaseRequestRepo,
            UnitCountRepo unitCountRepo,
            SupplierSaleRepo supplierSaleRepo,
            ProductSaleRepo productSaleRepo,
            ProductGroupRepo productGroupRepo,
            ProductGroupRTCRepo productGroupRTCRepo,
            ProductRTCRepo productRTCRepo,
            WarehouseRepo warehouseRepo,
            ProjectPartlistPriceRequestTypeRepo projectPartlistPriceRequestTypeRepo
            ) : base(currentUser)
        {
            _currentUser = currentUser;
            _employeeRepo = employeeRepo;
            _currencyRepo = currencyRepo;
            //_projectPartlistPurchaseRequestRepo = projectPartlistPurchaseRequestRepo;
            _unitCountRepo = unitCountRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _productSaleRepo = productSaleRepo;
            _productGroupRepo = productGroupRepo;
            _productGroupRTCRepo = productGroupRTCRepo;
            _productRTCRepo = productRTCRepo;
            _warehouseRepo = warehouseRepo;
            _projectPartlistPriceRequestTypeRepo = projectPartlistPriceRequestTypeRepo;
        }

        #endregion Khai báo repo

        #region Lưu log master

        private static readonly Dictionary<string, string> _map = new()
        {
            { "ID", "ID" },
            { "ProjectPartListID", "hạng mục dự án" },
            { "EmployeeID", "nhân viên phụ trách" },
            { "UnitCountID", "đơn vị tính" },
            { "SupplierSaleID", "nhà cung cấp" },
            { "ApprovedTBP", "người duyệt TBP" },
            { "ApprovedBGD", "người duyệt BGD" },
            { "ProductSaleID", "sản phẩm NCC" },
            { "ProductGroupID", "nhóm sản phẩm" },
            { "CurrencyID", "loại tiền tệ" },
            { "EmployeeIDRequestApproved", "người duyệt yêu cầu" },
            { "POKHDetailID", "chi tiết POKH" },
            { "JobRequirementID", "yêu cầu công việc" },
            { "InventoryProjectID", "dự án kho" },
            { "ProductGroupRTCID", "nhóm sản phẩm RTC" },
            { "ProductRTCID", "sản phẩm RTC" },
            { "EmployeeApproveID", "người phê duyệt" },
            { "ProjectPartlistPurchaseRequestTypeID", "loại yêu cầu mua hàng" },
            { "DuplicateID", "bản ghi gốc" },
            { "WarehouseID", "kho" },
            { "ProjectPartlistPriceRequestID", "yêu cầu báo giá" },

            { "ProductCode", "mã sản phẩm" },
            { "ProductName", "tên sản phẩm" },
            { "StatusRequest", "trạng thái yêu cầu" },
            { "DateRequest", "ngày yêu cầu" },
            { "DateReturnExpected", "ngày hàng về mong đợi" },
            { "DateOrder", "ngày đặt hàng" },
            { "DateEstimate", "ngày dự kiến hàng về" },
            { "DateReturnActual", "ngày hàng về thực tế" },
            { "DateReceive", "ngày nhận hàng" },
            { "Quantity", "số lượng" },
            { "UnitPrice", "đơn giá" },
            { "TotalPrice", "thành tiền" },
            { "UnitMoney", "đơn vị tiền tệ" },
            { "Note", "ghi chú" },
            { "IsApprovedTBP", "đã duyệt TBP" },
            { "IsApprovedBGD", "đã duyệt BGD" },
            { "DateApprovedTBP", "ngày duyệt TBP" },
            { "DateApprovedBGD", "ngày duyệt BGD" },
            { "CreatedBy", "người tạo" },
            { "CreatedDate", "ngày tạo" },
            { "UpdatedBy", "người cập nhật" },
            { "UpdatedDate", "ngày cập nhật" },
            { "CurrencyRate", "tỷ giá" },
            { "HistoryPrice", "giá lịch sử" },
            { "TotalPriceExchange", "thành tiền quy đổi" },
            { "LeadTime", "lead time" },
            { "UnitFactoryExportPrice", "đơn giá xuất xưởng" },
            { "UnitImportPrice", "đơn giá nhập khẩu" },
            { "TotalImportPrice", "tổng giá nhập khẩu" },
            { "IsImport", "hàng nhập khẩu" },
            { "IsRequestApproved", "yêu cầu đã được duyệt" },
            { "ReasonCancel", "lý do hủy" },
            { "VAT", "VAT" },
            { "TotaMoneyVAT", "tổng tiền gồm VAT" },
            { "TotalDayLeadTime", "tổng số ngày lead time" },
            { "IsCommercialProduct", "hàng thương mại" },
            { "IsDeleted", "đã xóa" },
            { "IsTechBought", "kỹ thuật đã mua" },
            { "TicketType", "loại phiếu" },
            { "DateReturnEstimated", "ngày dự kiến trả" },
            { "NoteHR", "ghi chú HR" },
            { "UnitName", "tên đơn vị" },
            { "Maker", "hãng sản xuất" },
            { "TargetPrice", "giá mục tiêu" },
            { "OriginQuantity", "số lượng gốc" },
            { "ParentProductCode", "mã sản phẩm cha" },
            { "IsPurchase", "đã mua" },
            { "IsPaidLater", "đợi ghép sau" }
        };

        public static string GetDisplayName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _map.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLog(ProjectPartlistPurchaseRequest oldObj, ProjectPartlistPurchaseRequest newObj)
        {
            try
            {
                if (oldObj == null || newObj == null) return string.Empty;

                var changes = new List<string>();
                var props = typeof(ProjectPartlistPurchaseRequest).GetProperties();

                var ignoreFields = new HashSet<string> { "CreatedDate", "UpdatedDate", "CreatedBy", "UpdatedBy", "IsRequestApproved" };

                foreach (var prop in props)
                {
                    if (ignoreFields.Contains(prop.Name)) continue;

                    var oldVal = prop.GetValue(oldObj);
                    var newVal = prop.GetValue(newObj);

                    if (newVal == null) continue;

                    if (oldVal is DateTime oldDate && newVal is DateTime newDate)
                    {
                        if (oldDate.Date == newDate.Date) continue;
                    }
                    else
                    {
                        if (Equals(oldVal, newVal)) continue;
                    }

                    string fieldName = GetDisplayName(prop.Name);

                    string oldStr = FormatValue(prop.Name, oldVal);
                    string newStr = FormatValue(prop.Name, newVal);

                    changes.Add($"+ thay đổi {fieldName} từ '{oldStr}' thành '{newStr}'");
                }

                return string.Join("\n", changes);
            }
            catch (Exception)
            {
                return "";
            }
        }

        #endregion Lưu log master

        #region khác

        private string FormatValue(string fieldName, object value)
        {
            try
            {
                if (value == null) return "rỗng";
                string msg = "";

                switch (fieldName)
                {
                    case "EmployeeID":
                        int employeeID = Convert.ToInt32(value);
                        msg = _employeeRepo.GetByID(employeeID).FullName;
                        return msg;

                    case "EmployeeIDRequestApproved":
                        int EmployeeIDRequestApproved = Convert.ToInt32(value);
                        msg = _employeeRepo.GetByID(EmployeeIDRequestApproved).FullName;
                        return msg;

                    case "UnitCountID":
                        int UnitCountID = Convert.ToInt32(value);
                        msg = _unitCountRepo.GetByID(UnitCountID).UnitCode;
                        return msg;

                    case "SupplierSaleID":
                        int SupplierSaleID = Convert.ToInt32(value);
                        msg = _supplierSaleRepo.GetByID(SupplierSaleID).NameNCC;
                        return msg;

                    case "ProductSaleID":
                        int ProductSaleID = Convert.ToInt32(value);
                        msg = _productSaleRepo.GetByID(ProductSaleID).ProductCode;
                        return msg;

                    case "ProductGroupID":
                        int ProductGroupID = Convert.ToInt32(value);
                        msg = _productGroupRepo.GetByID(ProductGroupID).ProductGroupID;
                        return msg;

                    case "CurrencyID":
                        int CurrencyID = Convert.ToInt32(value);
                        msg = _currencyRepo.GetByID(CurrencyID).Code;
                        return msg;

                    case "ProductGroupRTCID":
                        int ProductGroupRTCID = Convert.ToInt32(value);
                        msg = _productGroupRTCRepo.GetByID(ProductGroupRTCID).ProductGroupNo;
                        return msg;

                    case "ProductRTCID":
                        int ProductRTCID = Convert.ToInt32(value);
                        msg = _productRTCRepo.GetByID(ProductRTCID).ProductCode;
                        return msg;

                    case "EmployeeApproveID":
                        int EmployeeApproveID = Convert.ToInt32(value);
                        msg = _employeeRepo.GetByID(EmployeeApproveID).FullName;
                        return msg;

                    case "ApprovedBGD":
                        int ApprovedBGD = Convert.ToInt32(value);
                        msg = _employeeRepo.GetByID(ApprovedBGD).FullName;
                        return msg;

                    case "ApprovedTBP":
                        int ApprovedTBP = Convert.ToInt32(value);
                        msg = _employeeRepo.GetByID(ApprovedTBP).FullName;
                        return msg;

                    case "WarehouseID":
                        int WarehouseID = Convert.ToInt32(value);
                        msg = _warehouseRepo.GetByID(WarehouseID).WarehouseCode;
                        return msg;

                    case "ProjectPartlistPurchaseRequestTypeID":
                        int ProjectPartlistPurchaseRequestTypeID = Convert.ToInt32(value);
                        msg = _projectPartlistPriceRequestTypeRepo.GetByID(ProjectPartlistPurchaseRequestTypeID).RequestTypeName;
                        return msg;

                    case "StatusRequest":
                        return Convert.ToInt32(value) switch
                        {
                            1 => "Yêu cầu mua / mượn hàng",
                            2 => "Huỷ yêu cầu mua hàng",
                            3 => "Đã đặt hàng",
                            4 => "Đang về",
                            5 => "Đã về",
                            6 => "Không đặt hàng",
                            7 => "Hoàn thành",
                            _ => string.Empty
                        };

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
            catch (Exception)
            {
                return "";
            }
        }

        public async Task AddLog(int projectPartListPurchaseRequestID, string logContent, string typeLog)
        {
            ProjectPartListPurchaseRequestLog log = new ProjectPartListPurchaseRequestLog();
            log.ProjectPartListPurchaseRequestID = projectPartListPurchaseRequestID;
            log.TypeLog = typeLog;
            log.ContentLog = logContent;
            log.CreatedBy = _currentUser.LoginName;
            log.CreatedDate = DateTime.Now;
            log.IsDeleted = false;

            await CreateAsync(log);
        }

        public async Task updateLog(ProjectPartlistPurchaseRequest oldModel, ProjectPartlistPurchaseRequest model)
        {
            try
            {
                string log = GenerateLog(oldModel, model);
                if (!String.IsNullOrWhiteSpace(log))
                {
                    AddLog(oldModel.ID, $"{_currentUser.FullName} đã cập nhật:\n{log}", "Cập nhật");
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion khác
    }
}