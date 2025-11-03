using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Duan.MeetingMinutes;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;

namespace RERPAPI.Controllers.Duan.MeetingMinutes
{
    [Route("api/[controller]")]
    [ApiController]

    public class MeetingMinutesController : ControllerBase
    {
       MeetingTypeRepo _meetingtype = new MeetingTypeRepo();
       ProjectManagerRepo _projectmanager = new ProjectManagerRepo();
        MeetingMinuteRepo __meetingMinutesRepo = new MeetingMinuteRepo();
        MeetingMinutesDetailRepo _meetingMinutesDetailRepo = new MeetingMinutesDetailRepo();
        MeetingMinutesAttendanceRepo _meetingMinutesAttendanceRepo = new MeetingMinutesAttendanceRepo();
        ProjectHistoryProblemRepo _projectHistoryProblemRepo = new ProjectHistoryProblemRepo();
       

        [HttpGet("get-meeting-type")]
        public IActionResult GetMeetingType() {
            try
            {
                var meetingtype = _meetingtype.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = meetingtype
                });
            }
            catch (Exception ex) { 
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });

            }
        }

        [HttpPost("get-meeting-minutes")]
        public IActionResult GetMeetingMinutes([FromBody] MeetingMinutesRequestParam request)
        {
            try
            {
                var meetingminutes = SQLHelper<dynamic>.ProcedureToList("spGetMeetingMinutes",
                    new string[] { "@DateStart", "@DateEnd", "@Keywords", "@MeetingTypeID" },
                    new object[] { request.DateStart, request.DateEnd, request.Keywords, request.MeetingTypeID });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        asset = SQLHelper<dynamic>.GetListData(meetingminutes, 0),
                        total = SQLHelper<dynamic>.GetListData(meetingminutes, 1)
                    }

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
                var employee = SQLHelper<dynamic>.ProcedureToList("spGetEmployee",
                    new string[] { "@Status" },
                    new object[] { employeerequest.Status });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        asset = SQLHelper<dynamic>.GetListData(employee, 0),
                        total = SQLHelper<dynamic>.GetListData(employee, 1)
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
                var userteam = SQLHelper<dynamic>.ProcedureToList("spGetUserTeam",
                    new string[] { "@DepartmentID" },
                    new object[] { userteamquest.DepartmentID });
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

                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        customerDetails,
                        employeeDetails,
                        employeeAttendance,
                        customerAttendance
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

                if(dto.DeletedMeetingMinutesAttendance.Count > 0)
                {
                    foreach(var item in dto.DeletedMeetingMinutesAttendance)
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

        [HttpPost("save-meeting-type")]
        public async Task<IActionResult> SaveData([FromBody] MeetingType meetingtype)
        {
            try
            {
                // Check trùng mã cuộc họp trong cùng một GroupID
                var existed = _meetingtype.GetAll()
                    .Any(x => x.TypeCode == meetingtype.TypeCode
                           && x.GroupID == meetingtype.GroupID   // check trong cùng nhóm
                           && x.ID != meetingtype.ID);           // loại trừ khi update

                if (existed)
                {
                    return Ok(new
                    {
                        status = 0,
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

                return Ok(new
                {
                    status = 1,
                    message = "" +
                    "Thêm thành công."
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






    }
}
