using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIPositionEmployeeController : ControllerBase
    {
        private readonly DepartmentRepo _departmentRepo;
        private readonly KPISessionRepo _kpiSessionRepo;
        private readonly KPIPositionRepo _kpiPositionRepo;
        private readonly CurrentUser _currentUser;
        private readonly KPIPositionEmployeeRepo _kpiPositionEmployeeRepo;
        private readonly ProjectTypeRepo _projectTypeRepo;
        private readonly KPIPositionTypeRepo _kpiPositionTypeRepo;
        public KPIPositionEmployeeController(DepartmentRepo departmentRepo, KPISessionRepo kpiSession, KPIPositionRepo kpiPositionRepo, CurrentUser currentUser, KPIPositionEmployeeRepo kpiPositionEmployeeRepo, ProjectTypeRepo projectTypeRepo, KPIPositionTypeRepo kpiPositionTypeRepo)
        {
            _departmentRepo = departmentRepo;
            _kpiSessionRepo = kpiSession;
            _kpiPositionRepo = kpiPositionRepo;
            _currentUser = currentUser;
            _kpiPositionEmployeeRepo = kpiPositionEmployeeRepo;
            _projectTypeRepo = projectTypeRepo;
            _kpiPositionTypeRepo = kpiPositionTypeRepo;
        }

        [HttpGet("get-data")]
        public IActionResult GetData(int kpiSessionId)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetKPIPosition",
                                new string[] { "@KPISessionID" },
                                new object[] { kpiSessionId });
                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-detail")]
        public IActionResult GetDetail(int positionId)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeByKPIPositionID",
                                new string[] { "@KPIPositionID"},
                                new object[] { positionId });
                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-position-type")]
        public IActionResult GetPositionType(int kpiSessionID)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetPoistionTypeByKPISessionID",
                                new string[] { "@KPISessionID" },
                                new object[] { kpiSessionID });
                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var data = _departmentRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-session")]
        public IActionResult GetKPISession(int departmentId)
        {
            try
            {
                var data = _kpiSessionRepo.GetAll(x => x.IsDeleted == false && x.DepartmentID == departmentId).OrderByDescending(x => x.ID).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-position")]
        public IActionResult DeletePosition(int positionID)
        {
            try
            {
                var data = _kpiPositionRepo.GetByID(positionID);
                data.IsDeleted= true;
                _kpiPositionRepo.Update(data);
                return Ok(ApiResponseFactory.Success(null, "Xóa vị trí thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-employee")]
        public IActionResult DeleteEmployee(List<DeleteEmployeeRequest> dto)
        {
            try
            {
                bool isAdmin = (_currentUser.IsAdmin == true && _currentUser.EmployeeID <= 0) || _currentUser.EmployeeID == 55; //C Ngân
                foreach (var item in dto)
                {
                    if (item.DepartmentID != _currentUser.DepartmentID && isAdmin)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể xóa nhân viên của phòng ban khác"));
                    }     
                }

                foreach (var item in dto)
                {
                    if(item.KPIPositionEmployeeID == null || item.KPIPositionEmployeeID <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "KPIPositionEmployeeID không hợp lệ"));
                    }
                    _kpiPositionEmployeeRepo.Delete(item.KPIPositionEmployeeID ?? 0);
                }
                return Ok(ApiResponseFactory.Success(null, "Xóa vị trí thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-position")]
        public async Task<IActionResult> SaveDataPosition([FromBody] KPIPosition model)
        {
            try
            { 
                if(string.IsNullOrWhiteSpace(model.PositionCode))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập mã chức vụ"));
                }
                if (string.IsNullOrWhiteSpace(model.PositionName))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập tên chức vụ"));
                }
                if (model.KPISessionID == null || model.KPISessionID <= 0 )
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn kỳ đánh giá"));
                }
                if (model.TypePosition == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn loại đánh giá"));
                }
                var listDuplicate = _kpiPositionRepo.GetAll(x => x.PositionCode == model.PositionCode && x.ID != model.ID && model.IsDeleted == false && x.TypePosition == model.TypePosition && x.KPISessionID == model.KPISessionID);

                if (listDuplicate.Count > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã chức vụ [{model.PositionCode}] đã được sử dụng!"));
                }    

                if (model.ID > 0)
                {
                    await _kpiPositionRepo.UpdateAsync(model);
                }
                else
                {
                    await _kpiPositionRepo.CreateAsync(model);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-position-employee-detail")]
        public IActionResult GetPositionEmployeeDetail(int departmentId, int kpiSessionId)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetAllEmployeeByKPIPositionID",
                                new string[] { "@KPISessionID", "@DepartmentID" },
                                new object[] { kpiSessionId, departmentId });
                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-position-employee")]
        public async Task<IActionResult> SavePositionEmployee([FromBody] SavePositionEmployeeRequest model)
        {
            try
            {
                foreach(int id in model.listDel)
                {
                    await _kpiPositionEmployeeRepo.DeleteAsync(id);
                }    
                foreach(int id in model.listInsert)
                {
                    KPIPositionEmployee newModel = new KPIPositionEmployee()
                    {
                        KPIPosiotionID = model.KPIPositionID,
                        EmployeeID = id
                    };
                    await _kpiPositionEmployeeRepo.CreateAsync(newModel);
                }    
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        //KPIPositionTypeDetail
        [HttpGet("get-project-types")]
        public IActionResult GetProjectTypes(int departmentId, int kpiSessionId)
        {
            try
            {
                var data = _projectTypeRepo.GetAll().OrderBy(x => x.ID).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-kpi-position-type")]
        public IActionResult SaveKPIPositionType(KPIPositionType model)
        {
            try
            {
                if(model.ID > 0)
                {
                    _kpiPositionTypeRepo.Update(model);
                }
                else
                {
                    _kpiPositionTypeRepo.Create(model);
                }
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("copy-position-employee")]
        public IActionResult CopyPositionEmployee(int kpiSessionFrom, int kpiSessionTo)
        {
            try
            {
                if (kpiSessionFrom <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn kỳ đánh giá muốn copy"));
                }
                if (kpiSessionTo <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn kỳ đánh giá muốn copy đến"));
                }

                //Xóa dữ liệu cũ
                List<KPIPosition> listPositionOld = _kpiPositionRepo.GetAll(x => x.KPISessionID == kpiSessionTo && x.IsDeleted == false);

                foreach(var item in listPositionOld)
                {
                    item.IsDeleted = true;
                    _kpiPositionRepo.Update(item);

                    List<KPIPositionEmployee> listPositionEmployeeOld = _kpiPositionEmployeeRepo.GetAll(x => x.KPIPosiotionID == item.ID && x.IsDeleted == false);
                    foreach(var item2 in listPositionEmployeeOld)
                    {
                        item2.IsDeleted = true;
                        _kpiPositionEmployeeRepo.Update(item2);
                    }    
                }

                List<KPIPosition> listData = _kpiPositionRepo.GetAll(x => x.KPISessionID == kpiSessionFrom && x.IsDeleted == false);
                //foreach(var item in listData)
                //{
                //    item.KPISessionID = kpiSessionTo;
                //    _kpiPositionRepo.Create(item);
                //    int newID = item.ID;

                //    //lưu nhân viên theo vị trí
                //    List<KPIPositionEmployee> listEmployee = _kpiPositionEmployeeRepo.GetAll(x => x.KPIPosiotionID == item.ID);
                //    if(listEmployee.Count > 0)
                //    {
                //        foreach(var itemEm in listEmployee)
                //        {
                //            itemEm.KPIPosiotionID = newID;
                //            _kpiPositionEmployeeRepo.Create(itemEm);
                //        }    
                //    }    
                //}    

                foreach (var src in listData)
                {
                    int oldID = src.ID;

                    var newPosition = new KPIPosition
                    {
                        KPISessionID = kpiSessionTo,
                        PositionCode = src.PositionCode,
                        TypePosition = src.TypePosition,
                        KPIPositionTypeID = src.KPIPositionTypeID,
                        PositionName = src.PositionName,
                        IsDeleted = false
                    };

                    _kpiPositionRepo.Create(newPosition);
                    int newID = newPosition.ID;

                    var listEmployee = _kpiPositionEmployeeRepo
                        .GetAll(x => x.KPIPosiotionID == oldID)
                        .ToList();

                    foreach (var itemEm in listEmployee)
                    {
                        var newEmp = new KPIPositionEmployee
                        {
                            KPIPosiotionID = newID,
                            EmployeeID = itemEm.EmployeeID,
                            IsDeleted = false
                        };

                        _kpiPositionEmployeeRepo.Create(newEmp);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class SavePositionEmployeeRequest
        {
            public List<int> listDel { get; set; }
            public List<int> listInsert { get; set; }
            public int KPIPositionID { get; set; }
        }

        public class DeleteEmployeeRequest
        {
            public int? DepartmentID { get; set; }
            public int? KPIPositionEmployeeID { get; set; }
        }
    }
}
