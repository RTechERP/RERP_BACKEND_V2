using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillImportDetailSerialNumberController : ControllerBase
    {
        private readonly BillImportDetailSerialNumberRepo _billImportDetailSerialNumberRepo;

        public BillImportDetailSerialNumberController(BillImportDetailSerialNumberRepo billImportDetailSerialNumberRepo)
        {
            _billImportDetailSerialNumberRepo = billImportDetailSerialNumberRepo;
        }
        [HttpGet("{id}")]
        public IActionResult getDataByID(int id)
        {
            try
            {
                var result = _billImportDetailSerialNumberRepo.GetByID(id);

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu chi tiết theo id hành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveDataBillExportDetailSerialNumber([FromBody] BillDetailSerialNumberDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new Exception("Dữ liệu không hợp lệ");
                }

                if (dto.billImportDetailSerialNumbers == null || !dto.billImportDetailSerialNumbers.Any())
                {
                    throw new Exception("Danh sách serial phiếu nhập rỗng");
                }

                foreach (var item in dto.billImportDetailSerialNumbers)
                {
                    if (item.ID <= 0)
                    {
                        _billImportDetailSerialNumberRepo.Create(item);
                    }
                    else
                    {
                        _billImportDetailSerialNumberRepo.Update(item);
                    }
                }

                return Ok(ApiResponseFactory.Success(dto.billImportDetailSerialNumbers, "Lưu dữ liệu serial phiếu nhập thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
