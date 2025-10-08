using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;

namespace RERPAPI.Controllers.SaleWareHouseManagement
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
                    throw new Exception("Danh sách ID không hợp lệ");
                  
                }
                if (request.Type != 1 && request.Type != 2)
                {
                    throw new Exception("Giá trị type không hợp lệ");
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
                    throw new Exception("Không tìm thấy dữ liệu cho bất kỳ ID nào");
                }

                return Ok(ApiResponseFactory.Success(results, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                    throw new Exception("Dữ liệu không hợp lệ");
                }

                if (dto.type == 1)
                {
                    if (dto.billImportDetailSerialNumbers == null || !dto.billImportDetailSerialNumbers.Any())
                    {
                        throw new Exception("Danh sách nhập hóa đơn rỗng");
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
                        throw new Exception("Danh sách xuất hóa đơn rỗng");
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

                return Ok(ApiResponseFactory.Success(dto, "Xử lý thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
