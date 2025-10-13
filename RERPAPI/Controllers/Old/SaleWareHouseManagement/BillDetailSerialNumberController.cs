using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDetailSerialNumberController : ControllerBase
    {
        BillExportDetailSerialNumberRepo _billExportDetailSerialNumberRepo = new BillExportDetailSerialNumberRepo();
        BillImportDetailSerialNumberRepo _billImportDetailSerialNumberRepo = new BillImportDetailSerialNumberRepo();

        [HttpPost("get-by-ids")]
        public async Task<IActionResult> getDataByIDs([FromBody] GetByIdsRequest request)
        {
            try
            {
                if (request.Ids == null || !request.Ids.Any())
                {
                    return BadRequest(new { status = 0, message = "Danh sách ID không hợp lệ" });
                }
                if (request.Type != 1 && request.Type != 2)
                {
                    return BadRequest(new { status = 0, message = "Giá trị type không hợp lệ" });
                }

                var results = new List<object>();
                foreach (var id in request.Ids)
                {
                    object data;
                    if (request.Type == 1)
                    {
                        data = _billImportDetailSerialNumberRepo.GetByID(id);
                    }
                    else
                    {
                        data =  _billExportDetailSerialNumberRepo.GetByID(id);
                    }

                    if (data != null)
                    {
                        results.Add(data);
                    }
                }

                if (!results.Any())
                {
                    return NotFound(new { status = 0, message = "Không tìm thấy dữ liệu cho bất kỳ ID nào" });
                }

                return Ok(new
                {
                    status = 1,
                    data = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message
                });
            }
        }

        public class GetByIdsRequest
        {
            public List<int> Ids { get; set; }
            public int Type { get; set; }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveDataBillExportDetailSerialNumber([FromBody] BillDetailSerialNumberDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                }

                if (dto.type == 1)
                {
                    if (dto.billImportDetailSerialNumbers == null || !dto.billImportDetailSerialNumbers.Any())
                    {
                        return BadRequest(new { status = 0, message = "Danh sách nhập hóa đơn rỗng" });
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
                }
                else
                {
                    if (dto.billExportDetailSerialNumbers == null || !dto.billExportDetailSerialNumbers.Any())
                    {
                        return BadRequest(new { status = 0, message = "Danh sách xuất hóa đơn rỗng" });
                    }

                    foreach (var item in dto.billExportDetailSerialNumbers)
                    {
                        if (item.ID <= 0)
                        {
                             _billExportDetailSerialNumberRepo.Create(item);
                        }
                        else
                        {
                             _billExportDetailSerialNumberRepo.Update(item);
                        }
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = "Xử lý thành công",
                    data = dto
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
    }
}
