using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryDeleteBillController : ControllerBase
    {
        private readonly HistoryDeleteBillRepo _historyDeleteBillRepo;
        public HistoryDeleteBillController(HistoryDeleteBillRepo historyDeleteBillRepo)
        {
            _historyDeleteBillRepo = historyDeleteBillRepo;
        }

        [HttpGet]
        public IActionResult getAll()
        {
            try
            {
                List<HistoryDeleteBill> result = _historyDeleteBillRepo.GetAll().ToList();
                return Ok(new
                {
                    status = 1,
                    data = result,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    status = 0,
                });
            }
        }
        [HttpPost("get-by-billtype")]
        public IActionResult getByBillType([FromBody] HistoryBillDeleteParamRequest filter)
        {
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
                return Ok(new
                {
                    status = 1,
                    data = rs,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    status = 0,
                });
            }
        }
        [HttpGet("{id}")]
        public IActionResult getDataByID(int id)
        {
            try
            {
                var result = _historyDeleteBillRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = result,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    status = 0,
                });
            }
        }
    }
}
