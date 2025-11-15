using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSaleController : ControllerBase
    {
        private readonly ProductSaleRepo _productSaleRepo;
        private readonly ProductGroupRepo _productGroupRepo;
        public ProductSaleController(ProductSaleRepo productSaleRepo, ProductGroupRepo productGroupRepo)
        {
            _productSaleRepo = productSaleRepo;
            _productGroupRepo = productGroupRepo;
        }

        [HttpGet("get-all")]
        public IActionResult GetProductSales()
        {
            var listGroup = _productGroupRepo.GetAll().Select(x => x.ID).ToList();
            var ids = string.Join(",", listGroup);
            var productsales = SQLHelper<dynamic>.ProcedureToList("spGetProductSale", new string[] { "@IDgroup" }, new object[] { ids });
            var lstProductSales = SQLHelper<dynamic>.GetListData(productsales, 0);
            return Ok(new
            {
                data = lstProductSales,
                status = 1
            });
        }
    }
}