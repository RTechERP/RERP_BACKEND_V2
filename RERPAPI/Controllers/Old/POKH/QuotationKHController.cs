using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationKHController : ControllerBase
    {
        private readonly QuotationKHDetailRepo _quotationDetailKHRepo;
        private readonly QuotationKHRepo _quotationKHRepo;

        public QuotationKHController(QuotationKHDetailRepo quotationDetailKHRepo, QuotationKHRepo quotationKHRepo)
        {
            _quotationDetailKHRepo = quotationDetailKHRepo;
            _quotationKHRepo = quotationKHRepo;
        }

        [HttpGet]
        public IActionResult Get(int status, int customerId, int userId, int size, int page, string filterText = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetQuotationKH",
                    new string[] { "@FilterText", "@Status", "@CustomerID", "@UserID", "@PageSize", "@PageNumber" },
                    new object[] { filterText, status, customerId, userId, size, page });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-details")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                var list = _quotationDetailKHRepo.GetAll().Where(x => x.QuotationKHID == id
                //&& x.IsDeleted != true
                );
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
