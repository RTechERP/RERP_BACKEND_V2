using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIErrorFineAmountController : ControllerBase
    {
        private readonly KPIErrorFineAmountRepo _kpiErrorFineAmountRepo;
        private readonly KPIErrorRepo _kpiErrorRepo;
        public KPIErrorFineAmountController(KPIErrorFineAmountRepo kpiErrorFineAmountRepo, KPIErrorRepo kpiErrorRepo)
        {
            _kpiErrorFineAmountRepo = kpiErrorFineAmountRepo;
            _kpiErrorRepo = kpiErrorRepo;
        }

        [HttpGet("get-kpierror")]
        public IActionResult GetKPIError(int departmentId, string keyword = "")
        {
            try
            {
                var dataKpiError = SQLHelper<object>.ProcedureToList("spGetKPIError",
                                                new string[] { "@Keyword", "@TypeID", "@DepartmentID" },
                                                new object[] { "",0,0 });
                var data = SQLHelper<object>.GetListData(dataKpiError, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-error-fine-amount")]
        public IActionResult GetKPIErrorFineAmount(int kpiErrorId)
        {
            try
            {
                //var dataKpiError = SQLHelper<object>.ProcedureToList("spGetKPIErrorFineAmount",
                //                                new string[] { "@KPIErrorID" },
                //                                new object[] { kpiErrorId });
                //var data = SQLHelper<object>.GetListData(dataKpiError, 0);
                //return Ok(ApiResponseFactory.Success(data, ""));

                var dataKpiError = SQLHelper<object>.ProcedureToList(
                    "spGetKPIErrorFineAmount",
                    new string[] { "@KPIErrorID" },
                    new object[] { kpiErrorId }
                );

                var data = SQLHelper<object>.GetListData(dataKpiError, 0);

                //Nếu chưa có dữ liệu insert mặc định 1 -> 6
                if (data == null || data.Count == 0)
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        _kpiErrorFineAmountRepo.Create(new KPIErrorFineAmount
                        {
                            KPIErrorID = kpiErrorId,
                            QuantityError = i,
                            TotalMoneyError = 0
                        });
                    }

                    // ===== Load lại sau khi insert =====
                    dataKpiError = SQLHelper<object>.ProcedureToList(
                        "spGetKPIErrorFineAmount",
                        new string[] { "@KPIErrorID" },
                        new object[] { kpiErrorId }
                    );

                    data = SQLHelper<object>.GetListData(dataKpiError, 0);
                }

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-kpi-error-fine-amount")]
        public IActionResult SaveKPIErrorFineAmount([FromBody] List<KPIErrorFineAmount> models)
        {
            try
            {
                if (models == null || models.Count == 0)
                    return Ok(ApiResponseFactory.Success(null, ""));

                foreach (var item in models)
                {
                    _kpiErrorFineAmountRepo.Update(item);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}
