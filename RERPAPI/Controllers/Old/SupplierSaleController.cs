using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.DocumentManager;
using ZXing;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierSaleController : ControllerBase
    {
        SupplierSaleRepo _supplierSaleRepo;
        EmployeeRepo _employeeRepo;
        public SupplierSaleController(
            SupplierSaleRepo supplierSaleRepo,
            EmployeeRepo employeeRepo
        )
        {
            _supplierSaleRepo = supplierSaleRepo;
            _employeeRepo = employeeRepo;
        }
        #region Get
        // Danh sách supplier
        [HttpGet("list-supplier-sale")]
        public async Task<IActionResult> getAll()
        {
            try
            {
                var supplierSale = _supplierSaleRepo.GetAll().OrderByDescending(x => x.ID);
                return Ok(ApiResponseFactory.Success(supplierSale, null));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("supplier-sale")]
        [RequiresPermission("N27,N33,N35,N1,N36")]
        public async Task<IActionResult> getSupplierSale(string? keyword, int page, int size)
        {
            try
            {
                var saleSupplier = SQLHelper<object>.ProcedureToList("spFindSupplierNCC",
                    new string[] { "@Find", "@PageNumber", "@PageSize" },
                    new object[] { keyword ?? "", page, size });

                var result = new
                {
                    data = SQLHelper<object>.GetListData(saleSupplier, 0),
                    totalPage = SQLHelper<object>.GetListData(saleSupplier, 1).FirstOrDefault().TotalPage ?? 1
                };

                return Ok(ApiResponseFactory.Success(result, null));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-supplier-sale")]
        //[RequiresPermission("N27,N33,N35,N1,N36")]
        public async Task<IActionResult> GetSupplierSale()
        {
            try
            {
                var saleSupplier = _supplierSaleRepo.GetAll(x => x.IsDeleted == false || x.IsDeleted == null);

                
                return Ok(ApiResponseFactory.Success(saleSupplier, null));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("supplier-sale-by-id")]
        public async Task<IActionResult> getsalesupplierbyid(int supplierID)
        {
            try
            {
                var data = _supplierSaleRepo.GetAll().FirstOrDefault(c => c.ID == supplierID);
                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        
        #endregion


        #region Method Post
        [HttpPost("supplier-sale")]
        [RequiresPermission("N27,N33,N35,N1")]
        public async Task<IActionResult> savesuppliersale([FromBody] SupplierSale supplierSale)
        {
            try
            {
                if (supplierSale != null && supplierSale.IsDeleted == true && supplierSale.ID > 0)
                {
                    SupplierSale sup = _supplierSaleRepo.GetByID(supplierSale.ID);
                    sup.IsDeleted = true;
                    await _supplierSaleRepo.UpdateAsync(sup);
                    return Ok(ApiResponseFactory.Success(null, null));
                }

                if (!_supplierSaleRepo.Validate(supplierSale, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                if (supplierSale.ID <= 0)
                {
                    if (supplierSale.EmployeeID == 0)
                    {
                        supplierSale.NVPhuTrach = "";
                    }
                    else
                    {
                        if (supplierSale.EmployeeID != null && supplierSale.EmployeeID > 0)
                        {
                            string NVPhuTrach = _employeeRepo.GetByID((int)supplierSale.EmployeeID).FullName;
                            supplierSale.NVPhuTrach = NVPhuTrach;
                        }
                    }
                    await _supplierSaleRepo.CreateAsync(supplierSale);
                }
                else
                {
                    if (supplierSale.EmployeeID != 0 && supplierSale.EmployeeID != null)
                    {
                        string NVPhuTrach = _employeeRepo.GetByID((int)supplierSale.EmployeeID).FullName;
                        supplierSale.NVPhuTrach = NVPhuTrach;
                    }

                    await _supplierSaleRepo.UpdateAsync(supplierSale);
                }

                return Ok(ApiResponseFactory.Success(supplierSale.ID, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        #endregion
    }
}