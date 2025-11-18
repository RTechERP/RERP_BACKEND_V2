using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.Warehouses.AGV;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Warehouses.AGV;

namespace RERPAPI.Controllers.Warehouse.AGV
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AGVBillExportController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AGVBillExportRepo _billExportRepo;
        private readonly AGVBillExportDetailRepo _detailRepo;

        public AGVBillExportController(IConfiguration configuration, AGVBillExportRepo billExportRepo, AGVBillExportDetailRepo detailRepo)
        {
            _configuration = configuration;
            _billExportRepo = billExportRepo;
            _detailRepo = detailRepo;
        }


        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var billExports = _billExportRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(billExports, ""));
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
                var billExport = _billExportRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(billExport, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] AGVBillExportDTO billExport)
        {
            try
            {
                var validate = _billExportRepo.Validate(billExport);
                if (validate.status == 0) return BadRequest(validate);

                if (billExport.ID <= 0)
                {

                    billExport.BillCode = _billExportRepo.GetBillCode(Convert.ToInt32(billExport.BillType));
                    await _billExportRepo.CreateAsync(billExport);
                }
                else await _billExportRepo.UpdateAsync(billExport);

                foreach (var item in billExport.AGVBillExportDetails)
                {
                    if (item.ID <= 0)
                    {
                        item.AGVBillExportID = billExport.ID;
                        await _detailRepo.CreateAsync(item);
                    }
                    else await _detailRepo.UpdateAsync(item);
                }


                return Ok(ApiResponseFactory.Success(billExport, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
