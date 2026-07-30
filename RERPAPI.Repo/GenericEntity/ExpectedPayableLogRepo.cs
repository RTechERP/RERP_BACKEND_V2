using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ExpectedPayableLogRepo : GenericRepo<ExpectedPayableLog>
    {
        private CurrentUser _currentUser;
        private CustomerRepo _customerRepo;
        private ProjectRepo _projectRepo;
        private UserRepo _userRepo;
        private CurrencyRepo _currencyRepo;
        private CustomerPartsRepo _customerPartsRepo;
        private ProductSaleRepo _productSaleRepo;
        private SupplierSaleRepo _supplierSaleRepo;
        private EmployeeRepo _employeeRepo;

        public ExpectedPayableLogRepo(
            CurrentUser currentUser,
            CustomerRepo customerRepo,
            ProjectRepo projectRepo,
            UserRepo userRepo,
            CurrencyRepo currencyRepo,
            CustomerPartsRepo customerPartsRepo,
            ProductSaleRepo productSaleRepo,
            SupplierSaleRepo supplierSaleRepo,
            EmployeeRepo employeeRepo
            ) : base(currentUser)
        {
            _currentUser = currentUser;
            _customerRepo = customerRepo;
            _projectRepo = projectRepo;
            _userRepo = userRepo;
            _currencyRepo = currencyRepo;
            _customerPartsRepo = customerPartsRepo;
            _productSaleRepo = productSaleRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _employeeRepo = employeeRepo;
        }

        private static readonly Dictionary<string, string> _map = new()
        {
            { "BillImportDetailID", "chi tiết phiếu nhập" },
            { "SupplierSaleID", "nhà cung cấp" },
            { "CurrencyID", "loại tiền" },
            { "DeliverID", "nhân viên mua/người giao" },
            { "InvoiceNumber", "số hóa đơn" },
            { "InvoiceDate", "ngày hóa đơn" },
            { "DueDate", "ngày tới hạn" },
            { "UnitPrice", "đơn giá" },
            { "DomesticPayable", "công nợ trong nước" },
            { "ForeignPayable", "công nợ nước ngoài" },
            { "ArisingAmount", "tiền hàng phát sinh" },
            { "OfficeExpense", "chi phí văn phòng" },
            { "TaxAmount", "thuế" },
            { "Note", "ghi chú" },
            { "IsDeleted", "trạng thái xóa" }
        };

        public static string GetDisplayName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _map.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLog(ExpectedPayable? oldObj, ExpectedPayable? newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(ExpectedPayable).GetProperties();

            var ignoreFields = new HashSet<string> { "CreatedDate", "UpdatedDate", "IsDeleted" };

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

        private string FormatValueMaster(string fieldName, object value)
        {
            if (value == null) return "rỗng";
            string msg = "";

            switch (fieldName)
            {
                case "SupplierSaleID":
                    int supplierSaleID = Convert.ToInt32(value);
                    msg = _supplierSaleRepo.GetByID(supplierSaleID).CodeNCC;
                    return msg;

                case "CurrencyID":
                    int currencyID = Convert.ToInt32(value);
                    msg = _currencyRepo.GetByID(currencyID).Code;
                    return msg;

                case "DeliverID":
                    int userID = Convert.ToInt32(value);
                    msg = _employeeRepo.GetByID(userID).FullName;
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

        public async Task AddLog(int expectedPayableID, string logContent, string typeLog)
        {
            ExpectedPayableLog log = new ExpectedPayableLog();
            log.ExpectedPayableID = expectedPayableID;
            log.TypeLog = typeLog;
            log.LogContent = logContent;
            log.CreatedBy = _currentUser.LoginName;
            log.CreatedDate = DateTime.Now;
            log.IsDeleted = false;

            await CreateAsync(log);
        }
    }
}