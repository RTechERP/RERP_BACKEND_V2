using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionController : ControllerBase
    {
        private readonly PositionContractRepo positionContractRepo;
        private readonly PositionInternalRepo positionInternalRepo;
        public PositionController(PositionContractRepo positionContractRepo, PositionInternalRepo positionInternalRepo)
        {
            this.positionContractRepo = positionContractRepo;
            this.positionInternalRepo = positionInternalRepo;
        }
        [HttpGet("position-contract")]
        public IActionResult GetPositionContract()
        {
            var result = positionContractRepo.GetAll().OrderBy(x => x.PriorityOrder);
            return Ok(result);
        }
        [HttpGet("position-internal")]
        public IActionResult GetPositionInternal()
        {
            var result = positionInternalRepo.GetAll().OrderBy(x => x.PriorityOrder);
            return Ok(result);
        }

        [HttpPost("position-contract")]
        public async Task<IActionResult> SavePositionContract([FromBody] EmployeeChucVuHD employeeChucVuHD)
        {
            try
            {
                List<EmployeeChucVuHD> employeeChucVuHDs = positionContractRepo.GetAll();


                if (employeeChucVuHDs.Any(x => (x.Name == employeeChucVuHD.Name || x.Code == employeeChucVuHD.Code) && x.ID != employeeChucVuHD.ID))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Tên hoặc mã chức vụ hợp đồng đã tồn tại"
                    });
                }
                if (employeeChucVuHD.ID <= 0)
                {
                    employeeChucVuHD.CreatedDate = DateTime.Now;
                    employeeChucVuHD.PriorityOrder = 0;
                    await positionContractRepo.CreateAsync(employeeChucVuHD);
                }
                else
                {
                    employeeChucVuHD.UpdatedDate = DateTime.Now;
                    await positionContractRepo.UpdateAsync(employeeChucVuHD);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = "Lỗi khi lưu chức vụ hợp đồng",
                    error = ex.Message
                });
            }
        }


        [HttpPost("position-internal")]
        public async Task<IActionResult> SavePositionInternal([FromBody] EmployeeChucVu employeeChucVu)
        {
            try
            {
                List<EmployeeChucVu> employeeChucVus = positionInternalRepo.GetAll();
                if (employeeChucVus.Any(x => (x.Name == employeeChucVu.Name || x.Code == employeeChucVu.Code) && x.ID != employeeChucVu.ID))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Tên hoặc mã chức vụ nội bộ đã tồn tại"
                    });
                }
                if (employeeChucVu.ID <= 0)
                {
                    employeeChucVu.CreatedDate = DateTime.Now;
                    employeeChucVu.PriorityOrder = 0;
                    await positionInternalRepo.CreateAsync(employeeChucVu);
                }
                else
                {
                    employeeChucVu.UpdatedDate = DateTime.Now;
                    await positionInternalRepo.UpdateAsync(employeeChucVu);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = "Lỗi khi lưu chức vụ nội bộ",
                    error = ex.Message
                });
            }
        }

        //[HttpDelete("position-contract/{positionContractId}")]
        //public async Task<IActionResult> DeletePositionContract(int positionContractId)
        //{
        //    try
        //    {
        //        var position = positionContractRepo.GetByID(positionContractId);
        //        if (position == null)
        //        {
        //            return NotFound(new
        //            {
        //                status = 0,
        //                message = "Chức vụ hợp đồng không tồn tại"
        //            });
        //        }
        //        await positionContractRepo.DeleteAsync(positionContractId);
        //        return Ok(new
        //        {
        //            status = 1,
        //            message = "Xóa chức vụ hợp đồng thành công"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = "Lỗi khi xóa chức vụ hợp đồng",
        //            error = ex.Message
        //        });
        //    }
        //}

        //[HttpDelete("position-internal/{positionInternalId}")]
        //public async Task<IActionResult> DeletePositionInternal(int positionInternalId)
        //{
        //    try
        //    {
        //        var position = positionInternalRepo.GetByID(positionInternalId);
        //        if (position == null)
        //        {
        //            return NotFound(new
        //            {
        //                status = 0,
        //                message = "Chức vụ nội bộ không tồn tại"
        //            });
        //        }
        //        await positionInternalRepo.DeleteAsync(positionInternalId);
        //        return Ok(new
        //        {
        //            status = 1,
        //            message = "Xóa chức vụ nội bộ thành công"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = "Lỗi khi xóa chức vụ nội bộ",
        //            error = ex.Message
        //        });
        //    }   
        //}
    }
}
