using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class RequestInvoiceLogRepo : GenericRepo<RequestInvoiceLog>
    {
        private CurrentUser _currentUser;
        private CustomerRepo _customerRepo;
        private TaxCompanyRepo _taxCompanyRepo;
        private EmployeeRepo _employeeRepo;
        private ProductSaleRepo _productSaleRepo;
        public RequestInvoiceLogRepo(
            CurrentUser currentUser,
            CustomerRepo customerRepo,
            TaxCompanyRepo taxCompanyRepo,
            EmployeeRepo employeeRepo,
            ProductSaleRepo productSaleRepo
            ) : base(currentUser)
        {
            _currentUser = currentUser;
            _customerRepo = customerRepo;
            _taxCompanyRepo = taxCompanyRepo;
            _employeeRepo = employeeRepo;
            _productSaleRepo = productSaleRepo;
        }

        #region Log chính
        private static readonly Dictionary<string, string> _map = new()
        {
            { "Code", "mã" },
            { "DateRequest", "ngày yêu cầu" },
            { "CustomerID", "khách hàng" },
            { "TaxCompanyID", "công ty" },
            { "EmployeeRequestID", "người yêu cầu" },
            { "ReceriverID", "người nhận" },
            { "Status", "trạng thái" },
            { "Note", "ghi chú" },
            { "DealineUrgency", "thời gian gấp" },
            { "AmendReason", "lý do sửa" },
            { "IsCustomsDeclared", "khai báo hải quan" },
            { "CreatedBy", "người tạo" },
            { "CreatedDate", "ngày tạo" },
            { "UpdatedBy", "người cập nhật" },
            { "UpdatedDate", "ngày cập nhật" },
            { "IsUrgency", "cần gấp" },
        };

        public static string GetDisplayName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _map.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLog(RequestInvoice oldObj, RequestInvoice newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(RequestInvoice).GetProperties();

            var ignoreFields = new HashSet<string> { "CreatedDate", "UpdatedDate", "IsDeleted", "UpdatedBy", "CreatedBy", "ProductByProject" };


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
        #endregion

        #region Log chi tiết
        private static readonly Dictionary<string, string> _mapRequestInvoiceDetail = new()
        {
            { "ID", "ID" },
            { "STT", "số thứ tự" },
            { "RequestInvoiceID", "phiếu yêu cầu hóa đơn" },
            { "ProductSaleID", "sản phẩm" },
            { "ProductByProject", "sản phẩm theo dự án" },
            { "Quantity", "số lượng" },
            { "ProjectID", "dự án" },
            { "POKHDetailID", "chi tiết POKH" },
            { "Specifications", "quy cách" },
            { "InvoiceNumber", "số hóa đơn" },
            { "InvoiceDate", "ngày hóa đơn" },
            { "CreatedBy", "người tạo" },
            { "CreatedDate", "ngày tạo" },
            { "UpdatedBy", "người cập nhật" },
            { "UpdatedDate", "ngày cập nhật" },
            { "Note", "ghi chú" },
            { "IsStock", "hàng tồn kho" },
            { "IsDeleted", "đã xóa" }
        };

        public static string GetDisplayNameDetail(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _mapRequestInvoiceDetail.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLogDetail(RequestInvoiceDetail oldObj, RequestInvoiceDetail newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(RequestInvoiceDetail).GetProperties();

            var ignoreFields = new HashSet<string> { "CreatedDate", "UpdatedDate", "IsDeleted", "UpdatedBy", "CreatedBy", "ProductByProject" };

            var project = _productSaleRepo.GetByID((int)oldObj.ProductSaleID);

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

                string oldStr = FormatValueMaster(prop.Name, oldVal);
                string newStr = FormatValueMaster(prop.Name, newVal);

                changes.Add($"+ thay đổi {fieldName} từ '{oldStr}' thành '{newStr}' \\n");
            }

            if (changes.Count() > 0)
            {
                changes.Insert(0, $"{project.ProductName}:");
            }

            return string.Join("\n", changes);
        }


        #endregion

        #region Log file


        #endregion

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
                case "TaxCompanyID":
                    int KHID = Convert.ToInt32(value);
                    msg = _taxCompanyRepo.GetByID(KHID).Name;
                    return msg;
                case "EmployeeRequestID":
                    int userID = Convert.ToInt32(value);
                    msg = _employeeRepo.GetByID(userID).FullName;
                    return msg;
                case "ReceriverID":
                    int receriverID = Convert.ToInt32(value);
                    msg = _employeeRepo.GetByID(receriverID).FullName;
                    return msg;
                case "ProductSaleID":
                    int productSaleID = Convert.ToInt32(value);
                    msg = _productSaleRepo.GetByID(productSaleID).ProductName;
                    return msg;
                case "Status":
                    int Status = Convert.ToInt32(value);
                    if (Status == 1) msg = "Yêu cầu xuất hóa đơn";
                    if (Status == 2) msg = "Đã xuất nháp";
                    if (Status == 3) msg = "Đã phát hành hóa đơn";
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

        public async Task AddLog(int requestInvoiceID, string logContent, string typeLog)
        {
            RequestInvoiceLog log = new RequestInvoiceLog();
            log.RequestInvoiceID = requestInvoiceID;
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
