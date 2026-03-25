using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.Project.Procedure;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectInfoController : ControllerBase
    {
        private readonly ProjectInfoRepo _projectInfoRepo;
        private readonly ProjectInforEmployeeRepo _projectInforEmployee;
        public ProjectInfoController(ProjectInfoRepo projectInfoRepo, ProjectInforEmployeeRepo projectInforEmployee)
        {
            _projectInfoRepo = projectInfoRepo;
            _projectInforEmployee = projectInforEmployee;
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "projectId")] int projectID)
        {
            try
            {
                List<spGetProjectTask> projectTasks = SQLHelper<spGetProjectTask>.ProcedureToListModel("spGetProjectTask", new string[] { "@ProjectID" }, new object[] { projectID });
                return Ok(ApiResponseFactory.Success(projectTasks));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve project tasks."));
            }
        }

        //[HttpPost]
        //public async IActionResult SaveData([FromBody] ProjectInfoParam param)
        //{
        //    try
        //    {
        //        if(param == null)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Invalid project task data."));
        //        }
        //        if(param.ID <= 0)
        //        {
        //            var newProjectInfor = new ProjectInfo
        //            {
        //                Name = param.Name,
        //                Icon = param.Icon,
        //                Color = param.Color,
        //                GroupProject = param.GroupProject,
        //                Description = param.Description,
        //                StartDate = param.StartDate,
        //                EndDate = param.EndDate,
        //                Priority = param.Priority
        //            };
        //            var result = _projectInfoRepo.CreateAsync(newProjectInfor);
        //            if(result == null || newProjectInfor.ID <= 0)
        //            {
        //                return BadRequest(ApiResponseFactory.Fail(null, "Failed to create new project task."));
        //            }

        //            if(param.projectInforEmployees != null && param.projectInforEmployees.Count >= 0)
        //            {
        //                foreach (var item in param.projectInforEmployees)
        //                {
        //                    var newProjectInforEmployee = new ProjectInforEmployee
        //                    {
        //                        ProjectInfoID = newProjectInfor.ID,
        //                        EmployeeID = item
        //                    };
        //                    var empResult = _projectInforEmployee.CreateAsync(newProjectInforEmployee);
        //                    if (empResult == null)
        //                    {
        //                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to associate employees with the new project task."));
        //                    }
        //                }
        //            }
        //            return Ok(ApiResponseFactory.Success(newProjectInfor));
        //        }
        //        else
        //        {
        //            var existingProjectInfor = await _projectInfoRepo.GetByIDAsync(param.ID);
        //            if (existingProjectInfor == null || existingProjectInfor.ID <= 0)
        //            {
        //                return BadRequest(ApiResponseFactory.Fail(null, "Project task not found."));
        //            }
        //            existingProjectInfor.Name = param.Name;
        //            existingProjectInfor.Icon = param.Icon;
        //            existingProjectInfor.Color = param.Color;
        //            existingProjectInfor.GroupProject = param.GroupProject;
        //            existingProjectInfor.Description = param.Description;
        //            existingProjectInfor.StartDate = param.StartDate;
        //            existingProjectInfor.EndDate = param.EndDate;
        //            existingProjectInfor.Priority = param.Priority;
        //            var updateResult = await _projectInfoRepo.UpdateAsync(existingProjectInfor);

        //            if (updateResult == null || existingProjectInfor.ID <= 0)
        //            {
        //                return BadRequest(ApiResponseFactory.Fail(null, "Failed to update project task."));
        //            }

        //            var listExistingProductInforEmployees = _projectInforEmployee.GetAll(e => e.ProjectInfoID == existingProjectInfor.ID && (!e.IsDeleted ?? true));
        //            if (listExistingProductInforEmployees != null && listExistingProductInforEmployees.Count >= 0)
        //            {
                        
        //                if(param.projectInforEmployees == null || param.projectInforEmployees.Count <= 0)
        //                {
        //                    foreach (var item in listExistingProductInforEmployees)
        //                    {
        //                        item.IsDeleted = true;
        //                        var deleteResult = await _projectInforEmployee.UpdateAsync(item);
        //                        if (deleteResult == null)
        //                        {
        //                            return BadRequest(ApiResponseFactory.Fail(null, "Failed to remove existing employee associations."));
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    var listExistingEmployeeIDs = listExistingProductInforEmployees.Select(e => e.EmployeeID).ToList();
        //                    var listToDeleteEmployees = listExistingEmployeeIDs.Except(param.projectInforEmployees).ToList();
        //                    var listNewEmployees = param.projectInforEmployees.Except(listExistingEmployeeIDs).ToList();

        //                }
        //            }
        //            else
        //            {
        //                if(param.projectInforEmployees != null && param.projectInforEmployees.Count > 0)
        //                {
        //                    foreach (var item in param.projectInforEmployees)
        //                    {
        //                        var newProjectInforEmployee = new ProjectInforEmployee
        //                        {
        //                            ProjectInfoID = existingProjectInfor.ID,
        //                            EmployeeID = item
        //                        };
        //                        var empResult = _projectInforEmployee.CreateAsync(newProjectInforEmployee);
        //                        if (empResult == null)
        //                        {
        //                            return BadRequest(ApiResponseFactory.Fail(null, "Failed to associate employees with the new project task."));
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, "Failed to save project task."));
        //    }
        //}
    }
}
