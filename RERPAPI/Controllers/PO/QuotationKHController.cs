using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.PO
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationKHController : ControllerBase
    {
        QuotationDetailKHRepo _quotationDetailKHRepo = new QuotationDetailKHRepo();
        QuotationKHRepo _quotationKHRepo = new QuotationKHRepo();

        [HttpGet]
        public IActionResult Get(int status, int customerId, int userId, int size, int page, string filterText = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetQuotationKH",
                    new string[] { "@FilterText", "@Status", "@CustomerID", "@UserID", "@PageSize", "@PageNumber" },
                    new object[] { filterText, status, customerId, userId, size, page  });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(list, 0),
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
        public IActionResult GetDetail(int id)
        {
            try
            {
                var list = _quotationDetailKHRepo.GetAll().Where(x => x.QuotationKHID == id);
                return Ok(new
                {
                    status = 1,
                    data = list,
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
