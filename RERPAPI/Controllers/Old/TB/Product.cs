using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

using RERPAPI.Repo.GenericEntity.TB;
namespace RERPAPI.Controllers.Old.TB
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        CategoriesRepo _categoriesRepo = new CategoriesRepo();
        ProductsRepo _productsRepo = new ProductsRepo();
        //API getall Lấy danh sách danh mục sản phẩm
        [HttpGet("get-all-category")]
        public IActionResult GetAll()
        {
            try
            {
                List<Category> category = _categoriesRepo.GetAll();
                category = _categoriesRepo.GetAll()
                   .Where(x => x.IsDeleted == false)
                   .ToList();
                return Ok(new
                {
                    status = 1,
                    data = category
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
        //API getall Lấy danh sách detail của danh mục sản phẩm
        [HttpGet("get-detail-category")]
        public IActionResult GetDetailCategory(int id)
        {
            try
            {
                // id ở đây là CategoryId
                var products = _productsRepo.GetAll().Where(p => p.CategoryId == id).ToList();

                return Ok(new
                {
                    status = 1,
                    products
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

        //API Dùng để thêm sửa xóa danh mục sản phẩm
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] Category category)
        {
            try
            {
                if (category.Id <= 0) await _categoriesRepo.CreateAsync(category);
                else _categoriesRepo.UpdateAsync(category);

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công.",
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
