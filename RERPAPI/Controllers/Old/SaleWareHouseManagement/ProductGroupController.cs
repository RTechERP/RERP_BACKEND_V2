using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupController : ControllerBase
    {
        private readonly ProductGroupRepo _productgroupRepo;
        private readonly ProductGroupLinkRepo _productGroupLinkRepo;
        private readonly ProductGroupWareHouseRepo _productgroupwarehouseRepo;
        public ProductGroupController(ProductGroupRepo productgroupRepo, ProductGroupWareHouseRepo productgroupwarehouseRepo, ProductGroupLinkRepo productGroupLinkRepo)
        {
            _productgroupRepo = productgroupRepo;
            _productgroupwarehouseRepo = productgroupwarehouseRepo;
            _productGroupLinkRepo = productGroupLinkRepo;
        }

        [HttpGet("")]
        public IActionResult getProductGroup(bool isvisible = true, string warehousecode = "HN")
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProductGroups", ["@Isvisible", "@WarehouseCode"], [isvisible, warehousecode]);
                var data = SQLHelper<ProductGroup>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-productgroup-purchase")]
        public IActionResult getProductGroupPurchase(bool isvisible = true, string warehousecode = "")
        {
            try
            {
                var data = _productgroupRepo.GetAll(x => x.IsVisible == true);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<ProductGroup> productGroups = _productgroupRepo.GetAll(x => x.IsVisible == true);

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
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                //TN.Binh update 19/10/25
                if (!CheckProductGroupCode(dto))
                {
                    //return Ok(new { status = 0, message = $"Mã nhóm [{dto.Productgroup.ProductGroupID}] đã tồn tại!" });
                }
                //end update 
                if (dto.Productgroup.ID <= 0)
                {
                    int newId = await _productgroupRepo.CreateAsynC(dto.Productgroup);
                    dto.ProductgroupWarehouse.ProductGroupID = newId;
                    await _productgroupwarehouseRepo.CreateAsync(dto.ProductgroupWarehouse);

                    if(dto.ProductgroupWarehouse.WarehouseID > 0)
                    {
                        ProductGroupLink model = new ProductGroupLink
                        {
                            WarehouseID = dto.ProductgroupWarehouse.WarehouseID,
                            ProductGroupID = newId,
                            IsDeleted = false,
                            Createdby = currentUser.LoginName,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = currentUser.LoginName,
                            UpdatedDate = DateTime.Now
                        };

                        await _productGroupLinkRepo.CreateAsync(model);
                    }
                    
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
            var exists = _productgroupRepo.GetAll(x => x.ProductGroupID == dto.Productgroup.ProductGroupID
                            && x.ID != dto.Productgroup.ID).ToList();
            if (exists.Count > 0) check = false;
            return check;
        }
        //end update
        #endregion


        #region Lấy danh sách nhóm sản phẩm mới 

        [HttpGet("product-group-new")]
        public async Task<IActionResult> getProductGroupNew(bool isVisible, bool isDeleted, int warehouseId)
        {
            try
            {
                var param = new
                {
                    WarehouseID = warehouseId,
                    IsDeleted = isDeleted,
                    IsVisible = isVisible
                };

                var (allGroups, groupInWarehouse) =
                    await SqlDapper<object>.QueryMultipleAsync<dynamic, dynamic>(
                        "spGetProductGroups_Test", param);

                return Ok(ApiResponseFactory.Success(new
                {
                    data = allGroups,
                    data1 = groupInWarehouse
                }, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("visible-product-group")]
        public async Task<IActionResult> visibleProductGroup([FromBody] List<ProductGroupLink> data)
        {
            try
            {
                List<ProductGroupLink> checkList = _productGroupLinkRepo.GetAll();
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (ProductGroupLink item in data)
                {
                    ProductGroupLink model = checkList.
                        FirstOrDefault(x => x.WarehouseID == item.WarehouseID && x.ProductGroupID == item.ProductGroupID) ??
                        new ProductGroupLink();

                    model.WarehouseID = item.WarehouseID;
                    model.ProductGroupID = item.ProductGroupID;
                    model.IsDeleted = item.IsDeleted;

                    if (model.ID > 0)
                    {
                        model.UpdatedBy = currentUser.LoginName;
                        model.UpdatedDate = DateTime.Now;
                        await _productGroupLinkRepo.UpdateAsync(model);
                    }
                    else
                    {
                        model.Createdby = currentUser.FullName;
                        model.CreatedDate = DateTime.Now;
                        model.UpdatedBy = currentUser.LoginName;
                        model.UpdatedDate = DateTime.Now;
                        await _productGroupLinkRepo.CreateAsync(model);
                    }
                }


                return Ok(ApiResponseFactory.Success(null, "Xử lý dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        #endregion
    }
}
