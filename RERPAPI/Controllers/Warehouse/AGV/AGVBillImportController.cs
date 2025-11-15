using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.Warehouses.AGV;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Warehouses.AGV;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Warehouse.AGV
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AGVBillImportController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AGVBillImportRepo _billImportRepo;
        private readonly AGVBillImportDetailRepo _detailRepo;

        public AGVBillImportController(IConfiguration configuration, AGVBillImportRepo billImportRepo, AGVBillImportDetailRepo detailRepo)
        {
            _configuration = configuration;
            _billImportRepo = billImportRepo;
            _detailRepo = detailRepo;
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var billImports = _billImportRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(billImports, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var billImport = _billImportRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(billImport, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] AGVBillImportDTO billImport)
        {
            try
            {
                var validate = _billImportRepo.Validate(billImport);
                if (validate.status == 0) return BadRequest(validate);

                if (billImport.ID <= 0)
                {

                    billImport.BillCode = _billImportRepo.GetBillCode(Convert.ToInt32(billImport.BillType));
                    await _billImportRepo.CreateAsync(billImport);
                }
                else await _billImportRepo.UpdateAsync(billImport);

                foreach (var item in billImport.AGVBillImportDetails)
                {
                    if (item.ID <= 0)
                    {
                        item.AGVBillImportID = billImport.ID;
                        await _detailRepo.CreateAsync(item);
                    }
                    else await _detailRepo.UpdateAsync(item);
                }


                return Ok(ApiResponseFactory.Success(billImport, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
