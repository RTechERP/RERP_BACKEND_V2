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
        SupplierRepo _supplierRepo;
        SupplierSaleContactRepo _supplierSaleContactRepo;
        SupplierSaleRepo _supplierSaleRepo;
        RulePayRepo _rulePayRepo;
        TaxCompanyRepo _taxCompanyRepo;
        EmployeeRepo _employeeRepo;
        DepartmentRepo _departmentRepo;
        DocumentFileRepo _documentFileRepo;
        DocumentTypeRepo _documentTypeRepo;
        DocumentRepo _documentRepo;
        ConfigSystemRepo _configSystemRepo;


        public SupplierSaleController(
            SupplierRepo supplierRepo,
            SupplierSaleContactRepo supplierSaleContactRepo,
            SupplierSaleRepo supplierSaleRepo,
            RulePayRepo rulePayRepo,
            TaxCompanyRepo taxCompanyRepo,
            EmployeeRepo employeeRepo,
            DepartmentRepo departmentRepo,
            DocumentFileRepo documentFileRepo,
            DocumentTypeRepo documentTypeRepo,
            DocumentRepo documentRepo,
            ConfigSystemRepo configSystemRepo
        )
        {
            _supplierRepo = supplierRepo;
            _supplierSaleContactRepo = supplierSaleContactRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _rulePayRepo = rulePayRepo;
            _taxCompanyRepo = taxCompanyRepo;
            _employeeRepo = employeeRepo;
            _departmentRepo = departmentRepo;
            _documentFileRepo = documentFileRepo;
            _documentTypeRepo = documentTypeRepo;
            _documentRepo = documentRepo;
            _configSystemRepo = configSystemRepo;
        }
        #region Get
        // Danh sách supplier
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

        [HttpGet("supplier-sale-contact")]
        [RequiresPermission("N27,N33,N52,N53,N35,N1")]
        public async Task<IActionResult> getSupplierSaleContact(int supplierID)
        {
            try
            {
                var data = _supplierSaleContactRepo.GetAll()
                    .Where(c => c.SupplierID == supplierID)
                    .OrderByDescending(c => c.ID);

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

        [HttpPost("supplier-sale-contact")]
        [RequiresPermission("N27,N33,N35,N1")]
        public async Task<IActionResult> savesuppliersalecontact([FromBody] SupplierSaleContact supplierSaleContact)
        {
            try
            {
                if (supplierSaleContact.ID <= 0)
                {
                    await _supplierSaleContactRepo.CreateAsync(supplierSaleContact);
                }
                else
                {
                    _supplierSaleContactRepo.Update(supplierSaleContact);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        #endregion
    }
}