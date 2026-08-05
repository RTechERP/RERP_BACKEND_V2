using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartListPriceRequestLogRepo : GenericRepo<ProjectPartListPriceRequestLog>
    {
        #region Khai báo repo

        private CurrentUser _currentUser;
        private EmployeeRepo _employeeRepo;
        private CurrencyRepo _currencyRepo;

        private UnitCountRepo _unitCountRepo;

        private SupplierSaleRepo _supplierSaleRepo;
        private ProductSaleRepo _productSaleRepo;
        private ProductGroupRepo _productGroupRepo;
        private ProductGroupRTCRepo _productGroupRTCRepo;
        private ProductRTCRepo _productRTCRepo;
        private WarehouseRepo _warehouseRepo;
        private ProjectPartlistPriceRequestTypeRepo _projectPartlistPriceRequestTypeRepo;

        public ProjectPartListPriceRequestLogRepo(
            CurrentUser currentUser,
            EmployeeRepo employeeRepo,
            CurrencyRepo currencyRepo,
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
            // --- Các trường ID chuyển lên phía trên ---
            { "ID", "ID" },
            { "ProjectPartListID", "hạng mục dự án" },
            { "EmployeeID", "nhân viên phụ trách" },
            { "SupplierSaleID", "nhà cung cấp" },
            { "CurrencyID", "loại tiền tệ" },
            { "QuoteEmployeeID", "nhân viên báo giá" },
            { "POKHDetailID", "chi tiết POKH" },
            { "JobRequirementID", "yêu cầu công việc" },
            { "ProjectPartlistPriceRequestTypeID", "loại yêu cầu báo giá" },
            { "EmployeeIDUnPrice", "nhân viên hủy/không báo giá" },

            // --- Các trường thông tin khác ---
            { "ProductCode", "mã sản phẩm" },
            { "ProductName", "tên sản phẩm" },
            { "StatusRequest", "trạng thái yêu cầu" },
            { "DateRequest", "ngày yêu cầu" },
            { "Deadline", "hạn chót" },
            { "Quantity", "số lượng" },
            { "UnitPrice", "đơn giá" },
            { "TotalPrice", "thành tiền" },
            { "Unit", "đơn vị tính" },
            { "Note", "ghi chú" },
            { "CreatedBy", "người tạo" },
            { "CreatedDate", "ngày tạo" },
            { "UpdatedBy", "người cập nhật" },
            { "UpdatedDate", "ngày cập nhật" },
            { "DatePriceQuote", "ngày báo giá" },
            { "TotalPriceExchange", "thành tiền quy đổi" },
            { "CurrencyRate", "tỷ giá" },
            { "HistoryPrice", "lịch sử giá" },
            { "LeadTime", "thời gian giao hàng" },
            { "UnitFactoryExportPrice", "đơn giá xuất xưởng" },
            { "UnitImportPrice", "đơn giá nhập khẩu" },
            { "TotalImportPrice", "thành tiền nhập khẩu" },
            { "IsImport", "hàng nhập khẩu" },
            { "IsDeleted", "đã xóa" },
            { "IsCheckPrice", "check giá" },
            { "VAT", "thuế VAT" },
            { "TotaMoneyVAT", "tổng tiền gồm VAT" },
            { "TotalDayLeadTime", "tổng số ngày giao hàng" },
            { "DateExpected", "ngày dự kiến" },
            { "IsCommercialProduct", "hàng thương mại" },
            { "Maker", "hãng sản xuất" },
            { "IsJobRequirement", "yêu cầu công việc" },
            { "NoteHR", "ghi chú HR" },
            { "IsRequestBuy", "yêu cầu mua hàng" },
            { "ReasonUnPrice", "lý do không báo giá" },
            { "LeadTimeTechnical", "thời gian giao hàng kỹ thuật" },
            { "TargetPrice", "giá mục tiêu (target)" },
            { "QuoteExpectedDate", "ngày dự kiến báo giá" },
            { "EffectiveDate", "ngày có hiệu lực" }
        };

        public static string GetDisplayName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _map.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLog(ProjectPartlistPriceRequest oldObj, ProjectPartlistPriceRequest newObj)
        {
            try
            {
                if (oldObj == null || newObj == null) return string.Empty;

                var changes = new List<string>();
                var props = typeof(ProjectPartlistPriceRequest).GetProperties();

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

                    if ((oldVal == null|| String.IsNullOrWhiteSpace(oldStr)) && newVal != null)
                    {
                        changes.Add($"+ thay đổi cập nhật {fieldName} thành '{newStr}'");
                    }
                    else
                    {
                        changes.Add($"+ thay đổi {fieldName} từ '{oldStr}' thành '{newStr}'");
                    }
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

                    case "QuoteEmployeeID":
                        int QuoteEmployeeID = Convert.ToInt32(value);
                        msg = _employeeRepo.GetByID(QuoteEmployeeID).FullName;
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
                            1 => "Yêu cầu báo giá",
                            2 => "Đã báo giá",
                            3 => "Đã hoàn thành",
                            4 => "Yêu cầu check lại",
                            5 => "Từ chối báo giá",
                            6 => "Yêu cầu báo giá lại",
                            7 => "Đã báo giá lại",
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

        public async Task AddLog(int projectPartListPriceRequestID, string logContent, string typeLog)
        {
            ProjectPartListPriceRequestLog log = new ProjectPartListPriceRequestLog();
            log.ProjectPartlistPriceRequestID = projectPartListPriceRequestID;
            log.TypeLog = typeLog;
            log.ContentLog = logContent;
            log.CreatedBy = _currentUser.LoginName;
            log.CreatedDate = DateTime.Now;
            log.IsDeleted = false;

            await CreateAsync(log);
        }

        public async Task updateLog(ProjectPartlistPriceRequest oldModel, ProjectPartlistPriceRequest model)
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