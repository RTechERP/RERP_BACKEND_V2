using Azure.Core;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistPurchaseRequestRepo:GenericRepo<ProjectPartlistPurchaseRequest>
    {
        public ProjectPartlistPurchaseRequestRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(List<ProjectPartlistPurchaseRequest> requests, out string message)
        {
            message = "";

            if (requests == null || requests.Count == 0)
            {
                message = "Danh sách yêu cầu mua hàng trống.";
                return false;
            }
            
            // 1. Validate từng request
            foreach (var request in requests)
            {
                if (request.ID > 0 && request.IsRequestApproved == true)
                {
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
                    return true;
                }
                else if (string.IsNullOrWhiteSpace(request.ProductName))
                {
                    message = "Tên sản phẩm không được để trống.";
                    return false;
                }
                if (request.EmployeeID == 0 && request.ID == 0)
                {
                    message = "Vui lòng chọn Nhân viên mua!";
                    return false;
                }
                if (request.Quantity == null || request.Quantity <= 0)
                {
                    message = $"Số lượng yêu cầu cho sản phẩm [{request.ProductCode}] phải lớn hơn 0.";
                    return false;
                }

                if (request.IsTechBought == false)
                {
                    DateTime deadline = request.DateReturnExpected ?? DateTime.MinValue;
                    DateTime dateNow = DateTime.Now;
                    if (deadline == DateTime.MinValue || deadline < dateNow)
                    {
                        message = $"Deadline của sản phẩm [{request.ProductCode}] không hợp lệ!";
                        return false;
                    }

                    double timeSpan = (deadline.Date - dateNow.Date).TotalDays + 1;
                    if (dateNow.Hour < 15)
                    {
                        if (timeSpan < 2)
                        {
                            message = $"Deadline tối thiểu cho sản phẩm [{request.ProductCode}] là 2 ngày từ hôm nay!";
                            return false;
                        }
                    }
                    else if (timeSpan < 3)
                    {
                        message = $"Deadline cho sản phẩm [{request.ProductCode}] phải cách hiện tại ít nhất 3 ngày!";
                        return false;
                    }

                    if (deadline.DayOfWeek == DayOfWeek.Sunday || deadline.DayOfWeek == DayOfWeek.Saturday)
                    {
                        message = $"Deadline [{deadline:dd/MM/yyyy}] của sản phẩm [{request.ProductCode}] rơi vào Thứ 7/CN. Vui lòng chọn ngày làm việc (T2-T6)!";
                        return false;
                    }
                }
                //else if (string.IsNullOrWhiteSpace(request.Note))
                //{
                //    message = $"Vui lòng nhập Ghi chú cho sản phẩm [{request.ProductCode}]!";
                //    return false;
                //}
            }

            // 2. Validate duplicate (DuplicateID)
            var duplicateIDs = requests
                .Where(r => r.DuplicateID.HasValue && r.DuplicateID.Value > 0)
                .Select(r => r.DuplicateID.Value)
                .Distinct()
                .ToList();

            foreach (int id in duplicateIDs)
            {
                decimal originQuantity = 0;
                bool foundOrigin = false;

                foreach (var request in requests)
                {
                    if (request.DuplicateID == id)
                    {
                        originQuantity += request.OriginQuantity ?? 0;
                        foundOrigin = true;
                        break;
                    }
                }

                if (!foundOrigin)
                {
                    message = $"Không tìm thấy Số lượng yêu cầu ban đầu của ID nhân bản mới: {id}!";
                    return false;
                }

                decimal totalQuantity = requests
                    .Where(r => r.DuplicateID == id)
                    .Sum(r => r.Quantity ?? 0);

                if (totalQuantity > originQuantity)
                {
                    var req = requests.FirstOrDefault(r => r.DuplicateID == id);
                    message = $"Tổng số lượng của sản phẩm [{req?.ProductCode}]: ({totalQuantity}) vượt quá số lượng yêu cầu ban đầu ({originQuantity})!";
                    return false;
                }
            }

            return true;
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
    }
}
