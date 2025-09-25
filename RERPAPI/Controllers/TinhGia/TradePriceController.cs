using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.TinhGia
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradePriceController : ControllerBase
    {
        TradePriceRepo _tradePriceRepo = new TradePriceRepo();
        TradePriceDetailRepo _tradePriceDetailRepo = new TradePriceDetailRepo();
        UnitCountRepo _unitCountRepo = new UnitCountRepo();
        ProjectRepo _projectRepo = new ProjectRepo();
        CustomerRepo _customerRepo = new CustomerRepo();

        [HttpGet]
        public IActionResult Get(int employeeId, int saleAdminId, int projectId, int customerId, string keyword = "")
        {
            try
            {
                //CHƯA CÓ XỬ LÍ EMPID THEO NGƯỜI DÙNG HIỆN TẠI NÊN ĐANG GET ALL THEO EMPID = -1
                int empId = -1;
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetTradePrice",
                    new string[] { "@ID", "@EmpID", "@EmployeeID", "@SaleAdminID", "@ProjectID", "@CustomerID", "@Keyword" },
                    new object[] { 0, empId, employeeId, saleAdminId, projectId, customerId, keyword });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-details")]
        public IActionResult GetDetails (int id)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetTradePriceDetail",
                    new string[] { "@TradePriceID" },
                    new object[] { id });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-unitcount")]
        public  IActionResult GetUnitCount()
        {
            try
            {
                var data = _unitCountRepo.GetAll().ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> Save([FromBody] TradePriceDTO dto)
        {
            try
            {
                if (dto.tradePrices.ID <= 0)
                {
                    await _tradePriceRepo.CreateAsync(dto.tradePrices);
                }
                else
                {
                    await _tradePriceRepo.UpdateAsync(dto.tradePrices);
                }
                var parentIdMapping = new Dictionary<int, int>();
                if (dto.tradePriceDetails.Count > 0)
                {
                    foreach (var item in dto.tradePriceDetails)
                    {
                        int idOld = item.ID;
                        int parentId = 0;

                        if (item.ParentID.HasValue && parentIdMapping.ContainsKey(item.ParentID.Value))
                        {
                            parentId = parentIdMapping[item.ParentID.Value];
                        }

                        TradePriceDetail model = idOld > 0 ? _tradePriceDetailRepo.GetByID(idOld) : new TradePriceDetail();
                        // gán id và gán parent id
                        model.TradePriceID = dto.tradePrices.ID;
                        model.ParentID = parentId;
                        //
                        // Gán lại dữ liệu cho TradePriceDetail
                        model.STT = item.STT;
                        model.Maker = item.Maker;
                        model.ProductID = item.ProductID;
                        model.ProductCodeCustomer = item.ProductCodeCustomer;
                        model.Quantity = item.Quantity;
                        model.Unit = item.Unit;
                        model.UnitImportPriceUSD = item.UnitImportPriceUSD;
                        model.TotalImportPriceUSD = item.TotalImportPriceUSD;
                        model.UnitImportPriceVND = item.UnitImportPriceVND;
                        model.TotalImportPriceVND = item.TotalImportPriceVND;
                        model.BankCharge = item.BankCharge;
                        model.ProtectiveTariff = item.ProtectiveTariff;
                        model.ProtectiveTariffPerPcs = item.ProtectiveTariffPerPcs;
                        model.TotalProtectiveTariff = item.TotalProtectiveTariff;
                        model.OrtherFees = item.OrtherFees;
                        model.CustomFees = item.CustomFees;
                        model.TotalImportPriceIncludeFees = item.TotalImportPriceIncludeFees;
                        model.UnitPriceIncludeFees = item.UnitPriceIncludeFees;
                        model.CMPerSet = item.CMPerSet;
                        model.UnitPriceExpectCustomer = item.UnitPriceExpectCustomer;
                        model.TotalPriceExpectCustomer = item.TotalPriceExpectCustomer;
                        model.Profit = item.Profit;
                        model.ProfitPercent = item.ProfitPercent;
                        model.LeadTime = item.LeadTime;
                        model.TotalPriceLabor = item.TotalPriceLabor;
                        model.TotalPriceRTCVision = item.TotalPriceRTCVision;
                        model.TotalPrice = item.TotalPrice;
                        model.UnitPricePerCOM = item.UnitPricePerCOM;
                        model.Note = item.Note;
                        model.CreatedDate = item.CreatedDate;
                        model.UpdatedDate = item.UpdatedDate;
                        model.CreatedBy = item.CreatedBy;
                        model.UpdatedBy = item.UpdatedBy;
                        model.UnitCountID = item.UnitCountID;
                        model.FeeShipPcs = item.FeeShipPcs;
                        model.ProductName = item.ProductName;
                        model.ProductCode = item.ProductCode;
                        model.ProductCodeOrigin = item.ProductCodeOrigin;
                        model.CurrencyID = item.CurrencyID;
                        model.CurrencyRate = item.CurrencyRate;
                        model.Margin = item.Margin; 

                        if (idOld > 0)
                        {
                            await _tradePriceDetailRepo.UpdateAsync(model);
                        }
                        else
                        {
                            await _tradePriceDetailRepo.CreateAsync(model);
                        }
                        parentIdMapping.Add(item.ID, model.ID);
                    }
                }
                if(dto.deletedTradePriceDetails.Count > 0)
                {
                    foreach( var item in dto.deletedTradePriceDetails)
                    {
                        var itemDel = _tradePriceDetailRepo.GetByID(item);
                        if(itemDel != null)
                        {
                            itemDel.IsDeleted = true;
                            itemDel.UpdatedDate = DateTime.Now;
                            await _tradePriceDetailRepo.UpdateAsync(itemDel);
                        }    
                    }    
                }

                //return Ok(new
                //{
                //    status = 1,
                //    message = "Success",
                //    data = new { id = dto.tradePrices.ID }
                //});
                return Ok(ApiResponseFactory.Success(new { id = dto.tradePrices.ID}, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("export-excel/{id}")]
        public IActionResult ExportExcel(int id)
        {
            try
            {
                TradePrice tradePrice = _tradePriceRepo.GetByID(id);
                Project project = _projectRepo.GetByID(tradePrice.ProjectID ?? 0);
                Customer customer = _customerRepo.GetByID(tradePrice.CustomerID ?? 0);
             
                List<List<dynamic>> data = SQLHelper<dynamic>.ProcedureToList("spGetTradePriceDetail", 
                    new string[] { "@TradePriceID" },
                    new object[] { id });

                var detailData = SQLHelper<dynamic>.GetListData(data, 0);
                if (!detailData.Any())
                {
                    throw new Exception("Không có dữ liệu chi tiết để xuất");
                }

                // Đường dẫn mẫu Excel
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "TINHGIATHUONGMAI.xlsx");
                if (!System.IO.File.Exists(templatePath))
                {
                    throw new Exception("Không tìm thấy file mẫu Excel");
                }

                using (var workbook = new XLWorkbook(templatePath))
                {
                    var sheet = workbook.Worksheet(1);

                    sheet.Cell(1, 1).Value = project.ProjectCode.ToString();
                    sheet.Cell(3, 1).Value = project.ProjectName.ToString();
                    sheet.Cell(4, 25).Value = tradePrice.RateCOM;
                    sheet.Cell(4, 26).Value = tradePrice.COM;

                    int startRow = 7;
                    int templateRows = 6; // số dòng mẫu

                    // Nếu số lượng sản phẩm > số dòng mẫu thì chèn thêm dòng
                    if (detailData.Count > templateRows)
                    {
                        sheet.Row(startRow + templateRows - 1).InsertRowsBelow(detailData.Count - templateRows);
                    }

                    for (int i = 0; i < detailData.Count; i++)
                    {
                        var rowData = (IDictionary<string, object>)detailData[i];
                        int rowIdx = startRow + i;

                        sheet.Cell(rowIdx, 2).Value = rowData["STT"]?.ToString() ?? "";
                        sheet.Cell(rowIdx, 3).Value = rowData["Maker"]?.ToString() ?? "";
                        sheet.Cell(rowIdx, 4).Value = rowData["ProductName"]?.ToString() ?? "";
                        sheet.Cell(rowIdx, 5).Value = rowData["ProductCode"]?.ToString() ?? "";
                        sheet.Cell(rowIdx, 6).Value = rowData["ProductCodeCustomer"]?.ToString() ?? "";
                        sheet.Cell(rowIdx, 7).Value = Convert.ToDouble(rowData["Quantity"] ?? 0);
                        sheet.Cell(rowIdx, 8).Value = rowData["Unit"]?.ToString() ?? "";
                        sheet.Cell(rowIdx, 9).Value = Convert.ToDecimal(rowData["UnitImportPriceUSD"] ?? 0);
                        sheet.Cell(rowIdx, 10).Value = Convert.ToDecimal(rowData["TotalImportPriceUSD"] ?? 0);
                        sheet.Cell(rowIdx, 11).Value = Convert.ToDecimal(rowData["UnitImportPriceVND"] ?? 0);
                        sheet.Cell(rowIdx, 12).Value = Convert.ToDecimal(rowData["TotalImportPriceVND"] ?? 0)    ;
                        sheet.Cell(rowIdx, 13).Value = Convert.ToDecimal(rowData["BankCharge"] ?? 0) ;
                        sheet.Cell(rowIdx, 14).Value = Convert.ToDecimal(rowData["ProtectiveTariff"] ?? 0);
                        sheet.Cell(rowIdx, 15).Value = Convert.ToDecimal(rowData["ProtectiveTariffPerPcs"] ?? 0);
                        sheet.Cell(rowIdx, 16).Value = Convert.ToDecimal(rowData["TotalProtectiveTariff"] ?? 0);
                        sheet.Cell(rowIdx, 17).Value = Convert.ToDecimal(rowData["OrtherFees"] ?? 0);
                        sheet.Cell(rowIdx, 18).Value = Convert.ToDecimal(rowData["CustomFees"] ?? 0);
                        sheet.Cell(rowIdx, 19).Value = Convert.ToDecimal(rowData["TotalImportPriceIncludeFees"] ?? 0);
                        sheet.Cell(rowIdx, 20).Value = Convert.ToDecimal(rowData["UnitPriceIncludeFees"] ?? 0);
                        sheet.Cell(rowIdx, 21).Value = Convert.ToDecimal(rowData["CMPerSet"] ?? 0);
                        sheet.Cell(rowIdx, 22).Value = Convert.ToDecimal(rowData["UnitPriceExpectCustomer"] ?? 0);
                        sheet.Cell(rowIdx, 23).Value = Convert.ToDecimal(rowData["TotalPriceExpectCustomer"] ?? 0);
                        sheet.Cell(rowIdx, 24).Value = Convert.ToDecimal(rowData["Profit"] ?? 0);
                        sheet.Cell(rowIdx, 25).Value = Convert.ToDouble(rowData["ProfitPercent"] ?? 0);
                        sheet.Cell(rowIdx, 26).Value = rowData["LeadTime"]?.ToString() ?? "";
                        sheet.Cell(rowIdx, 29).Value = Convert.ToDecimal(rowData["TotalPrice"] ?? 0);
                        sheet.Cell(rowIdx, 30).Value = Convert.ToDecimal(rowData["UnitPricePerCOM"] ?? 0);
                        sheet.Cell(rowIdx, 31).Value = rowData["Note"]?.ToString() ?? "";

                        sheet.Cell(7, 1).Value = $"{customer.ContactName}_{project.ProjectCode}_{project.ProjectName}";
                        sheet.Cell(7, 27).Value = Convert.ToDecimal(rowData["TotalPriceLabor"] ?? 0);
                        sheet.Cell(7, 28).Value = Convert.ToDecimal(rowData["TotalPriceRTCVision"] ?? 0);

                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        var fileName = $"{project.ProjectCode}_{DateTime.Now:yy-MM-dd}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
