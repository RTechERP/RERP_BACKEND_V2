using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Dynamic;
using System.Text.RegularExpressions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsSaleController : ControllerBase
    {
        ProductGroupRepo productgroupRepo = new ProductGroupRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();
        WareHouseRepo WareHouseRepo = new WareHouseRepo();
        ProductsSaleRepo productsaleRepo = new ProductsSaleRepo();
        ProductGroupWareHouseRepo productgroupwarehouseRepo = new ProductGroupWareHouseRepo();

        #region hàm lấy dữ liệu nhóm vật tư
        [HttpGet("getdataproductgroup")]
        public IActionResult GetdataProductGroup()
        {
            try
            {

                List<ProductGroup> datepGroup = productgroupRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = datepGroup
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
        #endregion

        #region hàm nhóm vật tư theo id
        [HttpGet("getdataproductgroupbyid")]
        public IActionResult GetDataProductGroupByID(int id)
        {
            try
            {
                ProductGroup result = productgroupRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = result
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

        #region hàm lấy dữ liệu vật tư theo id, tên

        [HttpGet("getproductsale")]
        public IActionResult GetProductSale(int id, string? find, bool checkeedAll)
        {
            try
            {
                if (checkeedAll == true)
                {
                    id = 0;
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToDynamicLists(
                       "usp_LoadProductsale", new string[] { "@id", "@Find", "@IsDeleted" },
                    new object[] { id, find ?? "", false }
                   );
                List<dynamic> rs = result[0];

                return Ok(new
                {
                    status = 1,
                    data = rs
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
        #endregion

        #region hàm lấy dữ liệu nhân sự
        [HttpGet("getdataEmployee")]
        public IActionResult GetDataEmployee(int? status, int? departmamentID, string? keyword, int? id)
        {
            try
            {

                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetEmployee", new string[] { "@Status", "@DepartmentID", "@Keyword", "@ID" }, new object[] { status ?? 0, departmamentID ?? 0, keyword ?? "", id ?? 0 });
                List<dynamic> rs = result[0];
                return Ok(new
                {
                    status = 1,
                    data = rs
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
        #endregion

        #region hàm lấy dữ liệu kho
        [HttpGet("getdatawh")]
        public IActionResult GetDataWH()
        {
            try
            {
                List<Warehouse> warehouse = WareHouseRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = warehouse
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
        #endregion

        #region hàm lấy vật tư theo id nhóm
        [HttpGet("getProductbyidgroup")]
        public IActionResult getProductByGroup(int id)
        {
            try
            {
                List<ProductSale> rs = productsaleRepo.GetAll().Where(x => x.ProductGroupID == id).ToList();
                return Ok(new
                {
                    status = 1,
                    data = rs
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
        #endregion

        #region hàm lấy dữ liệu bảng kho nhóm sản phẩm (bảng trung gian)
        [HttpGet("getdatproductgroupwh")]
        public IActionResult GetDataProductGroupWareHours(int? warehouseID, int? productgroupID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToDynamicLists(
                    "spGetProductGroupWarehouse",
                    new string[] { "@WarehouseID", "@ProductGroupID" }, 
                    new object[] { warehouseID ?? 0, productgroupID ?? 0 }
                );

                if (result == null || result.Count == 0 || result[0] == null)
                {
                    return Ok(new
                    {
                        status = 1,
                        data = new List<dynamic>()
                    });
                }

                return Ok(new
                {
                    status = 1,
                    data = result[0]
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
        #endregion

        #region hàm thêm , cập nhật bảng nhóm vật tư
        [HttpPost("savedatagroup")]
        [HttpPost("savedatagroup")]
        public async Task<IActionResult> saveDataGroup([FromBody] ProductGoupDTO dto)
        {
            try
            {
                if (dto.Productgroup.ID <= 0)
                {
                    int newId = await productgroupRepo.CreateAsync(dto.Productgroup);
                    dto.ProductgroupWarehouse.ProductGroupID = newId;
                    await productgroupwarehouseRepo.CreateAsync(dto.ProductgroupWarehouse);
                }
                else
                {
                    productgroupRepo.UpdateFieldsByID(dto.Productgroup.ID, dto.Productgroup);

                    var existing = await productgroupwarehouseRepo.FindByGroupAndWarehouseAsync(dto.Productgroup.ID, (int)dto.ProductgroupWarehouse.WarehouseID);


                    if (existing != null)
                    {
                        dto.ProductgroupWarehouse.ID = existing.ID;
                        await productgroupwarehouseRepo.UpdateAsync(dto.ProductgroupWarehouse);
                    }
                    else
                    {
                        dto.ProductgroupWarehouse.ProductGroupID = dto.Productgroup.ID;
                        await productgroupwarehouseRepo.CreateAsync(dto.ProductgroupWarehouse);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    data = dto
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

            // Tránh CS0161:
            return BadRequest("Unexpected error.");
        }

        #endregion

        #region hàm xóa (isVisible=false) nhóm vật tư
        [HttpPost("deleteproductgroup")]
        public IActionResult DeleteProductGroup([FromBody] ProductGroup data)
        {
            try
            {

                // Kiểm tra xem có sản phẩm nào thuộc nhóm không
                bool hasProduct = productsaleRepo.GetAll().Any(x => x.ProductGroupID == data.ID);

                if (hasProduct)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Nhóm sản phẩm đang chứa thiết bị, không thể xóa."
                    });
                }
                var item = productgroupRepo.GetByID(data.ID);
                if (item == null)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Không tìm thấy nhóm sản phẩm để xóa."
                    });
                }

                item.IsVisible = false;
                productgroupRepo.Update(item); // sửa ở đây

                return Ok(new
                {
                    status = 1,
                    message = "Xóa nhóm sản phẩm thành công."
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
        #endregion

    
    }
}
