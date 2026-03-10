using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static NPOI.HSSF.Util.HSSFColor;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class KPIEmployeeTeamController : ControllerBase
    {
        #region Khai báo repository
        KPIEmployeeTeamRepo teamRepo;
        DepartmentRepo departmentRepo;
        EmployeeRepo employeeRepo;
        KPIEmployeeTeamRepo _kpiEmployeeTeamRepo;
        KPIEmployeeTeamLinkRepo _kpiEmployeeTeamLinkRepo;
        public KPIEmployeeTeamController(KPIEmployeeTeamRepo teamRepo, EmployeeRepo employeeRepo, DepartmentRepo departmentRepo, KPIEmployeeTeamRepo kPIEmployeeTeamRepo, KPIEmployeeTeamLinkRepo kPIEmployeeTeamLinkRepo)
        {
            this.teamRepo = teamRepo;
            this.employeeRepo = employeeRepo;
            this.departmentRepo = departmentRepo;
            _kpiEmployeeTeamRepo = kPIEmployeeTeamRepo;
            _kpiEmployeeTeamLinkRepo = kPIEmployeeTeamLinkRepo;
        }

        #endregion
        #region lấy ra tất cả team
        /// <summary>
        /// lấy team theo từng quý, nawmg, phòng ban
        /// </summary>
        /// <param name="yearValue"></param>
        /// <param name="quarterValue"></param>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        [HttpGet("getall")]
        public IActionResult GetAll(int yearValue, int quarterValue, int departmentID)
        {

            try
            {

                var teams = SQLHelper<object>.ProcedureToList("spGetALLKPIEmployeeTeam",
                                                                new string[] { "@YearValue", "@QuarterValue", "@DepartmentID" },
                                                                new object[] { yearValue, quarterValue, departmentID });


                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(teams, 0)
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

        [HttpGet("get-employee-in-team")]
        public IActionResult GetAllEmployee(int departmentID = 0, int kpiEmployeeTeamID = 0)
        {
            try
            {
                var employees = SQLHelper<object>.ProcedureToList("spGetKPIEmployeeByDepartmentID", new string[] { "@DepartmentID", "@KPIEmployeeTeam" }, new object[] { departmentID, kpiEmployeeTeamID });

                return Ok(new { status = 1, data = SQLHelper<object>.GetListData(employees, 0) });
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
        #endregion
        #region lấy theo ID
        [HttpGet("getbyid")]
        public IActionResult FindByID(int id)
        {
            try
            {
                KPIEmployeeTeam team = teamRepo.GetByID(id);
                if (team.ID <= 0)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Team không có trong cơ sở dữ liệu!"
                    });
                }
                return Ok(new
                {
                    status = 1,
                    data = team
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
        #endregion
        #region Lưu dữ liệu
        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] KPIEmployeeTeam team)
        {
            try
            {

                if (team.ID <= 0) await teamRepo.CreateAsync(team);
                else await teamRepo.UpdateAsync(team);

                return Ok(new
                {
                    status = 1,
                    data = team,
                    message = "Cập nhật thành công!"
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
        #endregion

        [HttpPost("copy")]
        public IActionResult Copy([FromBody] CopyKPITeamRequest request)
        {
            try
            {

                CopyKPITeam(request);

                return Ok(ApiResponseFactory.Success(null, "Copy thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        private void CopyKPITeam(CopyKPITeamRequest request)
        {
            int oldQuarter = request.OldQuarter;
            int newQuarter = request.NewQuarter;
            int oldYear = request.OldYear;
            int newYear = request.NewYear;
            int? departmentID = request.DepartmentID;

            // ===== Validate =====
            if (oldQuarter == newQuarter && oldYear == newYear)
                throw new Exception("Quý/Năm mới phải khác Quý/Năm cũ!");

            if (oldQuarter > newQuarter && oldYear >= newYear)
                throw new Exception("Quý/Năm mới phải lớn hơn Quý/Năm cũ!");

            List<KPIEmployeeTeam> listTeam;
            List<KPIEmployeeTeam> listExistTeam;
            if(departmentID > 0)
            {
                 listTeam = _kpiEmployeeTeamRepo.GetAll(x => x.QuarterValue == oldQuarter && x.YearValue == oldYear && x.IsDeleted == false && x.DepartmentID == departmentID);
                 listExistTeam = _kpiEmployeeTeamRepo.GetAll(x => x.QuarterValue == newQuarter && x.YearValue == newYear && x.IsDeleted == false && x.DepartmentID == departmentID);
            }
            else
            {
                listTeam = _kpiEmployeeTeamRepo.GetAll(x => x.QuarterValue == oldQuarter && x.YearValue == oldYear && x.IsDeleted == false);
                listExistTeam = _kpiEmployeeTeamRepo.GetAll(x => x.QuarterValue == newQuarter && x.YearValue == newYear && x.IsDeleted == false);
            }

            if (!listTeam.Any())
                throw new Exception("Không tìm thấy team gốc để copy!");

            // ===== Lấy toàn bộ cây team =====
            var allTeamsToCopy = GetTeamHierarchy(listTeam, oldQuarter, oldYear);

            Dictionary<int, int> teamIdMapping = new();

            // ===== Insert team mới =====
            foreach (var oldTeam in allTeamsToCopy)
            {
                KPIEmployeeTeam newTeam = new KPIEmployeeTeam
                {
                    Name = oldTeam.Name,
                    DepartmentID = oldTeam.DepartmentID,
                    QuarterValue = newQuarter,
                    YearValue = newYear,
                    LeaderID = oldTeam.LeaderID,
                    ParentID = 0,
                    IsDeleted = false
                };

                _kpiEmployeeTeamRepo.Create(newTeam);
                int newTeamID = newTeam.ID;
                teamIdMapping.Add(oldTeam.ID, newTeamID);

                CopyTeamDetails(oldTeam.ID, newTeamID);
            }

            // ===== Update ParentID =====
            foreach (var oldTeam in allTeamsToCopy)
            {
                if (!teamIdMapping.ContainsKey(oldTeam.ID)) continue;

                var newTeam = _kpiEmployeeTeamRepo.GetByID(teamIdMapping[oldTeam.ID]);

                if (oldTeam.ParentID < 0)
                    newTeam.ParentID = oldTeam.ParentID;
                else if (oldTeam.ParentID.HasValue && teamIdMapping.ContainsKey(oldTeam.ParentID.Value))
                    newTeam.ParentID = teamIdMapping[oldTeam.ParentID.Value];

                _kpiEmployeeTeamRepo.Update(newTeam);
            }

            // ===== Delete team cũ =====
            DeleteExistingTeams(listExistTeam);
        }

        // ================== PRIVATE METHODS ==================

        private List<KPIEmployeeTeam> GetTeamHierarchy(List<KPIEmployeeTeam> selectedTeams, int quarter, int year)
        {
            var result = new List<KPIEmployeeTeam>();
            if (!selectedTeams.Any()) return result;

            // ===== DESCENDANTS =====
            var queue = new Queue<int>(selectedTeams.Select(t => t.ID));
            result.AddRange(selectedTeams);

            while (queue.Count > 0)
            {
                int parentId = queue.Dequeue();
                var children = _kpiEmployeeTeamRepo.GetAll(x =>
                    x.ParentID == parentId &&
                    x.QuarterValue == quarter &&
                    x.YearValue == year &&
                    x.IsDeleted == false);

                foreach (var child in children)
                {
                    if (!result.Any(r => r.ID == child.ID))
                    {
                        result.Add(child);
                        queue.Enqueue(child.ID);
                    }
                }
            }

            // ===== ANCESTORS =====
            var stack = new Stack<int>(selectedTeams.Select(t => t.ParentID ?? 0));

            while (stack.Count > 0)
            {
                int parentId = stack.Pop();
                if (parentId <= 0) continue;

                var parents = _kpiEmployeeTeamRepo.GetAll(x =>
                    x.ID == parentId &&
                    x.QuarterValue == quarter &&
                    x.YearValue == year &&
                    x.IsDeleted == false);

                foreach (var parent in parents)
                {
                    if (!result.Any(r => r.ID == parent.ID))
                    {
                        result.Add(parent);
                        stack.Push(parent.ParentID ?? 0);
                    }
                }
            }

            return result;
        }


        private void CopyTeamDetails(int oldTeamID, int newTeamID)
        {

            var listDetail = _kpiEmployeeTeamLinkRepo.GetAll(x => x.KPIEmployeeTeamID == oldTeamID && x.IsDeleted == false);

            if (!listDetail.Any()) return;

            var newDetails = listDetail.Select(d => new KPIEmployeeTeamLink
            {
                KPIEmployeeTeamID = newTeamID,
                EmployeeID = d.EmployeeID,
                IsDeleted = false
            }).ToList();

            _kpiEmployeeTeamLinkRepo.CreateRange(newDetails);
        }

        private void DeleteExistingTeams(List<KPIEmployeeTeam> lstExistTeam)
        {
            foreach (var team in lstExistTeam)
            {
                team.IsDeleted = true;

                _kpiEmployeeTeamRepo.Update(team);
                var listDetail = _kpiEmployeeTeamLinkRepo.GetAll(x => x.KPIEmployeeTeamID == team.ID && x.IsDeleted == false);

                foreach (var detail in listDetail)
                {
                    detail.IsDeleted = true;
                    _kpiEmployeeTeamLinkRepo.Update(detail);
                }
            }
        }

        public class CopyKPITeamRequest
        {
            public int OldQuarter { get; set; }
            public int NewQuarter { get; set; }
            public int OldYear { get; set; }
            public int NewYear { get; set; }
            public int? DepartmentID { get; set; }
        }

    }
}
