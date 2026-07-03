using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryMoneyPOController : ControllerBase
    {
        private HistoryMoneyPORepo _historyMoneyPORepo;
        private POKHDetailRepo _pokhDetailRepo;
        private POKHRepo _pokhRepo;
        private EmployeeTeamSaleRepo _employeeTeamSaleRepo;
        private DepartmentRepo _departmentRepo;
        public HistoryMoneyPOController(HistoryMoneyPORepo historyMoneyPORepo, POKHDetailRepo pokhDetailRepo, POKHRepo pokhRepo, EmployeeTeamSaleRepo employeeTeamSaleRepo, DepartmentRepo departmentRepo)
        {
            _historyMoneyPORepo = historyMoneyPORepo;
            _pokhDetailRepo = pokhDetailRepo;
            _pokhRepo = pokhRepo;
            _employeeTeamSaleRepo = employeeTeamSaleRepo;
            _departmentRepo = departmentRepo;
        }

        [HttpGet("get-banknames")]
        [Authorize]
        public IActionResult GetBankNames()
        {
            var listBanks = new List<object>()
            {
                new { BankName = "Techcombank-CN Ba Đình (19037214270015)" },
                new { BankName = "MB Bank-CN Đông Anh (835333886666)" },
                new { BankName = "TP Bank-CN Hà Nội (007 1361 8001)" }
            };

            return Ok(ApiResponseFactory.Success(listBanks, ""));
        }

        [HttpGet("load-product-data")]
        [Authorize]
        public IActionResult LoadProductData(string filterText = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetProductByPOorSHD",
                         new string[] { "@Filter" },
                         new object[] { filterText });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("load-products-by-pokh-ids")]
        [Authorize]
        public IActionResult LoadProductsByPOKHIds([FromBody] List<int> pokhIds)
        {
            try
            {
                if (pokhIds == null || pokhIds.Count == 0)
                {
                    return Ok(ApiResponseFactory.Success(new List<object>(), ""));
                }

                string pokhIdsStr = string.Join(",", pokhIds);
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetProductByPOKHIds",
                         new string[] { "@POKHIds" },
                         new object[] { pokhIdsStr });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpGet("load-team-sales")]
        //[Authorize]
        //public IActionResult LoadTeamSales()
        //{
        //    try
        //    {
        //        var list = _employeeTeamSaleRepo.GetAll(x => x.ParentID == 0 && x.IsDeleted != 1).ToList();
        //        return Ok(ApiResponseFactory.Success(list, ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpGet("load-departments")]
        [Authorize]
        public IActionResult LoadDepartments()
        {
            try
            {
                var list = _departmentRepo.GetAll(x => x.IsDeleted != true).ToList();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-history-money-po")]
        [Authorize]
        public IActionResult LoadHistoryMoneyPO(int pokhDetailId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetHistoryMoneyPONew",
                         new string[] { "@POKHDetailID" },
                         new object[] { pokhDetailId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("load-history-money-po-multiple")]
        [Authorize]
        public IActionResult LoadHistoryMoneyPOMultiple([FromBody] List<int> pokhDetailIds)
        {
            try
            {
                if (pokhDetailIds == null || pokhDetailIds.Count == 0)
                {
                    return Ok(ApiResponseFactory.Success(new List<object>(), ""));
                }

                string pokhDetailIdsStr = string.Join(",", pokhDetailIds);
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetHistoryMoneyPOMultiple",
                         new string[] { "@POKHDetailIds" },
                         new object[] { pokhDetailIdsStr });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("export-excel")]
        [AllowAnonymous]
        public IActionResult ExportExcel(int pokhDetailId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetHistoryMoneyPONew",
                         new string[] { "@POKHDetailID" },
                         new object[] { pokhDetailId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("LichSuTienVe");

                    // Headers
                    worksheet.Cell(1, 1).Value = "STT";
                    worksheet.Cell(1, 2).Value = "Ngày tiền về";
                    worksheet.Cell(1, 3).Value = "Số tiền";
                    worksheet.Cell(1, 4).Value = "Ghi chú";
                    worksheet.Cell(1, 5).Value = "Ngân hàng";
                    worksheet.Cell(1, 6).Value = "Số hóa đơn";
                    worksheet.Cell(1, 7).Value = "Người phụ trách (Admin)";
                    worksheet.Cell(1, 8).Value = "Người phụ trách (PM)";
                    worksheet.Cell(1, 9).Value = "Team";
                    worksheet.Cell(1, 10).Value = "VAT (%)";
                    worksheet.Cell(1, 11).Value = "Tiền trước VAT";

                    int row = 2;
                    foreach (var item in data)
                    {
                        var dict = item as IDictionary<string, object>;
                        worksheet.Cell(row, 1).Value = dict.ContainsKey("STT") && dict["STT"] != null ? dict["STT"].ToString() : (row - 1).ToString();

                        if (dict.ContainsKey("MoneyDate") && dict["MoneyDate"] != null)
                        {
                            if (DateTime.TryParse(dict["MoneyDate"].ToString(), out DateTime date))
                                worksheet.Cell(row, 2).Value = date.ToString("dd/MM/yyyy");
                        }

                        worksheet.Cell(row, 3).Value = dict.ContainsKey("Money") && dict["Money"] != null ? Convert.ToDecimal(dict["Money"]) : 0;
                        worksheet.Cell(row, 4).Value = dict.ContainsKey("Note") && dict["Note"] != null ? dict["Note"].ToString() : "";
                        worksheet.Cell(row, 5).Value = dict.ContainsKey("BankName") && dict["BankName"] != null ? dict["BankName"].ToString() : "";
                        worksheet.Cell(row, 6).Value = dict.ContainsKey("InvoiceNo") && dict["InvoiceNo"] != null ? dict["InvoiceNo"].ToString() : "";
                        worksheet.Cell(row, 7).Value = dict.ContainsKey("EmployeeName") && dict["EmployeeName"] != null ? dict["EmployeeName"].ToString() : "";
                        worksheet.Cell(row, 8).Value = dict.ContainsKey("PMEmployeeName") && dict["PMEmployeeName"] != null ? dict["PMEmployeeName"].ToString() : "";
                        worksheet.Cell(row, 9).Value = dict.ContainsKey("TeamSaleName") && dict["TeamSaleName"] != null ? dict["TeamSaleName"].ToString() : "";
                        worksheet.Cell(row, 10).Value = dict.ContainsKey("VAT") && dict["VAT"] != null ? Convert.ToDecimal(dict["VAT"]) : 0;
                        worksheet.Cell(row, 11).Value = dict.ContainsKey("MoneyVAT") && dict["MoneyVAT"] != null ? Convert.ToDecimal(dict["MoneyVAT"]) : 0;

                        row++;
                    }

                    var rngHeaders = worksheet.Range("A1:K1");
                    rngHeaders.Style.Font.Bold = true;
                    rngHeaders.Style.Fill.BackgroundColor = XLColor.LightGray;

                    var rngTable = worksheet.Range($"A1:J{Math.Max(row - 1, 1)}");
                    rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    // Format money columns
                    if (row > 2)
                    {
                        worksheet.Range($"C2:C{row - 1}").Style.NumberFormat.Format = "#,##0";
                        worksheet.Range($"K2:K{row - 1}").Style.NumberFormat.Format = "#,##0";
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"LichSuTienVe_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("export-excel-multiple")]
        [AllowAnonymous]
        public IActionResult ExportExcelMultiple([FromBody] List<int> pokhDetailIds)
        {
            try
            {
                if (pokhDetailIds == null || pokhDetailIds.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách POKHDetail IDs trống"));
                }

                string pokhDetailIdsStr = string.Join(",", pokhDetailIds);
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetHistoryMoneyPOMultiple",
                         new string[] { "@POKHDetailIds" },
                         new object[] { pokhDetailIdsStr });
                var data = SQLHelper<dynamic>.GetListData(list, 0);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("LichSuTienVe");

                    // Headers
                    worksheet.Cell(1, 1).Value = "STT";
                    worksheet.Cell(1, 2).Value = "Mã PO";
                    worksheet.Cell(1, 3).Value = "Khách hàng";
                    worksheet.Cell(1, 4).Value = "Ngày tiền về";
                    worksheet.Cell(1, 5).Value = "Số tiền";
                    worksheet.Cell(1, 6).Value = "Ghi chú";
                    worksheet.Cell(1, 7).Value = "Ngân hàng";
                    worksheet.Cell(1, 8).Value = "Số hóa đơn";
                    worksheet.Cell(1, 9).Value = "Người phụ trách (Admin)";
                    worksheet.Cell(1, 10).Value = "Người phụ trách (PM)";
                    worksheet.Cell(1, 11).Value = "Team";
                    worksheet.Cell(1, 12).Value = "VAT (%)";
                    worksheet.Cell(1, 13).Value = "Tiền trước VAT";

                    int row = 2;
                    foreach (var item in data)
                    {
                        var dict = item as IDictionary<string, object>;
                        worksheet.Cell(row, 1).Value = dict.ContainsKey("STT") && dict["STT"] != null ? dict["STT"].ToString() : (row - 1).ToString();
                        worksheet.Cell(row, 2).Value = dict.ContainsKey("POCode") && dict["POCode"] != null ? dict["POCode"].ToString() : "";
                        worksheet.Cell(row, 3).Value = dict.ContainsKey("CustomerName") && dict["CustomerName"] != null ? dict["CustomerName"].ToString() : "";

                        if (dict.ContainsKey("MoneyDate") && dict["MoneyDate"] != null)
                        {
                            if (DateTime.TryParse(dict["MoneyDate"].ToString(), out DateTime date))
                                worksheet.Cell(row, 4).Value = date.ToString("dd/MM/yyyy");
                        }

                        worksheet.Cell(row, 5).Value = dict.ContainsKey("Money") && dict["Money"] != null ? Convert.ToDecimal(dict["Money"]) : 0;
                        worksheet.Cell(row, 6).Value = dict.ContainsKey("Note") && dict["Note"] != null ? dict["Note"].ToString() : "";
                        worksheet.Cell(row, 7).Value = dict.ContainsKey("BankName") && dict["BankName"] != null ? dict["BankName"].ToString() : "";
                        worksheet.Cell(row, 8).Value = dict.ContainsKey("InvoiceNo") && dict["InvoiceNo"] != null ? dict["InvoiceNo"].ToString() : "";
                        worksheet.Cell(row, 9).Value = dict.ContainsKey("EmployeeName") && dict["EmployeeName"] != null ? dict["EmployeeName"].ToString() : "";
                        worksheet.Cell(row, 10).Value = dict.ContainsKey("PMEmployeeName") && dict["PMEmployeeName"] != null ? dict["PMEmployeeName"].ToString() : "";
                        worksheet.Cell(row, 11).Value = dict.ContainsKey("TeamSaleName") && dict["TeamSaleName"] != null ? dict["TeamSaleName"].ToString() : "";
                        worksheet.Cell(row, 12).Value = dict.ContainsKey("VAT") && dict["VAT"] != null ? Convert.ToDecimal(dict["VAT"]) : 0;
                        worksheet.Cell(row, 13).Value = dict.ContainsKey("MoneyVAT") && dict["MoneyVAT"] != null ? Convert.ToDecimal(dict["MoneyVAT"]) : 0;

                        row++;
                    }

                    var rngHeaders = worksheet.Range("A1:M1");
                    rngHeaders.Style.Font.Bold = true;
                    rngHeaders.Style.Fill.BackgroundColor = XLColor.LightGray;

                    var rngTable = worksheet.Range($"A1:M{Math.Max(row - 1, 1)}");
                    rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    // Format money columns
                    if (row > 2)
                    {
                        worksheet.Range($"E2:E{row - 1}").Style.NumberFormat.Format = "#,##0";
                        worksheet.Range($"M2:M{row - 1}").Style.NumberFormat.Format = "#,##0";
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"LichSuTienVe_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("export-excel-filter")]
        [AllowAnonymous]
        public IActionResult ExportExcelFilter(DateTime? fromDate, DateTime? toDate, int? departmentId, int? userId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spExportHistoryMoneyPO",
                         new string[] { "@FromDate", "@ToDate", "@TeamSaleID", "@UserID" },
                         new object[] { fromDate, toDate, departmentId, userId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("LichSuTienVe");

                    // Headers
                    worksheet.Cell(1, 1).Value = "STT";
                    worksheet.Cell(1, 2).Value = "Sales phụ trách";
                    worksheet.Cell(1, 3).Value = "Số PO";
                    worksheet.Cell(1, 4).Value = "Khách hàng";
                    worksheet.Cell(1, 5).Value = "Giá trị PO";
                    worksheet.Cell(1, 6).Value = "Giá trị tiền về";
                    worksheet.Cell(1, 7).Value = "Admin phụ trách";

                    int row = 2;
                    foreach (var item in data)
                    {
                        var dict = item as IDictionary<string, object>;
                        worksheet.Cell(row, 1).Value = dict.ContainsKey("STT") && dict["STT"] != null ? dict["STT"].ToString() : (row - 1).ToString();
                        worksheet.Cell(row, 2).Value = dict.ContainsKey("FullName") && dict["FullName"] != null ? dict["FullName"].ToString() : (dict.ContainsKey("LoginName") && dict["LoginName"] != null ? dict["LoginName"].ToString() : "");
                        worksheet.Cell(row, 3).Value = dict.ContainsKey("POCode") && dict["POCode"] != null ? dict["POCode"].ToString() : "";
                        worksheet.Cell(row, 4).Value = dict.ContainsKey("CustomerName") && dict["CustomerName"] != null ? dict["CustomerName"].ToString() : "";
                        worksheet.Cell(row, 5).Value = dict.ContainsKey("TotalMoneyPO") && dict["TotalMoneyPO"] != null ? Convert.ToDecimal(dict["TotalMoneyPO"]) : 0;
                        worksheet.Cell(row, 6).Value = dict.ContainsKey("Money") && dict["Money"] != null ? Convert.ToDecimal(dict["Money"]) : 0;
                        worksheet.Cell(row, 7).Value = dict.ContainsKey("EmployeeName") && dict["EmployeeName"] != null ? dict["EmployeeName"].ToString() : "";

                        row++;
                    }

                    // Style headers
                    var rngHeaders = worksheet.Range("A1:G1");
                    rngHeaders.Style.Font.Bold = true;
                    rngHeaders.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Borders
                    var rngTable = worksheet.Range($"A1:G{Math.Max(row - 1, 1)}");
                    rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    // Format money columns
                    if (row > 2)
                    {
                        worksheet.Range($"E2:E{row - 1}").Style.NumberFormat.Format = "#,##0";
                        worksheet.Range($"F2:F{row - 1}").Style.NumberFormat.Format = "#,##0";
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"LichSuTienVe_Export_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveHistoryMoney(HistoryMoneyPODTO dto)
        {
            if (dto.historyMoneyPOs == null || !dto.historyMoneyPOs.Any())
                return BadRequest("Danh sách dữ liệu trống.");

            try
            {
                if (dto.listIdsDel != null && dto.listIdsDel.Count > 0)
                {
                    foreach (int id in dto.listIdsDel)
                    {
                        HistoryMoneyPO modelDel = await _historyMoneyPORepo.GetByIDAsync(id);
                        if (modelDel != null)
                        {
                            modelDel.IsDeleted = true;
                            modelDel.UpdatedDate = DateTime.Now;
                            await _historyMoneyPORepo.UpdateAsync(modelDel);
                        }
                    }
                }

                foreach (var item in dto.historyMoneyPOs)
                {
                    HistoryMoneyPO model = item.ID > 0 ? await _historyMoneyPORepo.GetByIDAsync(item.ID) : new HistoryMoneyPO();

                    model.Money = item.Money;
                    model.MoneyVAT = item.MoneyVAT;
                    model.VAT = item.VAT;
                    model.MoneyDate = item.MoneyDate;
                    model.BankName = item.BankName;
                    model.InvoiceNo = item.InvoiceNo;
                    model.Note = item.Note;
                    model.ProductID = item.ProductID;
                    model.IsFilm = item.IsFilm;
                    model.UserID = item.UserID;
                    model.PMUserID = item.PMUserID;

                    // Update RecivedMoneyDate for the specific POKHDetail
                    if (model.MoneyDate.HasValue)
                    {
                        // Use item.POKHDetailID if available, otherwise fallback to dto.pokhDetailId
                        int targetPOKHDetailId = item.POKHDetailID > 0 ? item.POKHDetailID ?? 0 : dto.pokhDetailId;
                        POKHDetail pokhDetailModel = await _pokhDetailRepo.GetByIDAsync(targetPOKHDetailId);
                        if (pokhDetailModel != null)
                        {
                            pokhDetailModel.RecivedMoneyDate = model.MoneyDate;
                            await _pokhDetailRepo.UpdateAsync(pokhDetailModel);
                        }
                    }

                    if (item.ID > 0)
                    {
                        await _historyMoneyPORepo.UpdateAsync(model);
                    }
                    else
                    {
                        // Use item.POKHDetailID and item.POKHID if available, otherwise fallback to dto
                        model.POKHID = item.POKHID > 0 ? item.POKHID : dto.pokhId;
                        model.POKHDetailID = item.POKHDetailID > 0 ? item.POKHDetailID : dto.pokhDetailId;
                        await _historyMoneyPORepo.CreateAsync(model);
                    }
                }

                // ===== Update PO (for single PO save) =====
                // Only update PO when saving for a single POKHDetail
                if (dto.pokhId > 0 && dto.pokhDetailId > 0)
                {
                    RERPAPI.Model.Entities.POKH po = await _pokhRepo.GetByIDAsync(dto.pokhId);
                    if (po != null)
                    {
                        decimal totalReceive = _historyMoneyPORepo.GetAll(x => x.POKHID == dto.pokhId).Sum(x => x.Money ?? 0);

                        po.ReceiveMoney = totalReceive;

                        // Logic trạng thái giống Winform
                        if (totalReceive > 0)
                        {
                            po.IsPay = true;

                            if (po.IsPay == true && po.IsExport == true && po.IsBill == true)
                                po.Status = 1;
                            else if (po.IsPay != true && po.IsExport == true)
                                po.Status = 3;
                            else if (po.IsPay != true && po.IsExport != true)
                                po.Status = 0;
                            else if (po.IsPay == true && po.IsExport != true)
                                po.Status = 2;
                            else if (po.IsPay == true && po.IsExport == true && po.IsBill != true)
                                po.Status = 4;
                            else if (po.IsShip == true && po.IsExport != true)
                                po.Status = 5;
                        }

                        // PaymentStatus
                        if (po.IsPay == true)
                        {
                            if (po.ReceiveMoney > 0 && po.ReceiveMoney < po.TotalMoneyPO)
                                po.PaymentStatus = 2;
                            else if (po.TotalMoneyPO - po.ReceiveMoney <= 0)
                                po.PaymentStatus = 3;
                        }
                        else
                        {
                            po.PaymentStatus = 1;
                        }

                        po.DeliveryStatus = 1;

                        await _pokhRepo.UpdateAsync(po);

                        decimal totalMoney = dto.totalMoneyIncludeVAT;

                        decimal totalMoneyRemaining = totalMoney - totalReceive;

                        return Ok(ApiResponseFactory.Success(new { TotalMoneyRemaining = totalMoneyRemaining }, "Lưu thành công"));
                    }
                }

                // For multi-POKHDetail save, return success without updating PO
                return Ok(ApiResponseFactory.Success(new { TotalMoneyRemaining = 0 }, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Error = ex.Message });
            }
        }
    }
}