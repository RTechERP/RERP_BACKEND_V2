using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
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
        [RequiresPermission("N2,N1")]
        public IActionResult GetPositionContract()
        {
            var result = positionContractRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.PriorityOrder);
            return Ok(result);
        }
        [HttpGet("position-internal")]
        [RequiresPermission("N2,N1")]
        public IActionResult GetPositionInternal()
        {
            var result = positionInternalRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.PriorityOrder);
            return Ok(result);
        }

        [HttpPost("position-contract")]
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> SavePositionContract([FromBody] EmployeeChucVuHD employeeChucVuHD)
        {
            try
            {
                List<EmployeeChucVuHD> employeeChucVuHDs = positionContractRepo.GetAll();

                if(employeeChucVuHD.IsDeleted == false)
                {
                    if (employeeChucVuHDs.Any(x => (
                    x.Name.ToLower().Trim() == employeeChucVuHD.Name.ToLower().Trim()
                    || x.Code.ToLower().Trim() == employeeChucVuHD.Code.ToLower().Trim()) 
                    && x.ID != employeeChucVuHD.ID 
                    && x.IsDeleted != true))
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = "Tên hoặc mã chức vụ hợp đồng đã tồn tại"
                        });
                    }
                }
                if (employeeChucVuHD.ID <= 0)
                {
                    employeeChucVuHD.Name = employeeChucVuHD.Name.Trim();
                    employeeChucVuHD.Code = employeeChucVuHD.Code.Trim();
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
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> SavePositionInternal([FromBody] EmployeeChucVu employeeChucVu)
        {
            try
            {
                List<EmployeeChucVu> employeeChucVus = positionInternalRepo.GetAll();
                if(employeeChucVu.IsDeleted == false)
                {
                    if (employeeChucVus.Any(x => (
                    x.Name.ToLower().Trim() == employeeChucVu.Name.ToLower().Trim()
                     || x.Code.ToLower().Trim() == employeeChucVu.Code.ToLower().Trim()) 
                     && x.ID != employeeChucVu.ID
                     && x.IsDeleted != true
                     ))
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = "Tên hoặc mã chức vụ nội bộ đã tồn tại"
                        });
                    }
                }
                
                if (employeeChucVu.ID <= 0)
                {
                    employeeChucVu.CreatedDate = DateTime.Now;
                    employeeChucVu.PriorityOrder = 0;
                    employeeChucVu.Name = employeeChucVu.Name.Trim();
                    employeeChucVu.Code = employeeChucVu.Code.Trim();
                    await positionInternalRepo.CreateAsync(employeeChucVu);
                }
                else
                {
                    employeeChucVu.UpdatedDate = DateTime.Now;
                    await positionInternalRepo.UpdateAsync(employeeChucVu);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("change-status-position-contract")]
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> changeStatusPositionContract([FromBody] List<EmployeeChucVuHD> employeeChucVuHDs)
        {
            try
            {
                if(employeeChucVuHDs.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Không có chức vụ được chọn!"));
                foreach (EmployeeChucVuHD item in employeeChucVuHDs)
                {
                    await positionContractRepo.UpdateAsync(item);
                }
                return Ok(ApiResponseFactory.Success(null, "Đã thay đổi trạng thái"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("change-status-position-internal")]
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> changeStatusPositionInternal([FromBody] List<EmployeeChucVu> employeeChucVus)
        {
            try
            {
                if (employeeChucVus.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Không có chức vụ được chọn!"));
                foreach (EmployeeChucVu item in employeeChucVus)
                {
                    await positionInternalRepo.UpdateAsync(item);
                }
                return Ok(ApiResponseFactory.Success(null, "Đã thay đổi trạng thái"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
