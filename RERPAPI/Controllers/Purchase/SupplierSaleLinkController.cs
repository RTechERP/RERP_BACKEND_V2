using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Purchase
{
    public class ImportExcelPayload
    {
        public List<Dictionary<string, object>> Rows { get; set; }
    }
    /// <summary>
    /// Controller quản lý liên kết Nhà cung cấp và Nhân viên mua hàng
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierSaleLinkController : ControllerBase
    {
        private readonly SupplierSaleLinkRepo _supplierSaleLinkRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly SupplierSaleRepo _supplierSaleRepo;

        public SupplierSaleLinkController(
            SupplierSaleLinkRepo supplierSaleLinkRepo,
            EmployeeRepo employeeRepo,
            SupplierSaleRepo supplierSaleRepo)
        {
            _supplierSaleLinkRepo = supplierSaleLinkRepo;
            _employeeRepo = employeeRepo;
            _supplierSaleRepo = supplierSaleRepo;
        }

        [HttpGet("getall")]
        public IActionResult GetAll(string keyword = "", int employeePurchaseID = 0)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetSupplierSaleLink",
                    new string[] { "@EmployeePurchaseID", "@KeyWord" },
                    new object[] { employeePurchaseID, keyword ?? "" });

                var result = SQLHelper<object>.GetListData(datas, 0);
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách NCC kèm theo trạng thái đã được chọn của nhân viên
        /// </summary>
        ///
        [RequiresPermission("N33,N1")]
        [HttpGet("get-with-selection")]
        public async Task<IActionResult> GetWithSelection(int employeePurchaseID = 0, string keyword = "", int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var param = new
                {
                    EmployeePurchaseID = employeePurchaseID,
                    SearchKeyword = keyword ?? "",
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var data = await SqlDapper<SupplierSaleWithSelectionDTO>.ProcedureToListTAsync("spGetSupplierSaleWithSelection", param);

                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu danh sách liên kết (Xóa cũ, Thêm mới)
        /// </summary>
        /// <param name="items">Danh sách liên kết cần lưu</param>
        /// <returns>Trạng thái thành công</returns>
        ///
        [RequiresPermission("N33,N1")]
        [HttpPost("save")]
        public async Task<IActionResult> Save(List<SupplierSaleLink> items)
        {
            try
            {
                if (items == null) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                // Nếu danh sách rỗng, ta vẫn cần xóa hết các liên kết cũ của nhân viên đó
                // Tuy nhiên, ta cần biết EmployeePurchaseID.
                // Ở frontend, ta luôn gửi ít nhất một item hoặc ta có thể truyền EmployeePurchaseID riêng.
                // Giả sử frontend luôn gửi payload hợp lệ.

                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        if (item.MatHang?.Length > 550)
                            return BadRequest(ApiResponseFactory.Fail(null, "Mặt hàng không được vượt quá 550 ký tự"));
                        if (item.Note?.Length > 550)
                            return BadRequest(ApiResponseFactory.Fail(null, "Ghi chú không được vượt quá 550 ký tự"));
                    }

                    int employeeID = items[0].EmployeePurchaseID;

                    // Xóa các liên kết cũ của nhân viên này
                    var oldLinks = _supplierSaleLinkRepo.GetAll(x => x.EmployeePurchaseID == employeeID);
                    if (oldLinks.Count > 0)
                    {
                        _supplierSaleLinkRepo.DeleteRange(oldLinks);
                    }

                    // Thêm các liên kết mới
                    await _supplierSaleLinkRepo.CreateRangeAsync(items);
                }
                else
                {
                    // Nếu items rỗng, có thể là user đã bỏ tích hết.
                    // Nhưng frontend hiện tại chặn save nếu không chọn item nào.
                    // Nếu muốn hỗ trợ bỏ tích hết, ta cần truyền EmployeeID riêng.
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Xóa liên kết theo ID (hỗ trợ xóa nhiều, cách nhau bởi dấu phẩy)
        /// </summary>
        /// <param name="id">ID hoặc danh sách ID của liên kết cần xóa</param>
        /// <returns>Trạng thái thành công</returns>
        ///
        [RequiresPermission("N33,N1")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return BadRequest(ApiResponseFactory.Fail(null, "ID không hợp lệ"));

                var ids = id.Split(',');
                foreach (var sId in ids)
                {
                    if (int.TryParse(sId, out int intId))
                    {
                        await _supplierSaleLinkRepo.DeleteAsync(intId);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N33,N1")]
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel([FromBody] ImportExcelPayload payload)
        {
            try
            {
                if (payload == null || payload.Rows == null || payload.Rows.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }
                int created = 0;
                int updated = 0;
                int skipped = 0;
                List<string> errors = new List<string>();

                var allEmployees = _employeeRepo.GetAll();
                var allSuppliers = _supplierSaleRepo.GetAll();
                var allLinks = _supplierSaleLinkRepo.GetAll();

                var currentUsername = User.Identity?.Name ?? "system";

                int rowIndex = 0;
                foreach (var row in payload.Rows)
                {
                    rowIndex++;
                    row.TryGetValue("STT", out object sttObj);
                    row.TryGetValue("Mã NV", out object maNvObj);
                    row.TryGetValue("Mã NCC", out object maNccObj);
                    row.TryGetValue("Mặt hàng", out object matHangObj);
                    row.TryGetValue("Ghi chú", out object ghiChuObj);

                    string stt = sttObj?.ToString()?.Trim();
                    string rowLabel = !string.IsNullOrEmpty(stt) ? $"Dòng STT {stt}" : $"Dòng thứ {rowIndex}";

                    string maNv = maNvObj?.ToString()?.Trim();
                    string maNcc = maNccObj?.ToString()?.Trim();
                    string matHang = matHangObj?.ToString()?.Trim();
                    string ghiChu = ghiChuObj?.ToString()?.Trim();

                    // 1. Kiểm tra thiếu Mã NV
                    if (string.IsNullOrEmpty(maNv))
                    {
                        skipped++;
                        errors.Add($"{rowLabel}: Chưa nhập Mã nhân viên");
                        continue;
                    }

                    // 2. Kiểm tra thiếu Mã NCC
                    if (string.IsNullOrEmpty(maNcc))
                    {
                        skipped++;
                        errors.Add($"{rowLabel}: Chưa nhập Mã nhà cung cấp");
                        continue;
                    }

                    // 3. Kiểm tra thiếu Mặt hàng
                    if (string.IsNullOrEmpty(matHang))
                    {
                        skipped++;
                        errors.Add($"{rowLabel}: Chưa nhập Mặt hàng");
                        continue;
                    }

                    // 4. Kiểm tra độ dài Mặt hàng hoặc Ghi chú > 550
                    if (matHang?.Length > 550 || ghiChu?.Length > 550)
                    {
                        skipped++;
                        errors.Add($"{rowLabel}: Mặt hàng hoặc Ghi chú vượt quá 550 ký tự");
                        continue;
                    }

                    // 5. Kiểm tra Mã nhân viên không tồn tại (so sánh không phân biệt hoa thường)
                    var emp = allEmployees.FirstOrDefault(x => string.Equals(x.Code?.Trim(), maNv, StringComparison.OrdinalIgnoreCase));
                    if (emp == null)
                    {
                        skipped++;
                        errors.Add($"{rowLabel}: Mã nhân viên '{maNv}' không tồn tại");
                        continue;
                    }

                    // 6. Kiểm tra Mã nhà cung cấp không tồn tại (so sánh không phân biệt hoa thường)
                    var supplier = allSuppliers.FirstOrDefault(x => string.Equals(x.CodeNCC?.Trim(), maNcc, StringComparison.OrdinalIgnoreCase));
                    if (supplier == null)
                    {
                        skipped++;
                        errors.Add($"{rowLabel}: Mã nhà cung cấp '{maNcc}' không tồn tại");
                        continue;
                    }

                    var existLink = allLinks.FirstOrDefault(x => x.EmployeePurchaseID == emp.ID && x.SupplierSaleID == supplier.ID);

                    if (existLink != null)
                    {
                        // Update
                        existLink.MatHang = matHang;
                        existLink.Note = ghiChu;
                        existLink.UpdatedDate = DateTime.Now;
                        existLink.UpdatedBy = currentUsername;

                        await _supplierSaleLinkRepo.UpdateAsync(existLink);
                        updated++;
                    }
                    else
                    {
                        // Create
                        var newLink = new SupplierSaleLink
                        {
                            EmployeePurchaseID = emp.ID,
                            SupplierSaleID = supplier.ID,
                            MatHang = matHang,
                            Note = ghiChu,
                            CreatedDate = DateTime.Now,
                            CreatedBy = currentUsername
                        };

                        await _supplierSaleLinkRepo.CreateAsync(newLink);
                        allLinks.Add(newLink);
                        created++;
                    }
                }

                return Ok(ApiResponseFactory.Success(new
                {
                    Created = created,
                    Updated = updated,
                    Skipped = skipped,
                    Errors = errors
                }, "Import thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}