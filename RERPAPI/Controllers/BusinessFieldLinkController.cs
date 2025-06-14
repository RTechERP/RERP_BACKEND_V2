using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessFieldLinkController : ControllerBase
    {
        BusinessFieldLinkRepo businessFieldLinkRepo = new BusinessFieldLinkRepo();
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                List<BusinessFieldLink> businessFieldLinks = businessFieldLinkRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = businessFieldLinks
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

        [HttpGet("customer")]
        public IActionResult GetBusinessFieldLinkByCustomerID(int customerID)
        {
            try
            {
                var businessFieldLink = SQLHelper<BusinessFieldLink>.FindByAttribute("CustomerID", customerID);
                if (businessFieldLink == null)
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "BusinessFieldLink not found"
                    });
                }
                return Ok(new
                {
                    status = 1,
                    data = businessFieldLink
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
    }
}
