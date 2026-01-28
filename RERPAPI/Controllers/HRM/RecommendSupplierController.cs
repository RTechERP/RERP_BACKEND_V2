using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Handover;
using RERPAPI.Model.Param.HRM.DepartmentRequired;
using RERPAPI.Repo.GenericEntity.BBNV;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.JobRequirements;
using RERPAPI.Repo.GenericEntity.HRM.DepartmentRequire;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecommendSupplierController : ControllerBase
    {
        JobRequirementRepo _jobRepo;

        DepartmentRequiredRepo _departmentrequired;

        //DepartmentRequiredApprovalsRepo _departmentrequiredapprovals;

        HCNSProposalsRepo _hcnsproposals;

        private IConfiguration _configuration;

        public RecommendSupplierController(JobRequirementRepo jobRepo, DepartmentRequiredRepo departmentrequired, HCNSProposalsRepo hcnsproposals, IConfiguration configuration)
        {
            _jobRepo = jobRepo;
            _departmentrequired = departmentrequired;
            //_departmentrequiredapprovals = departmentrequiredapprovals;
            _hcnsproposals = hcnsproposals;
            _configuration = configuration;
        }

        [HttpPost("get-department-required-data")]
        public IActionResult GetHandoverData([FromBody] DepartmentRequiredRequestParam request)
        {
            try
            {
                var departmentRequired = SQLHelper<dynamic>.ProcedureToList("spGetDepartmentRequired",
                new string[] { "@JobRequirementID", "@EmployeeID", "@DepartmentID", "@Keyword", "@DateStart", "@DateEnd" },
                new object[] { request.JobRequirementID, request.EmployeeID, request.DepartmentID, request.Keyword, request.DateStart, request.DateEnd }
                );

                var HCNSProPosal = SQLHelper<dynamic>.ProcedureToList("spGetHCNSProposal",
                    new string[] { "@JobRequirementID", "@DepartmentRequiredID", "@DateStart", "@DateEnd" },
                    new object[] { request.JobRequirementID, request.DepartmentRequiredID ,request.DateStart, request.DateEnd }
                );

                var departmentRequiredData = SQLHelper<dynamic>.GetListData(departmentRequired, 0);
                var HCNSProPosalData = SQLHelper<dynamic>.GetListData(HCNSProPosal, 0);

                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        departmentRequiredData,
                        HCNSProPosalData,
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data-department-required")]
        public async Task<IActionResult> SaveData([FromBody] RecommendSupplierDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                //if (dto == null || dto.JobRequirementID == null)
                //{
                //    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                //}

                int JobRequirementID = dto.JobRequirementID;
                int DepartmentRequiredID = 0;
        

                // Phòng ban yêu cầu
                if (dto.DepartmentRequired != null)
                {
                    var maxSttReceiver = _departmentrequired.GetAll()
                        .Where(x => x.JobRequirementID == JobRequirementID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttEmployeeCounter = maxSttReceiver;

                    foreach (var employeeRequired in dto.DepartmentRequired)
                    {

                        employeeRequired.JobRequirementID = JobRequirementID;

                        var existing = _departmentrequired.GetAll()
                   .FirstOrDefault(x => x.JobRequirementID == JobRequirementID && x.ID == employeeRequired.ID);

                        if (existing == null || employeeRequired.ID <= 0)
                        {
                            sttEmployeeCounter++;
                            employeeRequired.STT = sttEmployeeCounter;
                            await _departmentrequired.CreateAsync(employeeRequired);
                            DepartmentRequiredID = employeeRequired.ID;

                        }

                        else
                            _departmentrequired.Update(employeeRequired);
                        DepartmentRequiredID = employeeRequired.ID;


                    }
                }

                // HCNS đề xuất
                if (dto.HCNSProposal != null && dto.HCNSProposal.Any())
                {
                    var maxSttProposal = _hcnsproposals.GetAll()
                        .Where(x => x.JobRequirementID == JobRequirementID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttHCNSProposal = maxSttProposal;
                    foreach (var itemProposal in dto.HCNSProposal)
                    {

                        itemProposal.JobRequirementID = JobRequirementID;
                       
                        var existing = _hcnsproposals.GetAll()
                .FirstOrDefault(x => x.JobRequirementID == JobRequirementID && x.ID == itemProposal.ID);

                        if (existing == null || itemProposal.ID <= 0)
                        {
                            sttHCNSProposal++;
                            itemProposal.STT = sttHCNSProposal;
                            itemProposal.CreatedDate = DateTime.Now;
                            itemProposal.DepartmentRequiredID = DepartmentRequiredID;
                            await _hcnsproposals.CreateAsync(itemProposal);
                        }

                        else
                            itemProposal.DepartmentRequiredID = existing.DepartmentRequiredID;
                        _hcnsproposals.Update(itemProposal);
     
                    }
                }
                if (dto.DeletedCommend.Count > 0)
                {
                    foreach (var item in dto.DeletedCommend)
                    {
                        HCNSProposal model = _hcnsproposals.GetByID(item);
                        model.IsDeleted = true;
                        await _hcnsproposals.UpdateAsync(model);
                    }
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công",
                    id = JobRequirementID,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("export-excel")]
        public IActionResult ExportExcel([FromBody] DepartmentRequiredRequestParam request)
        {
            try
            {
                var templateFolder = _configuration.GetValue<string>("PathTemplate");
                if (string.IsNullOrWhiteSpace(templateFolder))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy đường dẫn template trên server!"));

                string templatePath = Path.Combine(templateFolder, "ExportExcel", "TemplateRecommendJobRequirement.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy File mẫu RecommendSupplierTemplate.xlsx!"));

                // Lấy dữ liệu
                var departmentRequired = SQLHelper<dynamic>.ProcedureToList("spGetDepartmentRequired",
                    new string[] { "@JobRequirementID", "@EmployeeID", "@DepartmentID", "@Keyword", "@DateStart", "@DateEnd" },
                    new object[] { request.JobRequirementID, request.EmployeeID, request.DepartmentID, request.Keyword, request.DateStart, request.DateEnd }
                );

                var HCNSProPosal = SQLHelper<dynamic>.ProcedureToList("spGetHCNSProposal",
                    new string[] { "@JobRequirementID", "@DepartmentRequiredID", "@DateStart", "@DateEnd" },
                    new object[] { request.JobRequirementID, request.DepartmentRequiredID, request.DateStart, request.DateEnd }
                );

                var masterData = SQLHelper<dynamic>.GetListData(departmentRequired, 0);
                var detailData = SQLHelper<dynamic>.GetListData(HCNSProPosal, 0);

                // Nếu có DepartmentRequiredID thì lọc chỉ lấy đúng bản ghi đó
                if (request.DepartmentRequiredID > 0)
                {
                    masterData = masterData.Where(x => x.ID == request.DepartmentRequiredID).ToList();
                    detailData = detailData.Where(x => x.DepartmentRequiredID == request.DepartmentRequiredID).ToList();
                }

                if (!masterData.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xuất!"));

                    using (var workbook = new ClosedXML.Excel.XLWorkbook(templatePath))
                    {
                        var sheet = workbook.Worksheet(1);
                        int startRow = 4;
                        int initialPlaceholders = 3;

                        // Điền Số yêu cầu vào Header (Tìm cell chứa chuỗi "Số yêu cầu")
                        var firstMaster = masterData.FirstOrDefault();
                        if (firstMaster != null)
                        {
                            var fm = (IDictionary<string, object>)firstMaster;
                            string requestNumber = GetVal(fm, "NumberRequest");
                            
                            var targetCell = sheet.CellsUsed(c => c.Address.RowNumber <= 10)
                                                  .FirstOrDefault(c => c.Value.ToString().Contains("Số yêu cầu"));
                            if (targetCell != null)
                            {
                                string existingVal = targetCell.Value.ToString();
                                // Nếu trong cell đã có text "Số yêu cầu:", ta chỉ thay thế phần sau dấu :
                                if (existingVal.Contains("Số yêu cầu:"))
                                {
                                    int colonIndex = existingVal.IndexOf("Số yêu cầu:");
                                    string prefix = existingVal.Substring(0, colonIndex + "Số yêu cầu:".Length);
                                    targetCell.Value = $"{prefix} {requestNumber}";
                                }
                                else
                                {
                                    // Ngược lại thì append vào cuối
                                    targetCell.Value = $"{existingVal} {requestNumber}";
                                }
                            }
                            else
                            {
                                // Nếu không tìm thấy cell có sẵn, điền tạm vào dòng 2
                                sheet.Cell(2, 1).Value = $"Số yêu cầu: {requestNumber}";
                            }
                        }

                        // Tính tổng số dòng cần thiết
                        int totalRowsNeeded = 0;
                        foreach (var master in masterData)
                        {
                            var prods = detailData.Where(d => d.DepartmentRequiredID == master.ID).ToList();
                            if (!prods.Any())
                            {
                                totalRowsNeeded += 1;
                            }
                            else
                            {
                                totalRowsNeeded += prods.Count;
                            }
                        }

                        // Xóa các merge cũ trong vùng template để tránh lỗi khi merge lại theo data thực tế
                        sheet.Range(startRow, 1, startRow + initialPlaceholders - 1, 20).Unmerge();

                        // Điều chỉnh số dòng template
                        if (totalRowsNeeded > initialPlaceholders)
                        {
                            // Chèn thêm dòng sau dòng cuối cùng của placeholder (dòng 6)
                            sheet.Row(startRow + initialPlaceholders - 1).InsertRowsBelow(totalRowsNeeded - initialPlaceholders);
                        }
                        else if (totalRowsNeeded < initialPlaceholders && totalRowsNeeded > 0)
                        {
                            // Xóa các dòng thừa
                            sheet.Rows(startRow + totalRowsNeeded, startRow + initialPlaceholders - 1).Delete();
                        }

                        int currentRow = startRow;
                        int stt = 1;

                        foreach (var master in masterData)
                        {
                            var productGroups = detailData.Where(d => d.DepartmentRequiredID == master.ID).GroupBy(d => d.ProductName).ToList();
                            int currentStt = stt++;

                            if (!productGroups.Any())
                            {
                                // Nếu không có detail, vẫn xuất dòng master
                                FillMasterRow(sheet, currentRow, master, currentStt, "");
                                currentRow++;
                                continue;
                            }

                            foreach (var group in productGroups)
                            {
                                string productName = group.Key?.ToString() ?? "";
                                var suppliers = group.ToList();
                                int supplierCount = suppliers.Count;

                                // Merge master cells A-K if there are multiple suppliers for one product
                                if (supplierCount > 1)
                                {
                                    for (int col = 1; col <= 11; col++) // A to K
                                    {
                                        sheet.Range(currentRow, col, currentRow + supplierCount - 1, col).Merge();
                                    }
                                    // Tên sản phẩm column L
                                    sheet.Range(currentRow, 12, currentRow + supplierCount - 1, 12).Merge();
                                }

                                // Fill Master Info (A-K)
                                FillMasterRow(sheet, currentRow, master, currentStt, productName);

                                // Fill Suppliers (M-T)
                                foreach (var supplier in suppliers)
                                {
                                    var sRow = (IDictionary<string, object>)supplier;
                                    sheet.Cell(currentRow, 13).Value = GetVal(sRow, "Supplier"); // M
                                    sheet.Cell(currentRow, 14).Value = GetVal(sRow, "Contact");  // N

                                    // Đơn giá (O)
                                    string unitPriceStr = GetVal(sRow, "UnitPrice");
                                    if (double.TryParse(unitPriceStr, out double unitPrice))
                                        sheet.Cell(currentRow, 15).SetValue(unitPrice);
                                    else
                                        sheet.Cell(currentRow, 15).Value = unitPriceStr;

                                    // Thành tiền (P)
                                    string totalAmountStr = GetVal(sRow, "TotalAmount");
                                    if (double.TryParse(totalAmountStr, out double totalAmount))
                                        sheet.Cell(currentRow, 16).SetValue(totalAmount);
                                    else
                                        sheet.Cell(currentRow, 16).Value = totalAmountStr;

                                    sheet.Cell(currentRow, 17).Value = GetVal(sRow, "Note"); // Q
                                    
                                    string isApproved = GetVal(sRow, "IsApproved");
                                    var cellApprove = sheet.Cell(currentRow, 18); // R: Duyệt
                                    var cellDisapprove = sheet.Cell(currentRow, 19); // S: Không duyệt

                                    if (isApproved == "1") 
                                    {
                                        cellApprove.Value = "v";
                                        cellApprove.Style.Font.FontColor = XLColor.Green;
                                        cellApprove.Style.Font.Bold = true;
                                    }
                                    else if (isApproved == "2") 
                                    {
                                        cellDisapprove.Value = "v";
                                        cellDisapprove.Style.Font.FontColor = XLColor.Red;
                                        cellDisapprove.Style.Font.Bold = true;
                                    }

                                    cellApprove.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    cellDisapprove.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    
                                    sheet.Cell(currentRow, 20).Value = GetVal(sRow, "DisapprovalReason"); // T

                                    sheet.Row(currentRow).Style.Alignment.WrapText = true;
                                    sheet.Row(currentRow).Style.Alignment.Vertical = ClosedXML.Excel.XLAlignmentVerticalValues.Center;
                                    currentRow++;
                                }
                            }
                        }

                    // Apply borders
                    var dataRange = sheet.Range(startRow, 1, currentRow - 1, 20);
                    dataRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DeXuatMuaHang.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private void FillMasterRow(ClosedXML.Excel.IXLWorksheet sheet, int row, dynamic master, int stt, string productName)
        {
            var m = (IDictionary<string, object>)master;
            sheet.Cell(row, 1).Value = stt; // A
            sheet.Cell(row, 2).Value = GetVal(m, "EmployeeName"); // B
            sheet.Cell(row, 3).Value = GetVal(m, "ChucVu"); // C: Vị trí
            sheet.Cell(row, 4).Value = GetVal(m, "EmployeeDepartment"); // D
            sheet.Cell(row, 5).Value = FormatDate(GetVal(m, "DateRequest")); // E
            sheet.Cell(row, 6).Value = GetVal(m, "RequestContent"); // F
            sheet.Cell(row, 7).Value = GetVal(m, "Reason"); // G
            sheet.Cell(row, 8).Value = GetVal(m, "Unit"); // H
            sheet.Cell(row, 9).Value = GetVal(m, "Quantity"); // I
            sheet.Cell(row, 10).Value = FormatDate(GetVal(m, "DeadlineRequest")); // J
            sheet.Cell(row, 11).Value = GetVal(m, "Note"); // K (Link/Mô tả)
            sheet.Cell(row, 12).Value = productName; // L (Tên sản phẩm)
        }

        private string GetVal(IDictionary<string, object> dict, string key)
        {
            if (dict.ContainsKey(key) && dict[key] != null)
                return dict[key].ToString();
            return "";
        }

        private string FormatDate(object date)
        {
            if (date == null) return "";
            if (DateTime.TryParse(date.ToString(), out DateTime dt))
                return dt.ToString("dd/MM/yyyy");
            return date.ToString();
        }

    }
}
