using Azure.Core;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistPurchaseRequestRepo : GenericRepo<ProjectPartlistPurchaseRequest>
    {
        CurrentUser _currentUser;
        ProductGroupRepo _productgroupRepo;
        ProductSaleRepo _productSaleRepo;
        EmployeeRepo _employeeRepo;
        EmployeeSendEmailRepo _employeeSendEmailRepo;
        public ProjectPartlistPurchaseRequestRepo(
            CurrentUser currentUser,
            ProductGroupRepo productgroupRepo,
            ProductSaleRepo productSaleRepo,
            EmployeeRepo employeeRepo,
            EmployeeSendEmailRepo employeeSendEmailRepo
        ) : base(currentUser)
        {
            _currentUser = currentUser;
            _productgroupRepo = productgroupRepo;
            _productSaleRepo = productSaleRepo;
            _employeeRepo = employeeRepo;
            _employeeSendEmailRepo = employeeSendEmailRepo;
        }

        public bool ValidateKeepProduct(List<ProductHoldDTO> requests, out string message)
        {
            message = "";
            if (requests == null || requests.Count == 0)
            {
                message = "Danh sách yêu cầu mua hàng trống.";
                return false;
            }
            foreach (var item in requests)
            {
                if (item.ProjectParlistPurchaseRequestID.Count <= 0 || item.ProductSaleID <= 0 || item.ProjectID <= 0) continue;
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetInventory", new[] { "@ProductSaleID" }, new object[] { item.ProductSaleID });
                var inventoryData = SQLHelper<dynamic>.GetListData(dt, 0);
                var quantity = inventoryData[0]?.TotalQuantityLast;
                if (quantity == null || Convert.ToDecimal(quantity) <= 0)
                {
                    message = "Số lượng tồn cuối kì phải lớn hơn 0!";
                    return false;
                }
            }
            return true;
        }

        public bool ValidateRequestApproved(List<ProjectPartlistPurchaseRequestDTO> requests, out string message)
        {
            message = "";
            if (requests.Count <= 0 || requests == null) { message = "Dữ liệu không hợp lệ"; return false; }
            foreach (ProjectPartlistPurchaseRequest request in requests)
            {
                if (request.ID <= 0) continue;
                if (request.ProductRTCID <= 0)
                {
                    if (request.SupplierSaleID <= 0)
                    {
                        message = $"Vui lòng nhập Nhà cung cấp cho sản phẩm [{request.ProductCode}]!";
                        return false;
                    }
                    if (request.UnitPrice == null || request.UnitPrice <= 0)
                    {
                        message = $"Vui lòng nhập Đơn giá cho sản phẩm [{request.ProductCode}]!";
                        return false;
                    }
                    if (request.ProductSaleID <= 0)
                    {
                        message = $"Vui lòng tạo Mã nội bộ cho sản phẩm [{request.ProductCode}]!";
                        return false;
                    }
                    if (request.CurrencyID <= 0)
                    {
                        message = $"Vui lòng chọn Loại tiền cho sản phẩm [{request.ProductCode}]!";
                        return false;
                    }
                }
            }
            return true;
        }

        public bool ValidateUpdateData(List<ProjectPartlistPurchaseRequestDTO> requests, out string message)
        {
            message = "";
            if (requests == null || requests.Count <= 0)
            {
                message = "Dữ liệu không hợp lệ.";
                return false;
            }

            // Lưu origin, total và productCode đầu tiên (hoặc gom hết cũng được)
            var dict = new Dictionary<int, (decimal origin, decimal total, string productCode)>();

            foreach (var request in requests)
            {
                int duplicateID = Convert.ToInt32(request.DuplicateID);
                if (duplicateID <= 0) continue;

                decimal quantity = request.Quantity == null ? 0 : Convert.ToDecimal(request.Quantity);
                decimal originQuantity = request.OriginQuantity == null ? 0 : Convert.ToDecimal(request.OriginQuantity);
                string productCode = request.ProductCode ?? "(Không có ProductCode)";

                if (!dict.TryGetValue(duplicateID, out var item))
                {
                    dict[duplicateID] = (originQuantity, quantity, productCode);
                }
                else
                {
                    item.total += quantity;
                    dict[duplicateID] = (item.origin, item.total, item.productCode); // giữ productCode đầu tiên
                }
            }

            // Kiểm tra lỗi
            var invalid = dict.FirstOrDefault(kv => kv.Value.origin != kv.Value.total);

            if (invalid.Key != 0)
            {
                message =
                    $"Tổng số lượng sản phẩm [{invalid.Value.productCode}] ({invalid.Value.total}) không khớp với số lượng ban đầu ({invalid.Value.origin})!";

                return false;
            }

            return true;
        }


        public bool validateManufacturer(List<ProjectPartlistPurchaseRequestDTO> requests, out string message)
        {
            //check validate bắt buộc có hãng khi tạo mã sp kho vision
            message = "";
            if (requests.Count <= 0 || requests == null) { message = "Dữ liệu không hợp lệ"; return false; }
            foreach (var item in requests)
            {
                if (item.ProductSaleID <= 0)
                {
                    if (string.IsNullOrEmpty(item.Manufacturer) && item.ProductGroupID == 4)
                    {
                        message = $"Yêu cầu mua hàng kho vision có mã sản phẩm {item.ProductCode} ở vị trí {item.TT} phải có hãng!";
                        return false;
                    }
                }
            }

            return true;
        }

        public bool validateDeleted(List<ProjectPartlistPurchaseRequestDTO> requests, bool isPurchaseRequestDemo, out string message)
        {
            message = "";
            foreach (var item in requests)
            {
                if (item.ID <= 0) continue;

                bool isCommercialProduct = Convert.ToBoolean(item.IsCommercialProduct);
                int poNCC = Convert.ToInt32(item.PONCCID);
                string productCode = Convert.ToString(item.ProductCode);

                if (!isCommercialProduct)
                {
                    message = $"Sản phẩm mã [{productCode}] không phải hàng thương mại.\nBạn không thể xoá!";
                    return false;
                }

                if (poNCC > 0)
                {
                    message = $"Sản phẩm mã [{productCode}] đã có PO Nhà cung cấp.\nBạn không thể xoá!";
                    return false;
                }

                if (isPurchaseRequestDemo)
                {
                    string updateName = Convert.ToString(item.UpdatedName);
                    int requestStatus = Convert.ToInt32(item.StatusRequest);
                    bool isApprovedTBP = Convert.ToBoolean(item.IsApprovedTBP);
                    bool isApprovedBGD = Convert.ToBoolean(item.IsApprovedTBP);

                    if (updateName != "" && requestStatus != 1)
                    {
                        message = $"Sản phẩm mã [{productCode}] đã nhân viên mua.\nBạn không thể hủy yêu cầu!";
                        return false;
                    }

                    if (isApprovedTBP)
                    {
                        message = $"Sản phẩm mã [{productCode}] đã được TBP duyệt.\nBạn không thể hủy yêu cầu!";
                        return false;
                    }

                    if (isApprovedBGD)
                    {
                        message = $"Sản phẩm mã [{productCode}] đã được BGD duyệt.\nBạn không thể hủy yêu cầu!";
                        return false;
                    }
                }
            }

            return true;
        }

        public ProjectPartlistPurchaseRequestDTO UpdateData(ProjectPartlistPurchaseRequestDTO item)
        {
            decimal quantity = Convert.ToDecimal(item.Quantity);
            decimal unitPrice = Convert.ToDecimal(item.UnitPrice);
            decimal totalPrice = quantity * unitPrice;

            decimal currencyRate = Convert.ToDecimal(item.CurrencyRate);
            decimal totalPriceExchange = totalPrice * currencyRate;

            decimal vat = Convert.ToDecimal(item.VAT);
            decimal totalMoneyVAT = totalPrice + ((totalPrice * vat) / 100);

            decimal targetPrice = Convert.ToDecimal(item.TargetPrice);
            int duplicateID = Convert.ToInt32(item.DuplicateID);
            decimal originQuantity = Convert.ToDecimal(item.OriginQuantity);

            item.TotalPrice = totalPrice;
            item.TotalPriceExchange = totalPriceExchange;
            item.TotaMoneyVAT = totalMoneyVAT;

            return item;
        }

        public string GenerateProductNewCode(int productGroupId)
        {
            string newCodeRTC = "";
            if (productGroupId <= 0) return newCodeRTC;

            var ds = SQLHelper<object>.ProcedureToList("spLoadNewCodeRTC", new string[] { "@Group" }, new object[] { productGroupId });
            var ds0 = SQLHelper<object>.GetListData(ds, 0);
            var ds1 = SQLHelper<object>.GetListData(ds, 1);
            string code = "";
            string codeRTC = ds1.Count() > 0 ? ds1[0].ProductGroupID : "";

            if (ds0.Count() == 0)
            {
                newCodeRTC = codeRTC + "000000001";
            }
            else
            {
                if (!codeRTC.Contains("HCM"))
                {
                    code = (string)(ds0[0].ProductNewCode).Replace(codeRTC, "");
                    int stt = Convert.ToInt32(code) + 1;
                    for (int i = 0; codeRTC.Length < (9 - stt.ToString().Length); i++)
                    {
                        codeRTC = codeRTC + "0";
                    }
                    newCodeRTC = codeRTC + stt.ToString();
                }
                else
                {
                    code = (string)(ds0[0].ProductNewCode).Replace(codeRTC, "");
                    int stt = Convert.ToInt32(code) + 1;
                    string indexString = Convert.ToString(stt);
                    for (int i = 0; indexString.Length < code.Length; i++)
                    {
                        indexString = "0" + indexString;
                    }
                    newCodeRTC = codeRTC + indexString.ToString();
                }
            }

            return newCodeRTC;
        }

        public bool ValidateSaveDataDetail(ProjectPartlistPurchaseRequestDTO request, out string message)
        {
            message = "";

            if (request == null)
            {
                message = "Dữ liệu không hợp lệ";
                return false;
            }

            // Kiểm tra Tên sản phẩm
            if (string.IsNullOrWhiteSpace(request.ProductName))
            {
                message = "Vui lòng nhập Tên sản phẩm!";
                return false;
            }

            // Kiểm tra Nhân viên mua
            if (request.EmployeeBuyID == 0 && request.ID == 0)
            {
                message = "Vui lòng chọn Nhân viên mua!";
                return false;
            }

            // Kiểm tra Số lượng
            if (request.Quantity <= 0)
            {
                message = "Vui lòng nhập Số lượng!";
                return false;
            }

            // Kiểm tra Deadline
            if (!Convert.ToBoolean(request.IsTechBought))
            {
                DateTime deadline = (DateTime)request.DateReturnExpected;
                DateTime dateNow = DateTime.Now;

                double timeSpan = (deadline.Date - dateNow.Date).TotalDays + 1;

                if (dateNow.Hour < 15)
                {
                    if (timeSpan < 2)
                    {
                        message = "Deadline tối thiểu là 2 ngày từ ngày hiện tại!";
                        return false;
                    }
                }
                else if (timeSpan < 3)
                {
                    message = "Yêu cầu từ sau 15h nên ngày Deadline sẽ bắt đầu tính từ ngày hôm sau và tối thiểu là 2 ngày!";
                    return false;
                }

                if (deadline.DayOfWeek == DayOfWeek.Sunday || deadline.DayOfWeek == DayOfWeek.Saturday)
                {
                    message = "Deadline phải là ngày làm việc (T2 - T6)!";
                    return false;
                }
            }

            // Kiểm tra Ghi chú nếu là TechBought
            if ((bool)request.IsTechBought && string.IsNullOrWhiteSpace(request.Note))
            {
                message = "Vui lòng chọn Ghi chú!";
                return false;
            }

            return true;
        }

        public async Task SendMail(ProjectPartlistPurchaseRequestDTO requestBuy)
        {
            if (requestBuy.ID <= 0) return;
            EmployeeSendEmail sendEmail = new EmployeeSendEmail();

            Employee employee = _employeeRepo.GetByID((int)requestBuy.EmployeeIDRequestApproved);

            sendEmail.Subject = $"YÊU CẦU MUA HÀNG - {_currentUser.FullName.ToUpper()} - {DateTime.Now.ToString("dd/MM/yyyy")}";
            sendEmail.EmailTo = $"{employee.EmailCongTy}";
            sendEmail.EmailCC = $"";
            sendEmail.Body = $@"<div> <p style=""font-weight: bold; color: red;"">[NO REPLY]</p> <p> Dear anh/chị {employee.FullName} </p ></div >
                        <div style = ""margin-top: 30px;"">
                        <p> Cho em yêu cầu mua hàng thông tin sản phẩm như sau: </p>
                        <p> Mã sản phẩm: {requestBuy.ProductCode}</p>
                        <p> Tên sản phẩm: {requestBuy.ProductName}</p>
                        <p> Số lượng: {requestBuy.Quantity}</p>
                        <p> Deadline: {requestBuy.DateReturnExpected}</p>
                        </div>
                        <div style = ""margin-top: 30px;"">
                        <p> Thanks </p>
                        <p> {_currentUser.FullName}</p>
                        </div>";

            sendEmail.StatusSend = 1;
            sendEmail.EmployeeID = requestBuy.JobRequirementEmployeeID;
            sendEmail.Receiver = requestBuy.JobRequirementApprovedTBPID;

            await _employeeSendEmailRepo.CreateAsync(sendEmail);
        }
    }
}
