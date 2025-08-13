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
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(list, 0)
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
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
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(list, 0)
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message, 
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("get-unitcount")]
        public  IActionResult GetUnitCount()
        {
            try
            {
                var data = _unitCountRepo.GetAll().ToList();
                return Ok(new
                {
                    status = 1,
                    data,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
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

                return Ok(new
                {
                    status = 1,
                    message = "Success",
                    data = new { id = dto.tradePrices.ID }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
