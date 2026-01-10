using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryProjectProductSaleLinkController : ControllerBase
    {
        UserTeamRepo _userTeamRepo;
        ProductGroupRepo _productGroupRepo;
        ProductSaleRepo _productSaleRepo;

        InventoryProjectProductSaleLinkRepo _inventoryProjectProductSaleLinkRepo;
        public InventoryProjectProductSaleLinkController(
            UserTeamRepo userTeamRepo,
            ProductGroupRepo productGroupRepo,
            ProductSaleRepo productSaleRepo,
            InventoryProjectProductSaleLinkRepo inventoryProjectProductSaleLinkRepo
        )
        {
            _userTeamRepo = userTeamRepo;
            _productGroupRepo = productGroupRepo;
            _productSaleRepo = productSaleRepo;
            _inventoryProjectProductSaleLinkRepo = inventoryProjectProductSaleLinkRepo;
        }

        [HttpGet("product-group")]
        public IActionResult ProductGroup()
        {
            try
            {
                var data = _productGroupRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("data")]
        public IActionResult LoadData(int productGroupID, string? keyWord)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetInventoryProjectProductSaleLink",
                    new string[] { "@ProductSaleID", "@EmployeeID", "@Keyword", "@ProductGroupID" },
                    new object[] { 0, 0, keyWord ?? "", productGroupID });
                var dt = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("data-detail")]
        public IActionResult LoadDataDetail()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProductSaleWithGroup",
                    new string[] { "@Keyword" },
                    new object[] { "" });
                var dt = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-inventory")]
        public async Task<IActionResult> SaveData([FromBody] List<int> lsDeleteds)
        {
            try
            {
                foreach (int id in lsDeleteds)
                {
                    InventoryProjectProductSaleLink model = _inventoryProjectProductSaleLinkRepo.GetByID(id);
                    if (model != null && id > 0)
                    {
                        model.IsDeleted = true;
                        await _inventoryProjectProductSaleLinkRepo.UpdateAsync(model);
                    }
                }
                return Ok(ApiResponseFactory.Success("", "Cập nhật thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("add-inventory")]
        public async Task<IActionResult> addInventory([FromBody] List<int> lsIds)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (int id in lsIds)
                {
                    ProductSale productSale = _productSaleRepo.GetByID(id);
                    if (productSale != null && id > 0)
                    {
                        InventoryProjectProductSaleLink model = new InventoryProjectProductSaleLink();
                        model.ProductSaleID = productSale.ID;
                        model.EmployeeID = currentUser.EmployeeID;
                        await _inventoryProjectProductSaleLinkRepo.CreateAsync(model);
                    }
                }
                return Ok(ApiResponseFactory.Success("", "Cập nhật thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}
