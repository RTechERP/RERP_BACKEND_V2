using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupController : ControllerBase
    {
        ProductGroupRepo _productgroupRepo = new ProductGroupRepo();
        ProductGroupWareHouseRepo _productgroupwarehouseRepo = new ProductGroupWareHouseRepo();

        [HttpGet("")]
        public IActionResult getProductGroup(bool isvisible = true, string warehousecode="")
        {
            try
            {
                //update 17/07/25 them truong hop warehousecode=HCM
                List<ProductGroup> data;
                List<int> excludedIds = new List<int> { 73, 74, 75, 76, 77 };

                if (warehousecode == "HN")
                {
                    //update isvible ngày 13/06/2025
                    data = _productgroupRepo.GetAll().Where(p => p.IsVisible == isvisible || !isvisible).ToList();
                }
                else
                {
                    //update 17/07/25 them truong hop warehousecode=HCM
                    data = _productgroupRepo.GetAll().Where(p => (p.IsVisible == isvisible || !isvisible) && !excludedIds.Contains(p.ID)).ToList();
                }

                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("{id}")]
        public IActionResult getProductGroupByID(int id)
        {
            try
            {
                ProductGroup result = _productgroupRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //update 14/06 fix khi xoa tu them 1 ban ghi bang productsalewarehouse
      
        [HttpPost("save-data")]
        public async Task<IActionResult> saveProductGroup([FromBody] ProductGoupDTO dto)
        {
            try
            {
                //TN.Binh update 19/10/25
                if (!CheckProductGroupCode(dto))
                {
                    return Ok(new { status = 0, message = $"Mã nhóm [{dto.Productgroup.ProductGroupID}] đã tồn tại!" });
                }
                //end update 
                if (dto.Productgroup.ID <= 0)
                {
                    int newId = await _productgroupRepo.CreateAsynC(dto.Productgroup);
                    dto.ProductgroupWarehouse.ProductGroupID = newId;
                    await _productgroupwarehouseRepo.CreateAsync(dto.ProductgroupWarehouse);
                }
                else
                {
                    _productgroupRepo.Update(dto.Productgroup);

                    // Nếu không gửi ProductgroupWarehouse thì bỏ qua phần cập nhật dưới đây
                    if (dto.ProductgroupWarehouse != null)
                    {
                        var existing = await _productgroupwarehouseRepo.FindByGroupAndWarehouseAsync(dto.Productgroup.ID, (int)dto.ProductgroupWarehouse.WarehouseID);

                        if (existing != null)
                        {
                            existing.EmployeeID = dto.ProductgroupWarehouse.EmployeeID;
                            existing.UpdatedBy = dto.ProductgroupWarehouse.UpdatedBy;
                            existing.UpdatedDate = DateTime.Now;

                            await _productgroupwarehouseRepo.UpdateAsync(existing);
                        }
                        else
                        {
                            dto.ProductgroupWarehouse.ProductGroupID = dto.Productgroup.ID;
                            await _productgroupwarehouseRepo.CreateAsync(dto.ProductgroupWarehouse);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(dto, "Xử lý dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //TN.Binh update 19/10/25
        #region check trùng mã sản phẩm khi thêm, sửa nhóm vật tư
        private bool CheckProductGroupCode(ProductGoupDTO dto)
        {
            bool check = true;
            var exists = _productgroupRepo.GetAll()
                .Where(x => x.ProductGroupID == dto.Productgroup.ProductGroupID
                            && x.ID != dto.Productgroup.ID).ToList();
            if (exists.Count > 0) check = false;
            return check;
        }
        //end update
        #endregion
    }
}
