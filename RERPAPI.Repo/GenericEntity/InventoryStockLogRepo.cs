using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class InventoryStockLogRepo : GenericRepo<InventoryStockLog>
    {


        #region Khai báo repo
        private CurrentUser _currentUser;
        private ProductSaleRepo _productSaleRepo;
        public InventoryStockLogRepo(
            CurrentUser currentUser, ProductSaleRepo productSaleRepo) : base(currentUser)
        {
            _productSaleRepo = productSaleRepo;
            _currentUser = currentUser;
        }


        #endregion Khai báo repo

        #region Log chính

        private static readonly Dictionary<string, string> _map = new()
        {
            { "ID", "ID" },
            { "InventoryID", "kho tồn" },
            { "Quantity", "số lượng" },
            { "CreatedBy", "Người tạo" },
            { "CreatedDate", "Ngày tạo" },
            { "UpdatedBy", "Người cập nhật" },
            { "UpdatedDate", "Ngày cập nhật" },
            { "EmployeeStock", "Người nhập tồn tối thiểu" },
            { "ProductSaleID", "Sản phẩm Sale" },
            { "WarehouseID", "Kho" },
            { "ProjectTypeID", "Loại dự án" },
            { "EmployeeIDRequest", "Người yêu cầu" },
            { "IsDeleted", "Đã xóa" },
            { "Note", "ghi chú" }
        };

        public static string GetDisplayName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _map.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLog(InventoryStock oldObj, InventoryStock newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(InventoryStock).GetProperties();

            var ignoreFields = new HashSet<string> { "Quantity", "Note" };

            foreach (var prop in props)
            {
                if (!ignoreFields.Contains(prop.Name)) continue;

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

                if (string.IsNullOrWhiteSpace(oldStr)) changes.Add($"+ thay đổi {fieldName} thành {newStr}");
                else changes.Add($"+ thay đổi {fieldName} từ {oldStr} thành {newStr}");
                
            }

            return string.Join("\n", changes);
        }

        #endregion Log chính


        #region Khác

        private string FormatValue(string fieldName, object value)
        {
            if (value == null) return "rỗng";
            string msg = "";

            switch (fieldName)
            {
                case "ProductSaleID":
                    int supplierSaleID = Convert.ToInt32(value);
                    msg = _productSaleRepo.GetByID(supplierSaleID).ProductCode;
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

        public async Task AddLog(int inventoryStockID, string logContent, string typeLog)
        {
            InventoryStockLog log = new InventoryStockLog();
            log.InventoryStockID = inventoryStockID;
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