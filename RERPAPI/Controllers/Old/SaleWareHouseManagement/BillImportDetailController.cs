using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
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
    }
}
