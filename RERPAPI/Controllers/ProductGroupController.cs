using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupController : ControllerBase
    {
        ProductGroupRepo _productGroupRepo = new ProductGroupRepo();
        #region Lấy tất cả nhóm sản phẩm
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<ProductGroup> productGroups = _productGroupRepo.GetAll(x=>x.IsVisible==true);

                return Ok(new
                {
                    status = 1,
                    data = productGroups
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
        #endregion
    }
}