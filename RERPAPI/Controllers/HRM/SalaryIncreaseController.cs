using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalaryIncreaseController : ControllerBase
    {
        private readonly SalaryIncreaseRepo _salaryIncreaseRepo;
        private readonly SalaryIncreaseDetailRepo _salaryIncreaseDetailRepo;
        private readonly CurrentUser _currentUser;

        public SalaryIncreaseController(
            SalaryIncreaseRepo salaryIncreaseRepo,
            SalaryIncreaseDetailRepo salaryIncreaseDetailRepo,
            CurrentUser currentUser)
        {
            _salaryIncreaseRepo = salaryIncreaseRepo;
            _salaryIncreaseDetailRepo = salaryIncreaseDetailRepo;
            _currentUser = currentUser;
        }

        public class SalaryIncreaseSearchParam
        {
            public string? Keyword { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        [HttpPost("search-master")]
        public IActionResult SearchMaster([FromBody] SalaryIncreaseSearchParam param)
        {
            try
            {
                DateTime? sd = null;
                DateTime? ed = null;
                if (param.StartDate.HasValue)
                {
                    sd = param.StartDate.Value.ToLocalTime().Date;
                }
                if (param.EndDate.HasValue)
                {
                    ed = param.EndDate.Value.ToLocalTime().Date.AddDays(1).AddSeconds(-1);
                }

                string procedureName = "spGetSalaryIncreaseMaster";
                string[] paramNames = new string[] { "@Keyword", "@StartDate", "@EndDate" };
                object[] paramValues = new object[] {
                    param.Keyword ?? "",
                    (object)sd ?? DBNull.Value,
                    (object)ed ?? DBNull.Value
                };

                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var result = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(result, "Lấy danh sách đợt tăng lương thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-master")]
        public async Task<IActionResult> SaveMaster([FromBody] SalaryIncrease dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }

                if (dto.ID <= 0)
                {
                    await _salaryIncreaseRepo.CreateAsync(dto);
                }
                else
                {
                    var entity = _salaryIncreaseRepo.GetByID(dto.ID);
                    if (entity != null)
                    {
                        entity.Code = dto.Code;
                        entity.Name = dto.Name;
                        entity.EffectiveDate = dto.EffectiveDate;
                        entity.MonthFrom = dto.MonthFrom;
                        entity.MonthTo = dto.MonthTo;
                        await _salaryIncreaseRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(dto, "Lưu đợt tăng lương thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-master")]
        public async Task<IActionResult> DeleteMaster([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bản ghi để xóa"));
                }

                foreach (var id in ids)
                {
                    var entity = await _salaryIncreaseRepo.GetByIDAsync(id);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        await _salaryIncreaseRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa đợt tăng lương thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class SalaryIncreaseDetailParam
        {
            public int SalaryIncreaseID { get; set; }
            public string? Keyword { get; set; }
        }

        [HttpPost("search-detail")]
        public IActionResult SearchDetail([FromBody] SalaryIncreaseDetailParam param)
        {
            try
            {
                string procedureName = "spGetSalaryIncreaseDetail";
                string[] paramNames = new string[] { "@SalaryIncreaseID", "@Keyword" };
                object[] paramValues = new object[] {
                    param.SalaryIncreaseID,
                    param.Keyword ?? ""
                };

                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var result = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(result, "Lấy danh sách chi tiết nhân viên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-detail")]
        public async Task<IActionResult> SaveDetail([FromBody] SalaryIncreaseDetail dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }

                if (dto.ID <= 0)
                {
                    await _salaryIncreaseDetailRepo.CreateAsync(dto);
                }
                else
                {
                    var entity = _salaryIncreaseDetailRepo.GetByID(dto.ID);
                    if (entity != null)
                    {
                        entity.EmployeeID = dto.EmployeeID;
                        entity.EmailTBP = dto.EmailTBP;
                        entity.PreviousBaseSalary = dto.PreviousBaseSalary;
                        entity.CurrentBaseSalary = dto.CurrentBaseSalary;
                        entity.IsSend = dto.IsSend;
                        await _salaryIncreaseDetailRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(dto, "Lưu chi tiết nhân viên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-detail")]
        public async Task<IActionResult> DeleteDetail([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bản ghi để xóa"));
                }

                foreach (var id in ids)
                {
                    var entity = await _salaryIncreaseDetailRepo.GetByIDAsync(id);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        await _salaryIncreaseDetailRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa chi tiết nhân viên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
