using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.Duan.MeetingMinutes;
using RERPAPI.Model.Param.MakerTraining;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.MakerTrainingFirm;
//using Microsoft.EntityFrameworkCore;


namespace RERPAPI.Controllers.TrainingFirm
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MakerTrainingController : ControllerBase
    {
        DepartmentRepo _tsDepartment;
        FirmRepo _firmRepo;
        MakerTrainingRepo _makerTrainingRepo;
        MakerTrainingEmployeeLinkRepo _makertrainingEmployeeLinkRepo;
        MakerTrainingDocumentRepo _makertrainingDocumentRepo;
        MakerTrainingTypeRepo _makertrainingTypeRepo;

        public MakerTrainingController(DepartmentRepo tsDepartment, FirmRepo firmRepo, MakerTrainingRepo makerTrainingRepo, MakerTrainingEmployeeLinkRepo makertrainingEmployeeLinkRepo, MakerTrainingDocumentRepo makertrainingDocumentRepo, MakerTrainingTypeRepo makertrainingTypeRepo)
        {
            _tsDepartment = tsDepartment;
            _firmRepo = firmRepo;
            _makerTrainingRepo = makerTrainingRepo;
            _makertrainingEmployeeLinkRepo = makertrainingEmployeeLinkRepo;
            _makertrainingDocumentRepo = makertrainingDocumentRepo;
            _makertrainingTypeRepo = makertrainingTypeRepo;
        }

        [RequiresPermission("N85,N32")]
        [HttpPost("get-maker-training")]
        public async Task<IActionResult> GetMakerTraining([FromBody] MakerTrainingRequestParam request)
        {
            try
            {
                var param = new
                {
                    request.DepartmentID ,
                    request.MakerTrainingTypeID,
                    request.FirmID,
                    request.DateStart,
                    request.DateEnd,
                    request.Keyword
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetMakerTraining", param);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex) {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-maker-training-data")]
        public async Task<IActionResult> GetMakerTrainingData([FromBody] MakerTrainingDataRequestParam makerTrainingData)
        {
            try
            {
                var param = new { makerTrainingData.MakerTrainingID };

                var (makerTrainingEmployeeLink, makerTrainingDocument) = await SqlDapper<object>.QueryMultipleAsync<dynamic, dynamic>("spGetMakerTrainingData", param);

                return Ok(ApiResponseFactory.Success(new
                {
                    MakerTrainingEmployeeLink = makerTrainingEmployeeLink,
                    MakerTrainingDocument = makerTrainingDocument
                }, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-employee-maker-training")]

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

        [HttpGet("get-departments")]

        public IActionResult GetDepartment()
        {
            try
            {
                var datadepartment = _tsDepartment.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = datadepartment
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

        [HttpGet("get-firms")]

        public IActionResult GetFirm()
        {
            try
            {
                var firm = _firmRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = firm
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

        [HttpGet("get-maker-training-type")]

        public IActionResult GetMakerTrainingType()
        {
            try
            {
                List<MakerTrainingType> trainingtype = _makertrainingTypeRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = trainingtype
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

        [HttpGet("get-maker-training-document/{id}")]
        public IActionResult GetMakerTrainingDocument (int id)
        {
            try
            {
                List<MakerTrainingDocument> document = _makertrainingDocumentRepo.GetAll().Where(x => x.MakerTrainingID == id && x.IsDeleted == false).ToList();
                return Ok(new
                {
                    status = 1,
                    data = document
                });

            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString(),
                });
            }

        }

        [HttpGet("get-maker-training/{id}")]
        public IActionResult getMakerTraining(int id)
        {
            try
            {
                MakerTraining result = _makerTrainingRepo.GetByID(id);
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

        [HttpPost("save-data-maker-training")]
        public async Task<IActionResult> SaveData([FromBody] MakerTrainingDTO dto)
        {
            try
            {
                if (dto == null || dto.MakerTraining == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                }

                int makerTrainingID = 0;

                // Master
                if (dto.MakerTraining.ID <= 0)
                {
                    var maxStt = _makerTrainingRepo.GetAll()
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    dto.MakerTraining.STT = maxStt + 1;


                    
                    await _makerTrainingRepo.CreateAsync(dto.MakerTraining);
                    makerTrainingID = dto.MakerTraining.ID; // repo sẽ gán ID sau khi insert
                }
                else
                {
                    await _makerTrainingRepo.UpdateAsync(dto.MakerTraining);
                    makerTrainingID = dto.MakerTraining.ID;
                }

                // Nhân viên training
                if (dto.MakerTrainingEmployeeLink != null && dto.MakerTrainingEmployeeLink.Any())
                {
                    var maxSttEmployee = _makertrainingEmployeeLinkRepo.GetAll()
                        .Where(x => x.MakerTrainingID == makerTrainingID) 
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttEmployeeCounter = maxSttEmployee;
                    foreach (var employeetraining in dto.MakerTrainingEmployeeLink)
                    {
                        employeetraining.MakerTrainingID = makerTrainingID;

                        if (employeetraining.ID <= 0)
                        {
                          sttEmployeeCounter++;
                          employeetraining.STT = sttEmployeeCounter;
                        await _makertrainingEmployeeLinkRepo.CreateAsync(employeetraining);
                        }    
                         
                        else
                           await _makertrainingEmployeeLinkRepo.UpdateAsync(employeetraining);
                    }
                }
                    if(dto.DeletedEmployees.Count > 0)
                    {
                    foreach( var item in dto.DeletedEmployees)
                    {
                        MakerTrainingEmployeeLink employeemodel = _makertrainingEmployeeLinkRepo.GetByID(item);
                        employeemodel.IsDeleted = true;
                        await _makertrainingEmployeeLinkRepo.UpdateAsync(employeemodel);
                    }
                    }

                // File training
                if (dto.MakerTrainingDocument != null && dto.MakerTrainingDocument.Any())
                {
                    var maxSttFile = _makertrainingDocumentRepo.GetAll()
                        .Where(x => x.MakerTrainingID == makerTrainingID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                     int sttFileCounter = maxSttFile;
                    foreach (var fileTraining in dto.MakerTrainingDocument)
                    {
                        fileTraining.MakerTrainingID = makerTrainingID;

                        if (fileTraining.ID <= 0)
                        {
                            sttFileCounter++;
                            fileTraining.STT = sttFileCounter;
                            fileTraining.CreatedDate = DateTime.Now;
                            await _makertrainingDocumentRepo.CreateAsync(fileTraining);
                        }
                            
                        else
                            _makertrainingDocumentRepo.Update(fileTraining);
                    }
                }
                if (dto.DeletedFileIds.Count > 0)
                {
                    foreach (var item in dto.DeletedFileIds)
                    {
                        MakerTrainingDocument model = _makertrainingDocumentRepo.GetByID(item);
                        model.IsDeleted = true;
                        await _makertrainingDocumentRepo.UpdateAsync(model);
                    }
                }
                if (dto.DeletedEmployees != null && dto.DeletedEmployees.Any())
                {
                    foreach (var item in dto.DeletedEmployees.Where(x => x > 0))
                    {
                        var employeemodel = _makertrainingEmployeeLinkRepo.GetByID(item);
                        if (employeemodel != null)
                        {
                            employeemodel.IsDeleted = true;
                            await _makertrainingEmployeeLinkRepo.UpdateAsync(employeemodel);
                        }
                    }
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công",
                    id = makerTrainingID,
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

        [HttpPost("save-makertraining-type")]
        public async Task<IActionResult> SaveData([FromBody] MakerTrainingType trainingtype)
        {
            try
            {
                // Check trùng mã
                var existed = _makertrainingTypeRepo.GetAll()
                    .Any(x => x.TypeCode == trainingtype.TypeCode
                           && x.ID != trainingtype.ID);

                if (existed)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Mã cuộc họp đã tồn tại."
                    });
                }

                if (trainingtype.ID <= 0)
                {
                    var maxSTT = _makertrainingTypeRepo.GetAll().Any()
                        ? _makertrainingTypeRepo.GetAll().Max(x => x.STT)
                         : 0;
                    trainingtype.STT = maxSTT + 1;
                    await _makertrainingTypeRepo.CreateAsync(trainingtype);
                }
                else
                {
                    _makertrainingTypeRepo.Update(trainingtype);
                }

                return Ok(new
                {
                    status = 1,
                    message = "" +
                    "Thêm thành công.",
                  
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

        //[RequiresPermission("N13,N1,N27,N31")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (ids == null || ids.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn mục training để xóa"));
                foreach (var item in ids)
                {

                    var project = _makerTrainingRepo.GetByID(item);
                    project.IsDeleted = true;
                    await _makerTrainingRepo.UpdateAsync(project);

                }
                return Ok(ApiResponseFactory.Success(ids, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }






    }
}
