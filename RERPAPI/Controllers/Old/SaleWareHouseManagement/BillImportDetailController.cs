using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillImportDetailController : ControllerBase
    {
        private readonly BillImportDetailRepo _billImportdetailRepo;

        public BillImportDetailController(BillImportDetailRepo billImportDetailRepo)
        {
            _billImportdetailRepo = billImportDetailRepo;
        }

        [HttpGet("BillImportID/{billImportID}")]
        public IActionResult getBillExportDetailByBillID(int billImportID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetBillImportDetail", new string[] { "@ID" },
                    new object[] { billImportID }
                   );
                List<dynamic> billDetail = result[0]; // dữ liệu chi tiết hóa đơn
                int totalPage = 0;
                return Ok(new
                {
                    status = 1,
                    data = billDetail,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData(List<BillImportDetail> details)
        {
            try
            {
                if (details.Count < 0) return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu trả về!"));
                foreach (BillImportDetail detail in details)
                {
                    if (detail.ID > 0) await _billImportdetailRepo.UpdateAsync(detail);
                    else await _billImportdetailRepo.CreateAsync(detail);
                }
                return Ok(ApiResponseFactory.Success(details, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-bill")]
        public async Task<IActionResult> SaveBill(List<BillImportSummaryUpdateDTO> details)
        {

            if (details == null || details.Count == 0)
                return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu thay đổi"));

            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);

            bool isAdmin = currentUser.IsAdmin && currentUser.ID <= 0;

            int successCount = 0;
            List<int> skippedIds = new();

            try
            {
                foreach (var item in details)
                {
                    if (item.IDDetail <= 0)
                        continue;

                    var billDetail = _billImportdetailRepo.GetByID(item.IDDetail);
                    if (item.DeliverID != currentUser.ID && !isAdmin && !_billImportdetailRepo.HasPermission("N105"))
                    {
                        skippedIds.Add(item.IDDetail);
                        continue;
                    }

                    DateTime? dueDate = null;
                    if (item.DateSomeBill.HasValue)
                        dueDate = item.DateSomeBill.Value.AddDays(item.DPO);

                    billDetail.SomeBill = item.SomeBill;
                    billDetail.DateSomeBill = item.DateSomeBill;
                    billDetail.DueDate = dueDate;
                    billDetail.TaxReduction = item.TaxReduction;
                    billDetail.COFormE = item.COFormE;
                    billDetail.DPO = item.DPO;

                    int rs = await _billImportdetailRepo.UpdateAsync(billDetail);
                    if (rs > 0)
                    {
                        successCount++;
                    }
                }
                if (successCount > 0)
                {
                    return Ok(ApiResponseFactory.Success(
                        new
                        {
                            Updated = successCount,
                        },
                        $"Lưu thành công {successCount} dòng"
                    ));
                }
                return Ok(ApiResponseFactory.Fail(null, $"Lưu dữ liệu thất bại!"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}