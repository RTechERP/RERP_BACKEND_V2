using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Dynamic;
using System.Text.RegularExpressions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSaleController : ControllerBase
    {
        ProductGroupRepo productgroupRepo = new ProductGroupRepo();
        ProductsSaleRepo productsaleRepo = new ProductsSaleRepo();
        InventoryRepo inventoryRepo = new InventoryRepo();

        //api ngày 12/06/2025

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

        #region hàm lấy dữ liệu productsale theo id
        [HttpGet("{id}")]
        public IActionResult getProductSaleByID(int id)
        {
            try
            {
                var rs = productsaleRepo.GetByID(id);

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

        #region hàm sinh mã nội bộ (productnewcode) 
        private string GenerateProductNewCode(int productGroupId)
        {
            // Bước 1: Lấy mã nhóm sản phẩm từ ID
            var productGroup = productgroupRepo.GetByID(productGroupId);
            if (productGroup == null || string.IsNullOrWhiteSpace(productGroup.ProductGroupID))
                return string.Empty;

            string productGroupCode = productGroup.ProductGroupID.Trim();

            // Bước 2: Lấy danh sách sản phẩm thuộc nhóm này
            var listProducts = productsaleRepo.GetAll()
                .Where(x => x.ProductGroupID == productGroupId &&
                            !string.IsNullOrWhiteSpace(x.ProductNewCode) &&
                            x.ProductNewCode.StartsWith(productGroupCode))
                .ToList();

            // Bước 3: Tính STT cao nhất đang dùng
            var listNewCodes = listProducts.Select(x => new
            {
                STT = int.TryParse(x.ProductNewCode.Substring(productGroupCode.Length), out int num) ? num : 0
            });

            int nextSTT = listNewCodes.Any() ? listNewCodes.Max(x => x.STT) + 1 : 1;
            string numberCodeText = nextSTT.ToString().PadLeft(9 - productGroupCode.Length, '0');

            return productGroupCode + numberCodeText;
        }
        #endregion
        //done+ update ngày 14/06 : xóa nhiều bản ghi 
        #region hàm thêm, sửa, xóa productSale
        [HttpPost("savedataproductsale")]
        public async Task<IActionResult> SaveDataProductSale([FromBody] List<ProductsSaleDTO> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    if (dto.ProductSale.ID <= 0)
                    {
                        // Tạo mới
                        if (string.IsNullOrWhiteSpace(dto.ProductSale.ProductNewCode))
                        {
                            dto.ProductSale.ProductNewCode = GenerateProductNewCode((int)dto.ProductSale.ProductGroupID);
                        }

                        int newId = await productsaleRepo.CreateAsync(dto.ProductSale);
                        dto.Inventory.ProductSaleID = newId;
                        dto.Inventory.WarehouseID = 1;
                        await inventoryRepo.CreateAsync(dto.Inventory);
                    }
                    else
                    {
                        // Cập nhật
                        productsaleRepo.UpdateFieldsByID(dto.ProductSale.ID, dto.ProductSale);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = "Xử lý thành công",
                    data = dtos
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



