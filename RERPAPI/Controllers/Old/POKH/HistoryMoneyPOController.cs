using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryMoneyPOController : ControllerBase
    {
        HistoryMoneyPORepo _historyMoneyPORepo;
        POKHDetailRepo _pokhDetailRepo;
        POKHRepo _pokhRepo;
        public HistoryMoneyPOController(HistoryMoneyPORepo historyMoneyPORepo, POKHDetailRepo pokhDetailRepo, POKHRepo pokhRepo)
        {
            _historyMoneyPORepo = historyMoneyPORepo;
            _pokhDetailRepo = pokhDetailRepo;
            _pokhRepo = pokhRepo;
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

        [HttpPost("save")]
        public async Task<IActionResult> SaveHistoryMoney(HistoryMoneyPODTO dto)
        {
            if (dto.historyMoneyPOs == null || !dto.historyMoneyPOs.Any())
                return BadRequest("Danh sách dữ liệu trống.");

            try
            {

                if(dto.listIdsDel.Count > 0)
                {
                    foreach(int id in dto.listIdsDel)
                    {
                        HistoryMoneyPO modelDel = await _historyMoneyPORepo.GetByIDAsync(id);
                        modelDel.IsDeleted = true;
                        modelDel.UpdatedDate = DateTime.Now;
                        await _historyMoneyPORepo.UpdateAsync(modelDel);
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

                    // Update RecivedMoneyDate
                    if (model.MoneyDate.HasValue)
                    {
                        POKHDetail pokhDetailModel = await _pokhDetailRepo.GetByIDAsync(dto.pokhDetailId);
                        if (pokhDetailModel != null)
                        {
                            pokhDetailModel.RecivedMoneyDate = model.MoneyDate;
                        }
                        await _pokhDetailRepo.UpdateAsync(pokhDetailModel);
                    }

                    if (item.ID > 0)
                    {
                        await _historyMoneyPORepo.UpdateAsync(model);
                    }
                    else
                    {
                        model.POKHID = dto.pokhId;
                        model.POKHDetailID = dto.pokhDetailId;
                        await _historyMoneyPORepo.CreateAsync(model);
                    }
                }

                // ===== Update PO =====
                RERPAPI.Model.Entities.POKH po = await _pokhRepo.GetByIDAsync(dto.pokhId);
                if (po == null)
                    return BadRequest("Không tìm thấy POKH.");

                decimal totalReceive = _historyMoneyPORepo.GetAll(x=>x.POKHID == dto.pokhId).Sum(x => x.Money ?? 0);

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


                return Ok(ApiResponseFactory.Success( new { TotalMoneyRemaining = totalMoneyRemaining }, "Lưu thành công"));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Error = ex.Message });
            }
        }

    }
}
