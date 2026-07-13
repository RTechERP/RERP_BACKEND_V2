using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Project
{
    /// <summary>
    /// Controller quản lý các bước/công đoạn theo Gate dự án (ProjectGateStep)
    /// và bảng link phòng ban (ProjectGateDepartment)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateStepController : ControllerBase
    {
        private readonly ProjectGateStepRepo _stepRepo;
        private readonly ProjectGateDepartmentRepo _deptRepo;
        private readonly ProjectGateStepPositionRepo _positionLinkRepo;
        private readonly ProjectGateRepo _gateRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly PositionInternalRepo _positionInternalRepo;
        private readonly ProjectGateStepCheckListRepo _checkListRepo;
        private readonly ProjectGateStepTemplateRepo _templateRepo; 

        public ProjectGateStepController(
            ProjectGateStepRepo stepRepo,
            ProjectGateDepartmentRepo deptRepo,
            ProjectGateStepPositionRepo positionLinkRepo,
            ProjectGateRepo gateRepo,
            DepartmentRepo departmentRepo,
            PositionInternalRepo positionInternalRepo,
            ProjectGateStepCheckListRepo checkListRepo,
            ProjectGateStepTemplateRepo templateRepo)
        {
            _stepRepo = stepRepo;
            _deptRepo = deptRepo;
            _positionLinkRepo = positionLinkRepo;
            _gateRepo = gateRepo;
            _departmentRepo = departmentRepo;
            _positionInternalRepo = positionInternalRepo;
            _checkListRepo = checkListRepo;
            _templateRepo = templateRepo;
        }

        /// <summary>
        /// Lấy tất cả các bước/công đoạn, join phòng ban thành 1 cột DepartmentNames
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] int? gateId = null, [FromQuery] int? departmentId = null)
        {
            try
            {
                var data = await SqlDapper<ProjectGateStepDTO>.ProcedureToListTAsync(
                    "spGetProjectGateSteps",
                    new { GateID = gateId, DepartmentID = departmentId }
                );

                foreach (var step in data)
                {
                    if (!string.IsNullOrEmpty(step.DepartmentIDsStr))
                    {
                        step.DepartmentIDs = step.DepartmentIDsStr.Split(',').Select(int.Parse).ToList();
                    }

                    if (!string.IsNullOrEmpty(step.PositionIDsStr))
                    {
                        step.PositionIDs = step.PositionIDsStr.Split(',').Select(int.Parse).ToList();
                    }

                    if (!string.IsNullOrEmpty(step.ChecklistsJson))
                    {
                        step.CheckLists = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectGateStepCheckListDTO>>(step.ChecklistsJson) ?? new List<ProjectGateStepCheckListDTO>();
                    }
                }

                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách bước/công đoạn theo ProjectGateID
        /// </summary>
        [HttpGet("get-by-gate/{gateId}")]
        public async Task<IActionResult> GetByGate(int gateId)
        {
            try
            {
                var data = await SqlDapper<ProjectGateStepDTO>.ProcedureToListTAsync(
                    "spGetProjectGateSteps",
                    new { GateID = gateId, DepartmentID = (int?)null }
                );

                foreach (var step in data)
                {
                    if (!string.IsNullOrEmpty(step.DepartmentIDsStr))
                    {
                        step.DepartmentIDs = step.DepartmentIDsStr.Split(',').Select(int.Parse).ToList();
                    }

                    if (!string.IsNullOrEmpty(step.PositionIDsStr))
                    {
                        step.PositionIDs = step.PositionIDsStr.Split(',').Select(int.Parse).ToList();
                    }

                    if (!string.IsNullOrEmpty(step.ChecklistsJson))
                    {
                        step.CheckLists = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectGateStepCheckListDTO>>(step.ChecklistsJson) ?? new List<ProjectGateStepCheckListDTO>();
                    }
                }

                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy produce: danh sách Gate, Department, ChucVu để FE dùng cho dropdown/multiselect
        /// </summary>
        [HttpGet("get-produce")]
        public IActionResult GetProduce()
        {
            try
            {
                var gates = _gateRepo.GetAll().OrderBy(x => x.STT ?? int.MaxValue)
                    .Select(g => new { g.ID, g.GateCode, g.GateName, g.Type })
                    .ToList();

                var departments = _departmentRepo.GetAll(x => x.IsDeleted != true)
                    .OrderBy(x => x.STT)
                    .Select(d => new { d.ID, d.Code, d.Name })
                    .ToList();

                var positions = _positionInternalRepo.GetAll(x => x.IsDeleted != true)
                    .OrderBy(x => x.PriorityOrder)
                    .Select(p => new { p.ID, p.Code, p.Name })
                    .ToList();

                var templates = _templateRepo.GetAll()
                    .OrderBy(t => t.Code)
                    .Select(t => new { t.ID, t.Code, t.Name })
                    .ToList();

                return Ok(ApiResponseFactory.Success(new { gates, departments, positions, templates }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Thêm mới hoặc cập nhật bước/công đoạn + đồng bộ danh sách phòng ban
        /// </summary>
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectGateStepDTO> dtos)
        {
            try
            {
                if (dtos == null || dtos.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ"));

                foreach (var dto in dtos)
                {
                    // Map DTO → Entity
                    var step = new ProjectGateStep
                    {
                        ID = dto.ID,
                        ProjectGateID = dto.ProjectGateID,
                        Content = dto.Content,
                        ChucVuID = dto.ChucVuID,
                        TT = dto.TT,
                        SortOrder = dto.SortOrder,
                        IsRepeat = dto.IsRepeat,
                        ProjectGateStepTemplateID = dto.ProjectGateStepTemplateID
                    };

                    int stepId;

                    if (dto.ID <= 0)
                    {
                        // Thêm mới
                        await _stepRepo.CreateAsync(step);
                        // Lấy ID vừa tạo
                        stepId = step.ID;
                    }
                    else
                    {
                        // Cập nhật
                        await _stepRepo.UpdateAsync(step);
                        stepId = dto.ID;
                    }

                    // Đồng bộ phòng ban: xóa cũ, insert mới
                    var oldLinks = _deptRepo.GetAll(x => x.ProjectGateStepID == stepId);
                    if (oldLinks.Count > 0)
                    {
                        await _deptRepo.DeleteRangeAsync(oldLinks);
                    }

                    if (dto.DepartmentIDs != null && dto.DepartmentIDs.Count > 0)
                    {
                        var newLinks = dto.DepartmentIDs.Select(deptId => new ProjectGateDepartment
                        {
                            DepartmentID = deptId,
                            ProjectGateStepID = stepId
                        }).ToList();

                        await _deptRepo.CreateRangeAsync(newLinks);
                    }

                    // Đồng bộ chức vụ: xóa cũ, insert mới
                    var oldPositionLinks = _positionLinkRepo.GetAll(x => x.ProjectGateStepID == stepId);
                    if (oldPositionLinks.Count > 0)
                    {
                        await _positionLinkRepo.DeleteRangeAsync(oldPositionLinks);
                    }

                    if (dto.PositionIDs != null && dto.PositionIDs.Count > 0)
                    {
                        var newPositionLinks = dto.PositionIDs.Select(posId => new ProjectGateStepPosition
                        {
                            ChucVuID = posId,
                            ProjectGateStepID = stepId
                        }).ToList();

                        await _positionLinkRepo.CreateRangeAsync(newPositionLinks);
                    }

                    // Đồng bộ checklist: xóa cũ, insert mới
                    var oldChecklists = _checkListRepo.GetAll(x => x.ProjectGateStepID == stepId);
                    if (oldChecklists.Count > 0)
                    {
                        await _checkListRepo.DeleteRangeAsync(oldChecklists);
                    }

                    if (dto.CheckLists != null && dto.CheckLists.Count > 0)
                    {
                        var newChecklists = dto.CheckLists.Select(c => new ProjectGateStepCheckList
                        {
                            ProjectGateStepID = stepId,
                            Type = c.Type,
                            PathFolder = c.PathFolder,
                            Description = c.Description
                        }).ToList();

                        await _checkListRepo.CreateRangeAsync(newChecklists);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Xóa danh sách bước/công đoạn theo ID
        /// </summary>
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bản ghi để xóa"));

                // Xóa step
                var steps = _stepRepo.GetAll(x => ids.Contains(x.ID));
                if (steps.Count > 0)
                    await _stepRepo.DeleteRangeAsync(steps);

                return Ok(ApiResponseFactory.Success(ids, "Xóa dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}
