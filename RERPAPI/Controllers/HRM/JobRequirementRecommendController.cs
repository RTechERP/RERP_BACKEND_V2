using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Param.HRM;
using RERPAPI.Repo.GenericEntity.HRM;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobRequirementRecommendController : ControllerBase
    {
        private readonly JobRequirementRecommendRepo _masterRepo;
        private readonly JobRequirementRecommendDetailRepo _detailRepo;
        private IConfiguration _configuration;

        public JobRequirementRecommendController(
            JobRequirementRecommendRepo masterRepo,
            JobRequirementRecommendDetailRepo detailRepo,
            IConfiguration configuration)
        {
            _masterRepo = masterRepo;
            _detailRepo = detailRepo;
            _configuration = configuration;
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpPost("get-all")]
        public IActionResult GetAll([FromBody] JobRequirementRecommendParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = SQLHelper<object>.ProcedureToList("spGetJobRequirementRecommend",
                    new string[] { "@StartDate", "@EndDate", "@Keyword", "@EmployeeID" },
                    new object[] { param.DateStart, param.DateEnd, param.Keyword ?? "", param.EmployeeID ?? 0 });

                var list = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(list));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetJobRequirementRecommendByID", new string[] { "@ID" }, new object[] { id });
                var master = SQLHelper<object>.GetListData(data, 0).FirstOrDefault();
                var details = SQLHelper<object>.GetListData(data, 1);

                return Ok(ApiResponseFactory.Success(new { master, details }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpGet("init-recommend/{jobRequirementID}")]
        public IActionResult InitRecommend(int jobRequirementID)
        {
            try
            {
                var data = SQLHelper<dynamic>.ProcedureToList("spGetJobRequirementForRecommend", new string[] { "@JobRequirementID" }, new object[] { jobRequirementID });
                var master = SQLHelper<object>.GetListData(data, 0).FirstOrDefault();
                var initialDetails = SQLHelper<object>.GetListData(data, 1);
                var historicalData = SQLHelper<object>.GetListData(data, 2);

                return Ok(ApiResponseFactory.Success(new { master, initialDetails, historicalData }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-historical-suppliers")]
        public IActionResult GetHistoricalSuppliers()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetJobRequirementRecommendHistorical", new string[] { }, new object[] { });
                var historicalData = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(historicalData));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] JobRequirementRecommendDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if (dto.Master.ID > 0)
                {
                    await _masterRepo.UpdateAsync(dto.Master);
                }
                else
                {
                    await _masterRepo.CreateAsync(dto.Master);
                }

                // Delete details
                if (dto.DeletedDetailIDs != null && dto.DeletedDetailIDs.Any())
                {
                    foreach (var id in dto.DeletedDetailIDs)
                    {
                        var detail = _detailRepo.GetByID(id);
                        if (detail != null)
                        {
                            detail.IsDeleted = true;
                            await _detailRepo.UpdateAsync(detail);
                        }
                    }
                }

                // Save details
                foreach (var detail in dto.Details)
                {
                    detail.JobRequirementRecommendID = dto.Master.ID;
                    if (detail.ID > 0)
                    {
                        await _detailRepo.UpdateAsync(detail);
                    }
                    else
                    {
                        await _detailRepo.CreateAsync(detail);
                    }
                }

                return Ok(ApiResponseFactory.Success(dto.Master.ID));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpPost("export-excel")]
        public IActionResult ExportExcel([FromBody] JobRequirementRecommendParam param)
        {
            try
            {
                var templateFolder = _configuration.GetValue<string>("PathTemplate");
                if (string.IsNullOrWhiteSpace(templateFolder))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy đường dẫn template trên server!"));

                string templatePath = Path.Combine(templateFolder, "ExportExcel", "TemplateRecommendJobRequirement.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy File mẫu TemplateRecommendJobRequirement.xlsx!"));

                var data = SQLHelper<dynamic>.ProcedureToList("spGetJobRequirementRecommendByID", new string[] { "@ID" }, new object[] { param.ID ?? 0 });
                var dtMaster = SQLHelper<dynamic>.GetListData(data, 0);
                var dtDetails = SQLHelper<dynamic>.GetListData(data, 1);

                if (!dtMaster.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xuất!"));

                using (var workbook = new XLWorkbook(templatePath))
                {
                    var sheet = workbook.Worksheet(1);
                    int startRow = 4;
                    int initialPlaceholders = 3;

                    // Fill Requirement Number in Header
                    var firstMaster = dtMaster.FirstOrDefault();
                    if (firstMaster != null)
                    {
                        var fm = (IDictionary<string, object>)firstMaster;
                        string requestNumber = GetVal(fm, "NumberRequest");

                        var targetCell = sheet.CellsUsed(c => c.Address.RowNumber <= 10)
                                              .FirstOrDefault(c => c.Value.ToString().Contains("Số yêu cầu"));
                        if (targetCell != null)
                        {
                            string existingVal = targetCell.Value.ToString();
                            if (existingVal.Contains("Số yêu cầu:"))
                            {
                                int colonIndex = existingVal.IndexOf("Số yêu cầu:");
                                string prefix = existingVal.Substring(0, colonIndex + "Số yêu cầu:".Length);
                                targetCell.Value = $"{prefix} {requestNumber}";
                            }
                            else
                            {
                                targetCell.Value = $"{existingVal} {requestNumber}";
                            }
                        }
                    }

                    // Calculate total rows needed
                    int totalRowsNeeded = 0;
                    foreach (var master in dtMaster)
                    {
                        var prods = dtDetails; // Since we only have one Master in this SP result
                        if (!prods.Any()) totalRowsNeeded += 1;
                        else totalRowsNeeded += prods.Count;
                    }

                    // Unmerge template placeholders
                    sheet.Range(startRow, 1, startRow + initialPlaceholders - 1, 20).Unmerge();

                    // Adjust rows
                    if (totalRowsNeeded > initialPlaceholders)
                    {
                        sheet.Row(startRow + initialPlaceholders - 1).InsertRowsBelow(totalRowsNeeded - initialPlaceholders);
                    }
                    else if (totalRowsNeeded < initialPlaceholders && totalRowsNeeded > 0)
                    {
                        sheet.Rows(startRow + totalRowsNeeded, startRow + initialPlaceholders - 1).Delete();
                    }

                    int currentRow = startRow;
                    int stt = 1;

                    foreach (var master in dtMaster)
                    {
                        var productGroups = dtDetails.GroupBy(d => d.ProductName).ToList();
                        int currentStt = stt++;

                        // Calculate total rows for this Master to merge A-K
                        int masterTotalRows = 0;
                        foreach (var group in productGroups)
                        {
                            masterTotalRows += group.Count();
                        }
                        if (masterTotalRows == 0) masterTotalRows = 1;

                        // Merge columns A-K (1 to 11) for the entire Master range
                        if (masterTotalRows > 1)
                        {
                            for (int col = 1; col <= 11; col++) // A to K
                            {
                                sheet.Range(currentRow, col, currentRow + masterTotalRows - 1, col).Merge();
                            }
                        }

                        if (!productGroups.Any())
                        {
                            FillMasterRow(sheet, currentRow, master, currentStt, "");
                            currentRow++;
                            continue;
                        }

                        int masterStartRow = currentRow;
                        foreach (var group in productGroups)
                        {
                            string productName = group.Key?.ToString() ?? "";
                            var suppliers = group.ToList();
                            int supplierCount = suppliers.Count;

                            // Merge column L (12) per product group
                            if (supplierCount > 1)
                            {
                                sheet.Range(currentRow, 12, currentRow + supplierCount - 1, 12).Merge();
                            }

                            // Fill Master info only on the first row of the Master (ClosedXML handles merged cells)
                            FillMasterRow(sheet, currentRow, master, currentStt, productName);

                            foreach (var supplier in suppliers)
                            {
                                var sRow = (IDictionary<string, object>)supplier;
                                sheet.Cell(currentRow, 13).Value = GetVal(sRow, "Supplier");
                                sheet.Cell(currentRow, 14).Value = GetVal(sRow, "Contact");

                                // Unit Price (O)
                                var cellUnitPrice = sheet.Cell(currentRow, 15);
                                if (double.TryParse(GetVal(sRow, "UnitPrice"), out double unitPrice))
                                {
                                    cellUnitPrice.Value = unitPrice;
                                    cellUnitPrice.Style.NumberFormat.Format = "#,##0";
                                }

                                // Total Amount (P)
                                var cellTotal = sheet.Cell(currentRow, 16);
                                if (double.TryParse(GetVal(sRow, "TotalAmount"), out double totalAmount))
                                {
                                    cellTotal.Value = totalAmount;
                                    cellTotal.Style.NumberFormat.Format = "#,##0";
                                }

                                sheet.Cell(currentRow, 17).Value = GetVal(sRow, "Note");

                                // Approval Status (R, S)
                                string isApproved = GetVal(sRow, "IsApproved");
                                var cellApprove = sheet.Cell(currentRow, 18);
                                var cellDisapprove = sheet.Cell(currentRow, 19);

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

                                sheet.Cell(currentRow, 20).Value = GetVal(sRow, "DisapprovalReason");

                                sheet.Row(currentRow).Style.Alignment.WrapText = true;
                                sheet.Row(currentRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                currentRow++;
                            }
                        }
                    }

                    var dataRange = sheet.Range(startRow, 1, currentRow - 1, 20);
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DeXuatPhuongAn_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private void FillMasterRow(IXLWorksheet sheet, int row, dynamic master, int stt, string productName)
        {
            var m = (IDictionary<string, object>)master;
            sheet.Cell(row, 1).Value = stt; // A
            sheet.Cell(row, 2).Value = GetVal(m, "EmployeeName"); // B
            sheet.Cell(row, 3).Value = GetVal(m, "ChucVu"); // C
            sheet.Cell(row, 4).Value = GetVal(m, "EmployeeDepartment"); // D
            sheet.Cell(row, 5).Value = FormatDate(GetVal(m, "DateRequest")); // E
            sheet.Cell(row, 6).Value = GetVal(m, "RequestContent"); // F
            sheet.Cell(row, 7).Value = GetVal(m, "Reason"); // G
            sheet.Cell(row, 8).Value = GetVal(m, "Unit"); // H
            sheet.Cell(row, 9).Value = GetVal(m, "Quantity"); // I
            sheet.Cell(row, 10).Value = FormatDate(GetVal(m, "DeadlineRequest")); // J
            sheet.Cell(row, 11).Value = GetVal(m, "Note"); // K
            sheet.Cell(row, 12).Value = productName; // L
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

        [HttpPost("approve-detail")]
        public async Task<IActionResult> ApproveDetail(int id, int status, string? reason = null)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var detail = _detailRepo.GetByID(id);
                if (detail == null) return BadRequest(ApiResponseFactory.Fail(null, "Bản ghi không tồn tại"));

                detail.IsApproved = status;
                detail.DisapprovalReason = reason;
                detail.ApproverID = currentUser.EmployeeID;
                detail.ApprovalDate = DateTime.Now;
                await _detailRepo.UpdateAsync(detail);
                return Ok(ApiResponseFactory.Success(true));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var master = _masterRepo.GetByID(id);
                if (master == null) return BadRequest(ApiResponseFactory.Fail(null, "Bản ghi không tồn tại"));

                master.IsDeleted = true;
                await _masterRepo.UpdateAsync(master);

                // Soft delete details
                var details = _detailRepo.GetAll(x => x.JobRequirementRecommendID == id && x.IsDeleted == false);
                foreach (var detail in details)
                {
                    detail.IsDeleted = true;

                    await _detailRepo.UpdateAsync(detail);
                }

                return Ok(ApiResponseFactory.Success(true));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}