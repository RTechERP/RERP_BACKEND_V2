using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;
using ZXing;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryDeleteBillController : ControllerBase
    {
        HistoryDeleteBillRepo _historyDeleteBillReoi = new HistoryDeleteBillRepo();

        [HttpGet]
        public IActionResult getAll()
        {
            try
            {
                List<HistoryDeleteBill> result = _historyDeleteBillReoi.GetAll().ToList();
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-by-billtype")]
        public IActionResult getByBillType([FromBody] HistoryBillDeleteParamRequest filter) {
            try
            {
                List<dynamic> rs;
                if (filter.billType == 1) // Phiếu nhập
                {
                    List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetBillImportLog", new string[] { "@BillImportID" },
                    new object[] { filter.billImportID ?? 0 }
                   );
                    rs = result[0];
                }
                else if (filter.billType == 2)
                {
                    List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                      "spGetBillImportTechLog", new string[] { "@BillImportTechID" },
                   new object[] { filter.billImportID ?? 0 }
                  );
                    rs = result[0];
                }
                else if (filter.billType == 3)
                {
                    List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                      "spGetBillExportTechLog", new string[] { "@BillExportTechID" },
                   new object[] { filter.billExportID ?? 0 }
                  );
                    rs = result[0];
                }
                else
                {
                    List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                      "spGetBillExportLog", new string[] { "@BillExportID" },
                   new object[] { filter.billExportID ?? 0 }
                  );
                    rs = result[0];
                }
                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("{id}")]
        public IActionResult getDataByID(int id)
        {
            try
            {
                var result = _historyDeleteBillReoi.GetByID(id);
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
