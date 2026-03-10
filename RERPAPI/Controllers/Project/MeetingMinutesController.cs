using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Duan.MeetingMinutes;

using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;
using RERPAPI.Repo.GenericEntity.MeetingMinutesRepo;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class MeetingMinutesController : ControllerBase
    {
        private readonly MeetingTypeRepo _meetingtype;
        private readonly ProjectManagerRepo _projectmanager;
        private readonly MeetingMinuteRepo __meetingMinutesRepo;
        private readonly MeetingMinutesDetailRepo _meetingMinutesDetailRepo;
        private readonly MeetingMinutesAttendanceRepo _meetingMinutesAttendanceRepo;
        private readonly ProjectHistoryProblemRepo _projectHistoryProblemRepo;
        private readonly MeetingMinutesFileRepo _meetingMinutesFileRepo;

        public MeetingMinutesController(
            MeetingTypeRepo meetingtype,
            ProjectManagerRepo projectmanager,
            MeetingMinuteRepo meetingMinutesRepo,
            MeetingMinutesDetailRepo meetingMinutesDetailRepo,
            MeetingMinutesAttendanceRepo meetingMinutesAttendanceRepo,
            ProjectHistoryProblemRepo projectHistoryProblemRepo,
            MeetingMinutesFileRepo meetingMinutesFileRepo
        )
        {
            _meetingtype = meetingtype;
            _projectmanager = projectmanager;
            __meetingMinutesRepo = meetingMinutesRepo;
            _meetingMinutesDetailRepo = meetingMinutesDetailRepo;
            _meetingMinutesAttendanceRepo = meetingMinutesAttendanceRepo;
            _projectHistoryProblemRepo = projectHistoryProblemRepo;
            _meetingMinutesFileRepo = meetingMinutesFileRepo;
        }

        [HttpGet("get-meeting-type")]
        public IActionResult GetMeetingType()
        {
            try
            {
                var meetingtype = _meetingtype.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = meetingtype
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

        [HttpPost("get-meeting-minutes")]
        public async Task<IActionResult> GetMeetingMinutes([FromBody] MeetingMinutesRequestParam request)
        {
            try
            {
                var param = new
                {
                    DateStart = request.DateStart.ToLocalTime().Date,
                    DateEnd = request.DateEnd.ToLocalTime().Date.AddDays(+1).AddSeconds(-1),
                    Keywords = request.Keywords,
                    MeetingTypeID = request.MeetingTypeID,
                };

                var meetingminutes = await SqlDapper<object>.ProcedureToListAsync("spGetMeetingMinutes", param);
                //var meetingminutes = SQLHelper<dynamic>.ProcedureToList("spGetMeetingMinutes",
                //    new string[] { "@DateStart", "@DateEnd", "@Keywords", "@MeetingTypeID" },
                //    new object[] { request.DateStart.ToLocalTime().Date, request.DateEnd.ToLocalTime().Date.AddDays(+1).AddSeconds(-1), request.Keywords, request.MeetingTypeID });
                return Ok(new
                {
                    status = 1,
                    //data = new
                    //{
                    //    asset = SQLHelper<object>.GetListData(meetingminutes, 0),
                    //    total = SQLHelper<object>.GetListData(meetingminutes, 1)
                    //}
                    data = meetingminutes

                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("get-meetingtype-groups")]
        public IActionResult GetMeetingTypeGroups()
        {
            try
            {
                var groups = _meetingtype.GetAll()
                    .GroupBy(mt => mt.GroupID)
                    .Select(g => new
                    {
                        GroupID = g.Key,
                        Rooms = g.Select(mt => new
                        {
                            mt.ID,
                            mt.TypeCode,
                            mt.TypeName
                        }).ToList()
                    })
                    .ToList();

                return Ok(new
                {
                    status = 1,
                    data = groups
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

        [HttpPost("get-employee")]

        public IActionResult GetEmployee([FromBody] EmployeeRequestParam employeerequest)
        {
            try
            {
             
                var employees = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status" },
                                                new object[] { 0});
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        asset = employees,
                     
                    }

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

        [HttpPost("get-user-team")]

        public IActionResult GetUserTeam([FromBody] UserTeamRequestParam userteamquest)
        {
            try
            {
                var userteam = SQLHelper<dynamic>.ProcedureToList("spGetUserTeamLink_New",
                    new string[] {"@UserTeamID", "@DepartmentID" },
                    new object[] {0, userteamquest.DepartmentID });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        asset = SQLHelper<dynamic>.GetListData(userteam, 0),
                        total = SQLHelper<dynamic>.GetListData(userteam, 1)
                    }

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

        [HttpGet("get-project-history-problem/{id}")]

        public IActionResult GetProjectProblem(int id)
        {
            try
            {
                var projectproblem = _projectHistoryProblemRepo.GetAll().Where(x => x.ProjectID == id).ToList();
                return Ok(new
                {
                    status = 1,
                    data = projectproblem
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

        [HttpGet("get-meeting-minutes/{id}")]
        public IActionResult getMeetingMinutes(int id)
        {
            try
            {
                MeetingMinute result = __meetingMinutesRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = result
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

        [HttpPost("get-meeting-minutes-details")]
        public IActionResult GetMeetingMinutesDetailsByID([FromBody] MeetingMinutesDetailsRequestParam detailrequest)
        {
            try
            {
                // Gọi stored procedure, trả về tất cả result set
                var allResults = SQLHelper<dynamic>.ProcedureToList(
                    "spGetMeetingMinutesDetailsByID",
                    new string[] { "@MeetingMinutesID" },
                    new object[] { detailrequest.MeetingMinutesID }
                );

                // Lấy từng result set
                var customerDetails = SQLHelper<dynamic>.GetListData(allResults, 0);       // Chi tiết khách hàng
                var employeeDetails = SQLHelper<dynamic>.GetListData(allResults, 2);       // nhân viên tham dự
                var employeeAttendance = SQLHelper<dynamic>.GetListData(allResults, 1);    // nội dung 
                var customerAttendance = SQLHelper<dynamic>.GetListData(allResults, 3);    // Khách hàng tham dự

                //nội dung nhân viên 
                var empContent = SQLHelper<dynamic>.GetListData(allResults, 1);
                //nội dung khách hàng
                var cusContent = SQLHelper<dynamic>.GetListData(allResults, 0);
                //nhân viên tham gia
                var empDetail = SQLHelper<dynamic>.GetListData(allResults, 2);
                //khách hàng tham gia 
                var cusDetail = SQLHelper<dynamic>.GetListData(allResults, 3);
                var file = SQLHelper<dynamic>.GetListData(allResults, 4);    // file

                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        //moi
                        empContent,
                        cusContent,
                        empDetail,
                        cusDetail,

                        //
                        customerDetails,
                        employeeDetails,
                        employeeAttendance,
                        customerAttendance,
                        file
                    }
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

        [HttpGet("get-projects")]

        public IActionResult GetDepartment()
        {
            try
            {
                var project = _projectmanager.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = project
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
        [HttpPost("save-data-meeting-minutes")]
        public async Task<IActionResult> SaveData([FromBody] MeetingMinutesDTO dto)
        {
            try
            {
                if (dto == null || dto.MeetingMinute == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                }

                int meetingMinutesID = 0;

                // Insert mới
                if (dto.MeetingMinute.ID <= 0)
                {
                    await __meetingMinutesRepo.CreateAsync(dto.MeetingMinute);
                    meetingMinutesID = dto.MeetingMinute.ID; // repo sẽ gán ID sau khi insert
                }
                // Update
                else
                {
                    __meetingMinutesRepo.Update(dto.MeetingMinute);
                    meetingMinutesID = dto.MeetingMinute.ID;
                }

                // Chi tiết biên bản (nhiều dòng)
                if (dto.MeetingMinutesDetail != null && dto.MeetingMinutesDetail.Any())
                {
                    foreach (var detail in dto.MeetingMinutesDetail)
                    {
                        detail.MeetingMinutesID = meetingMinutesID;

                        if (detail.ID <= 0)
                            await _meetingMinutesDetailRepo.CreateAsync(detail);
                        else
                            _meetingMinutesDetailRepo.Update(detail);
                    }
                }

                if (dto.DeletedMeetingMinutesDetails.Count > 0)
                {
                    foreach (var item in dto.DeletedMeetingMinutesDetails)
                    {
                        MeetingMinutesDetail meetingDetails = _meetingMinutesDetailRepo.GetByID(item);
                        meetingDetails.IsDeleted = true;
                        await _meetingMinutesDetailRepo.UpdateAsync(meetingDetails);

                    }
                }

                // Người tham dự (nhiều dòng)
                if (dto.MeetingMinutesAttendance != null && dto.MeetingMinutesAttendance.Any())
                {
                    foreach (var attendance in dto.MeetingMinutesAttendance)
                    {
                        attendance.MeetingMinutesID = meetingMinutesID;

                        if (attendance.ID <= 0)
                            await _meetingMinutesAttendanceRepo.CreateAsync(attendance);
                        else
                            _meetingMinutesAttendanceRepo.Update(attendance);
                    }
                }

                if (dto.DeletedMeetingMinutesAttendance.Count > 0)
                {
                    foreach (var item in dto.DeletedMeetingMinutesAttendance)
                    {
                        MeetingMinutesAttendance meetingAttendance = _meetingMinutesAttendanceRepo.GetByID(item);
                        meetingAttendance.IsDeleted = true;
                        await _meetingMinutesAttendanceRepo.UpdateAsync(meetingAttendance);

                    }
                }

                // Vấn đề lịch sử dự án (nhiều dòng)
                if (dto.ProjectHistoryProblem != null && dto.ProjectHistoryProblem.Any())
                {
                    foreach (var problem in dto.ProjectHistoryProblem)
                    {
                        problem.ProjectID = dto.MeetingMinute.ProjectID; ;

                        if (problem.ID <= 0)
                            await _projectHistoryProblemRepo.CreateAsync(problem);
                        else
                            _projectHistoryProblemRepo.Update(problem);
                    }
                }
                //lưu file 
                if (dto.MeetingMinutesFile != null && dto.MeetingMinutesFile.Any())
                {
                    foreach (var item in dto.MeetingMinutesFile)
                    {
                        item.MeetingMinutesID = meetingMinutesID;
                        if (item.ID <= 0)
                            await _meetingMinutesFileRepo.CreateAsync(item);
                        else
                            await _meetingMinutesFileRepo.UpdateAsync(item);
                    }
                }
                if (dto.DeletedFile != null && dto.DeletedFile.Any())
                {
                    foreach (var item in dto.DeletedFile)
                    {
                        var rs = _meetingMinutesFileRepo.GetByID(item);
                        rs.IsDeleted = true;
                        await _meetingMinutesFileRepo.UpdateAsync(rs);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công",
                    id = meetingMinutesID
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
        //private async Task SaveFiles(MeetingMinutesDTO dto, IFormFile[] files, int meetingMinutesID)
        //{
        //    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "meeting-minutes");
        //    Directory.CreateDirectory(uploadPath);

        //    var fileEntities = new List<MeetingMinutesFile>();

        //    foreach (var file in files)
        //    {
        //        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        //        var serverPath = Path.Combine(uploadPath, fileName);
        //        var relativePath = $"/uploads/meeting-minutes/{fileName}";

        //        using (var stream = new FileStream(serverPath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        fileEntities.Add(new MeetingMinutesFile
        //        {
        //            MeetingMinutesID = meetingMinutesID,
        //            FileName = file.FileName,
        //            OriginPath = file.FileName,
        //            ServerPath = relativePath,
        //            CreatedBy = "system", // hoặc lấy từ token
        //            CreatedDate = DateTime.Now,
        //            IsDeleted = false
        //        });
        //    }

        //    // Ghi DB
        //    foreach (var file in fileEntities)
        //    {
        //        await _meetingMinutesFileRepo.CreateAsync(file);
        //    }

        //    // Cập nhật danh sách file trong DTO (nếu cần trả về)
        //    dto.MeetingMinutesFile ??= new();
        //    dto.MeetingMinutesFile.AddRange(fileEntities);
        //}
        //private async Task DeleteFiles(MeetingMinutesDTO dto)
        //{
        //    if (dto.DeletedFile == null || !dto.DeletedFile.Any()) return;

        //    foreach (var fileId in dto.DeletedFile)
        //    {
        //        var file = _meetingMinutesFileRepo.GetByID(fileId);
        //        if (file != null)
        //        {
        //            // Xóa file vật lý
        //            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.ServerPath.TrimStart('/'));
        //            if (System.IO.File.Exists(fullPath))
        //            {
        //                System.IO.File.Delete(fullPath);
        //            }

        //            // Đánh dấu xóa mềm
        //            file.IsDeleted = true;
        //            file.UpdatedDate = DateTime.Now;
        //            file.UpdatedBy = "system";
        //            await _meetingMinutesFileRepo.UpdateAsync(file);
        //        }
        //    }
        //}

        [HttpPost("save-meeting-type")]
        public async Task<IActionResult> SaveData([FromBody] MeetingType meetingtype)
        {
            try
            {
                //Check trùng mã cuộc họp trong cùng một GroupID
                var existed = _meetingtype.GetAll()
                    .Any(x => x.TypeCode == meetingtype.TypeCode
                           && x.GroupID == meetingtype.GroupID   // check trong cùng nhóm
                           && x.ID != meetingtype.ID);           // loại trừ khi update

                if (existed)
                {
                    return Ok(new
                    {
                        status = 2,
                        message = "Mã cuộc họp đã tồn tại."
                    });
                }

                if (meetingtype.ID <= 0)
                {
                    await _meetingtype.CreateAsync(meetingtype);
                }
                else
                {
                    _meetingtype.Update(meetingtype);
                }

                return Ok(ApiResponseFactory.Success(meetingtype, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }






    }
}
