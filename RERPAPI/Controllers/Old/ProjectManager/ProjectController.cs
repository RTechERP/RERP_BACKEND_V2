using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Text.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.Old.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]

    // [ApiKeyAuthorize]
    public class ProjectController : ControllerBase
    {
        #region Khai báo biến
        ProjectRepo projectRepo = new ProjectRepo();
        ProjectTreeFolderRepo projectTreeFolderRepo = new ProjectTreeFolderRepo();
        CustomerRepo customerRepo = new CustomerRepo();
        ProjectTypeRepo projectTypeRepo = new ProjectTypeRepo();
        ProjectStatusRepo projectStatusRepo = new ProjectStatusRepo();
        BusinessFieldRepo businessFieldRepo = new BusinessFieldRepo();

        GroupFileRepo groupFileRepo = new GroupFileRepo();
        FirmBaseRepo firmBaseRepo = new FirmBaseRepo();
        ProjectTypeBaseRepo projectTypeBaseRepo = new ProjectTypeBaseRepo();
        FollowProjectBaseRepo followProjectBaseRepo = new FollowProjectBaseRepo();

        ProjectStatusLogRepo projectStatusLogRepo = new ProjectStatusLogRepo();
        ProjectEmployeeRepo projectEmployeeRepo = new ProjectEmployeeRepo();

        ProjectUserRepo projectUserRepo = new ProjectUserRepo();
        ProjectTypeLinkRepo projectTypeLinkRepo = new ProjectTypeLinkRepo();

        ProjectCostRepo projectCostRepo = new ProjectCostRepo();
        ProjectPriorityLinkRepo projectPriorityLinkRepo = new ProjectPriorityLinkRepo();

        ProjectCurrentSituationRepo projectCurrentSituationRepo = new ProjectCurrentSituationRepo();

        ProjectPriorityRepo projectPriorityRepo = new ProjectPriorityRepo();
        ProjectPersonalPriotityRepo projectPersonalPriotityRepo = new ProjectPersonalPriotityRepo();
        ProjectWorkerTypeRepo projectWorkerTypeRepo = new ProjectWorkerTypeRepo();
        DailyReportTechnicalRepo dailyReportTechnicalRepo = new DailyReportTechnicalRepo();
        ProjectStatusDetailRepo projectStatusDetailRepo = new ProjectStatusDetailRepo();
        #endregion

        #region Hàm dùng chung

        private List<object> getDataUser(int numberTable)
        {
            //List<PmDTO> pms = SQLHelper<PmDTO>.ProcedureToListFromResult("spGetEmployeeForProject", numberTable, new string[] { }, new object[] { });
            var pms = SQLHelper<object>.ProcedureToList("spGetEmployeeForProject", new string[] { }, new object[] { });
            return pms[numberTable];
        }
        #endregion

        #region API GET
        // Lấy danh sách thư mục 
        // [ApiKeyAuthorize]
        [HttpGet("get-folders")]
        public async Task<IActionResult> GetFolders()
        {
            try
            {
                List<ProjectTreeFolder> projectTreeFolders = projectTreeFolderRepo.GetAll();
                return Ok(ApiResponseFactory.Success(projectTreeFolders, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Danh sách nhân viên khi thêm dự án lấy table 1
        [HttpGet("get-pms")]
        //[ApiKeyAuthorize]
        public async Task<IActionResult> GetPms()
        {
            try
            {
                var pms = getDataUser(0);
                return Ok(ApiResponseFactory.Success(pms, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy danh sách khách hàng
        [HttpGet("get-customers")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                List<Customer> customers = customerRepo.GetAll().Where(x => x.IsDeleted == false).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(customers, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Danh sách nhân viên khi thêm dự án lấy table 2 phụ trách sale/ phụ trách kỹ thuật/ leader
        [HttpGet("get-users")]
        // [ApiKeyAuthorize]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = getDataUser(1);
                return Ok(ApiResponseFactory.Success(users, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Danh sách loại dự án 
        [HttpGet("get-project-types")]
        //   [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectTypes()
        {
            try
            {
                List<ProjectType> projectTypes = projectTypeRepo.GetAll().Where(x => x.ID != 4).ToList();
                return Ok(ApiResponseFactory.Success(projectTypes, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Danh sách loại dự án ProjectTypeLink
        [HttpGet("get-project-type-links")]
        //   [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectTypeLinks(int id)
        {
            try
            {
                //List<ProjectTypeLinkDTO> projectTypeLinkDTOs = SQLHelper<ProjectTypeLinkDTO>
                //    .ProcedureToList("spGetProjectTypeLink", new string[] { "@ProjectID" }, new object[] { id });
                var projectTypeLinkDTOs = SQLHelper<object>.ProcedureToList("spGetProjectTypeLink", new string[] { "@ProjectID" }, new object[] { id });

                return Ok(ApiResponseFactory.Success(projectTypeLinkDTOs.FirstOrDefault(), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Load Hạng mục công việc
        [HttpGet("get-project-items")]
        //   [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectItems(int id)
        {
            try
            {
                if (id <= 0) id--;

                //List<ProjectItemDTO> projectItems = SQLHelper<ProjectItemDTO>
                //    .ProcedureToList("spGetProjectItem", new string[] { "@ProjectID" }, new object[] { id });
                var projectItems = SQLHelper<object>.ProcedureToList("spGetProjectItem", new string[] { "@ProjectID" }, new object[] { id });
                return Ok(ApiResponseFactory.Success(projectItems[0], ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        // Danh sách trạng thái dự án 
        [HttpGet("get-project-status")]
        //   [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectStatus()
        {
            try
            {
                List<ProjectStatus> projectStatus = projectStatusRepo.GetAll().OrderBy(x => x.STT).ToList();
                return Ok(ApiResponseFactory.Success(projectStatus, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Danh sách lĩnh vực kinh doanh
        [HttpGet("get-business-fields")]
        // [ApiKeyAuthorize]
        public async Task<IActionResult> GetBusinessFields()
        {
            try
            {
                List<BusinessField> businessFields = businessFieldRepo.GetAll().OrderBy(x => x.STT).ToList();
                return Ok(ApiResponseFactory.Success(businessFields, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Danh sách dự án 
        [HttpGet("get-projects")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjects(int size, int page,
            DateTime dateTimeS, DateTime dateTimeE, string? projectType,
            int pmID, int leaderID, int bussinessFieldID, string? projectStatus,
            int customerID, int saleID, int userTechID, int globalUserID, string? keyword, bool isAGV
            )
        {
            try
            {
                int[] typeCheck = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                List<int> projectTypeIDs = projectTypeRepo.GetAll().Select(x => x.ID).ToList();

                if (string.IsNullOrWhiteSpace(projectType)) projectType = string.Join(",", projectTypeIDs);
                else
                {
                    foreach (string item in projectType.Split(','))
                    {
                        int index = projectTypeIDs.IndexOf(Convert.ToInt32(item));
                        if (index >= 0 && index < typeCheck.Length)
                        {
                            typeCheck[index] = 1;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(projectStatus))
                {
                    List<int> listStatus = projectStatusRepo.GetAll().Select(x => x.ID).ToList();
                    projectStatus = string.Join(",", listStatus);
                }

                if (isAGV == false) projectStatus = "";


                var projects = SQLHelper<object>.ProcedureToList("spGetProject",
                    new string[] {
                        "@PageSize", "@PageNumber", "@DateStart", "@DateEnd", "@FilterText", "@CustomerID", "@UserID",
                        "@ListProjectType", "@LeaderID", "@UserIDTech", "@EmployeeIDPM", "@1", "@2", "@3", "@4", "@5",
                        "@6", "@7", "@8", "@9", "@UserIDPriotity", "@BusinessFieldID", "@ProjectStatus"
                    },
                    new object[] {
                        size, page, dateTimeS, dateTimeE, keyword ?? "", customerID, saleID, projectType, leaderID,
                        userTechID, pmID, typeCheck[0] ,typeCheck[1] ,typeCheck[2] ,typeCheck[3] ,typeCheck[4] ,typeCheck[5]
                        ,typeCheck[6] ,typeCheck[7] ,typeCheck[8], globalUserID, bussinessFieldID, projectStatus
                    });

                //var projects = SQLHelper<object>.ProcedureToList("spGetProject",
                //    new string[] {
                //        "@PageSize", "@PageNumber", "@DateStart", "@DateEnd", "@FilterText", "@CustomerID", "@UserID",
                //        "@ListProjectType", "@LeaderID", "@UserIDTech", "@EmployeeIDPM", "@1", "@2", "@3", "@4", "@5",
                //        "@6", "@7", "@8", "@9", "@UserIDPriotity", "@BusinessFieldID"
                //    },
                //    new object[] {
                //        size, page, dateTimeS, dateTimeE, keywword ?? "", customerID, saleID, projectType, leaderID,
                //        userTechID, pmID, typeCheck[0] ,typeCheck[1] ,typeCheck[2] ,typeCheck[3] ,typeCheck[4] ,typeCheck[5]
                //        ,typeCheck[6] ,typeCheck[7] ,typeCheck[8], globalUserID, bussinessFieldID
                //    });
                var project = SQLHelper<object>.GetListData(projects, 0);
                var totalPage = SQLHelper<object>.GetListData(projects, 1).FirstOrDefault().TotalPage;
                return Ok(ApiResponseFactory.Success(new { project, totalPage }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lấy danh sách dự án chi tiết ở dưới
        [HttpGet("get-project-details")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectDetails(int id)
        {
            // Update store spGetProjectDetail p.Note => p.Note AS ProjectNote 
            try
            {
                //List<ProjectDetailDTO> projectDetails = SQLHelper<ProjectDetailDTO>
                //    .ProcedureToList("spGetProjectDetail", new string[] { "@ID" }, new object[] { id });
                var projectDetails = SQLHelper<object>.ProcedureToList("spGetProjectDetail", new string[] { "@ID" }, new object[] { id });
                return Ok(ApiResponseFactory.Success(projectDetails, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }


        }
        // Lấy danh sách dự án chi tiết ở dưới
        [HttpGet("get-project")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> GetProject(int id)
        {
            try
            {
                var projectDetail = projectRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(projectDetail, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy hiện trạng dự án 
        [HttpGet("get-project-current-situation")]
        //[ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectCurrentSituation(int projectId, int employeeId)
        {
            try
            {
                ProjectCurrentSituation projectDetail = projectCurrentSituationRepo.GetAll()
                    .Where(x => x.ProjectID == projectId && x.EmployeeID == employeeId)
                    .OrderByDescending(x => x.DateSituation).FirstOrDefault();
                projectDetail = projectDetail ?? new ProjectCurrentSituation();


                return Ok(ApiResponseFactory.Success(projectDetail.ContentSituation, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        // modal lấy danh sách nhóm file
        [HttpGet("get-group-files")]
        // [ApiKeyAuthorize]
        public async Task<IActionResult> GetGroupFiles()
        {
            try
            {
                List<GroupFile> grf = groupFileRepo.GetAll();
                return Ok(ApiResponseFactory.Success(grf, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // modal lấy danh sách FirmBase
        [HttpGet("get-firm-bases")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> GetFirmBases()
        {
            try
            {
                List<FirmBase> firmBases = firmBaseRepo.GetAll();
                return Ok(ApiResponseFactory.Success(firmBases, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // modal lấy kiểu dự án Base
        [HttpGet("get-project-type-bases")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectTypeBases()
        {
            try
            {
                List<ProjectTypeBase> pfb = projectTypeBaseRepo.GetAll();
                return Ok(ApiResponseFactory.Success(pfb, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // modal lấy người dùng dự án 
        [HttpGet("get-project-users")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectUsers(int id)
        {
            try
            {
                var projectUser = SQLHelper<object>.ProcedureToList("spGetProjectUser", new string[] { "@ID" }, new object[] { id });
                return Ok(ApiResponseFactory.Success(projectUser.FirstOrDefault(), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //modal lấy dữ liệu FollowProjectBase
        [HttpGet("get-follow-project-bases")]
        //   [ApiKeyAuthorize]
        public async Task<IActionResult> GetFollowProjectBases(int id)
        {
            try
            {
                FollowProjectBase followProjectBase = followProjectBaseRepo
                    .GetAll().Where(x => x.ProjectID == id)
                    .OrderByDescending(x => x.ExpectedPlanDate).FirstOrDefault();
                return Ok(ApiResponseFactory.Success(followProjectBase, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //modal lấy dữ liệu PriorityType
        [HttpGet("get-priority-type")]
        // [ApiKeyAuthorize]
        public async Task<IActionResult> GetPriorityType()
        {
            try
            {
                var projectType = projectPriorityRepo.GetAll().Where(x => x.ParentID == 0).ToList();
                return Ok(ApiResponseFactory.Success(projectType, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //modal lấy dữ liệu PriorityType
        [HttpGet("get-project-priority-detail")]
        // [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectPriorityDetail(int id)
        {
            try
            {
                var projectType = projectPriorityRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(projectType, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //modal lấy dữ liệu priorytipersion
        [HttpGet("get-personal-priority")]
        // [ApiKeyAuthorize]
        public async Task<IActionResult> GetPersonalPriority(int projectId, int userId)
        {
            try
            {
                int prio = 0;
                var priority = projectPersonalPriotityRepo.GetAll().Where(x => x.ProjectID == projectId && x.UserID == userId).FirstOrDefault();
                if (priority != null)
                {
                    prio = (int)priority.Priotity;
                }
                return Ok(ApiResponseFactory.Success(prio, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //modal kiểm tra mã ưu tiê
        [HttpGet("check-project-priority")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> CheckProjectPriority(int id, string code)
        {
            try
            {
                bool check = false;
                var projectPriority = projectPriorityRepo.GetAll().Where(x => x.ID != id && x.Code == code);
                if (projectPriority.Count() > 0) check = true;
                return Ok(ApiResponseFactory.Success(check, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Xóa dữ liệu project
        [HttpGet("deleted-project")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> DeletedProject(string ids)
        {
            try
            {
                var idArray = ids.Split(',').Select(int.Parse).ToArray();
                if (idArray.Count() > 0)
                {
                    foreach (int id in idArray)
                    {
                        projectRepo.Delete(id);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Xóa dự án thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("check-project-code")]
        //[ApiKeyAuthorize]
        public async Task<IActionResult> CheckProjectCode(int id, string projectCode)
        {
            try
            {
                List<Model.Entities.Project> projects = new List<Model.Entities.Project>();
                if (id > 0)
                {
                    var check = projectRepo.GetAll(x => x.ProjectCode == projectCode);
                    projects = projectRepo.GetAll(x => x.ProjectCode == projectCode && x.ID != id);
                }
                else
                {
                    projects = projectRepo.GetAll().Where(x => x.ProjectCode.Contains(projectCode)).ToList();
                }


                return Ok(ApiResponseFactory.Success(projects.Count() > 0 ? 0 : 1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("save-change-project")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> SaveChangeProject(int projectIdOld, int projectIdNew)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spUpdateProjectIDInDailyReportTechnical_ByNewProjectID",
                    new string[] { "@OldProjectID", "@NewProjectID" }, new object[] { projectIdOld, projectIdNew });

                return Ok(ApiResponseFactory.Success(true, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // lấy trạng thái dự án 
        [HttpGet("get-project-statuss")]
        // [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjectStatus(int projectId)
        {
            try
            {
                var status = SQLHelper<object>.ProcedureToList("spGetProjectStatus", new string[] { "@ProjectID" }, new object[] { projectId });
                var statuss = SQLHelper<object>.GetListData(status, 0);
                return Ok(ApiResponseFactory.Success(statuss, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpGet("foldertree/{id}")]
        //public async Task<IActionResult> foldertree(int id)
        //{
        //    try
        //    {
        //        // Cho vào hàm chung sau 
        //        string pathLocation = @"\\192.168.1.190\duan\Projects"; //Thư mục trên server
        //        if (/*Config.IsOnline*/ false && string.IsNullOrEmpty(Config.ConnectionString)) pathLocation = @"\\14.232.152.154\duan\Projects";
        //        else if (!string.IsNullOrEmpty(Config.ConnectionString)) pathLocation = @"D:\LeTheAnh\RTC\Project"; //Thư mục test local

        //        Project prj = projectRepo.GetByID(id);
        //        if(prj != null)
        //        {
        //            DateTime createdDate = (DateTime)prj.CreatedDate;
        //            string code = prj.ProjectCode;
        //            int year = createdDate.Year;

        //            if (/*Global.IsOnline*/ false)
        //            {
        //                //pathLocation = @"\\rtctechnologydata.ddns.net\DUAN\Projects\";
        //                pathLocation = @"\\14.232.152.154\DUAN\Projects\";
        //            }
        //        }

        //        // lấy trạng thái dự án 
        //        [HttpGet("get-project-statuss")]
        //        // [ApiKeyAuthorize]
        //        public async Task<IActionResult> GetProjectStatus(int projectId)
        //        {
        //            try
        //            {
        //                Directory.CreateDirectory(pathLocation);
        //            }
        //            catch (Exception)
        //            {
        //                pathLocation = @"\\rtctechnologydata.ddns.net\DUAN\Projects\";
        //            }
        //        }

        //            string path = $@"{pathLocation}\{year}\{code}";
        //            List<int> listProjectTypeID = new List<int>();
        //            List<string> listProjectTypeName = new List<string>();

        //            var data = SQLHelper<object>.ProcedureToList("spGetProjectTypeLink", new string[] { "@ProjectID" }, new object[] { id });

        //            var projectTypes = SQLHelper<object>.GetListData(data, 0).Where(x=> x.Seleted == true);
        //            foreach(var item in projectTypes)
        //            {
        //                int projectTypeID;
        //                string projectTypeName = "";
        //                if (item.Seleted)
        //                {
        //                    projectTypeID = item.ProjectTypeID;
        //                    projectTypeName = item.ProjectTypeName;

        //                    if (!listProjectTypeID.Contains(projectTypeID)) listProjectTypeID.Add(projectTypeID);
        //                    if (!listProjectTypeName.Contains(projectTypeName)) listProjectTypeName.Add(projectTypeName);
        //                }
        //            }

        //            if (listProjectTypeID.Count > 0)
        //            {
        //                for (int i = 0; i < listProjectTypeID.Count; i++)
        //                {

        //                    var dt = SQLHelper<object>.GetListData(SQLHelper<object>.ProcedureToList("sp_GetProjectTypeTreeFolder",
        //                                                            new string[] { "@ProjectTypeID" },
        //                                                            new object[] { listProjectTypeID[i] }), 0);

        //                    if (dt.Count() <= 0)
        //                        continue;
        //                    dt.Columns.Add("Path", typeof(string));

        //                    //---------------------------------------------------------------- phuc add new
        //                    dt.Columns.Add("GP1ID", typeof(int));
        //                    dtProjectTypeTreeFolder = dt.Copy();
        //                    addGPProjectTypeTreeFolder(dt);
        //                    //------------------------------------------------------------------------end

        //                    CreateDirectoryWithDatatable(dt, path);

        //                    ////string parentfolder = TextUtils.ToString(dt.Rows[0]["FolderName"]);

        //        //                    if (dt.Count() <= 0)
        //        //                        continue;
        //        //                    dt.Columns.Add("Path", typeof(string));

        //                    //string subPath = "";
        //                    //for (int j = 0; j < dt.Rows.Count; j++)
        //                    //{
        //                    //    var dataRow1 = dt.Select("ParentID = 0");
        //                    //    var dataRow2 = dt.Select("ParentID <> 0");
        //                    //    string parentfolder = dataRow1[0]["FolderName"].ToString();
        //                    //    //string subFolder = dt.Rows[i]["FolderName"].ToString();

        //                    //    //subPath += parentfolder + "\\" + subFolder;

        //                    //    //string pathCreate = Path.Combine(path, parentfolder, subFolder);

        //                    //    Directory.CreateDirectory($@"{path}\{parentfolder}");
        //                    //    for (int k = 0; k < dataRow2.Length; k++)
        //                    //    {
        //                    //        string childfolder = dataRow2[k]["FolderName"].ToString();
        //                    //        Directory.CreateDirectory($@"{path}\{parentfolder}\{childfolder}");
        //                    //    }
        //                    //}
        //                }
        //            }

        //        }
        //        return Ok(new
        //        {
        //            status = 1,
        //            data = ""
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}

        //        //                    //    //subPath += parentfolder + "\\" + subFolder;

        // modal loadProjectCode
        [HttpGet("get-project-code-modal")]
        //[ApiKeyAuthorize]
        public async Task<IActionResult> GePprojectCodeModal(int projectId, string customerShortName, int projectType)
        {
            try
            {
                string returnProjectCode = "";

                int number = 0;
                string year = DateTime.Now.Year.ToString().Substring(2, 2);
                int currentYear = DateTime.Now.Year;
                DateTime from = new DateTime(currentYear, 1, 1);
                DateTime to = from.AddYears(1);
                string cusShortName = $"{customerShortName}.";

                var listCodes = projectRepo.GetAll()
                    .Where(x => x.ProjectCode.Contains(cusShortName) && x.CreatedDate >= from && x.CreatedDate < to)
                    .Select(x => new
                    {
                        STT = Convert.ToInt32(x.ProjectCode.Split('.')[x.ProjectCode.Split('.').Length - 1]),
                        x.ProjectCode,
                    }).OrderByDescending(x => x.STT).ToList();

                string projectCode = "";
                if (listCodes.Count() > 0)
                {
                    projectCode = listCodes.FirstOrDefault().ProjectCode.ToString();
                }

                if (projectId == 0)
                {
                    if (projectCode == "")
                    {
                        returnProjectCode = customerShortName + "." + year + ".001";
                    }
                    else
                    {
                        string count = "";
                        if (!projectCode.Contains("."))
                        {
                            count = (Convert.ToInt32(projectCode.Substring(projectCode.Length - 2)) + 1).ToString();
                        }
                        else
                        {
                            string[] arrCode = projectCode.Split('.');
                            count = (Convert.ToInt32(arrCode[arrCode.Length - 1]) + 1).ToString();
                        }
                        for (int j = 0; count.Length < 3; j++)
                        {
                            count = "0" + count;
                        }
                        returnProjectCode = customerShortName + "." + year + "." + count;
                    }

                    if (projectType == 2)
                    {
                        returnProjectCode = $"TM.{returnProjectCode}";
                    }
                    else if (projectType == 3)
                    {
                        returnProjectCode = $"F.{returnProjectCode}";
                    }
                }


                return Ok(ApiResponseFactory.Success(returnProjectCode == "" ? projectCode : returnProjectCode, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy danh sách khách hàng
        [HttpGet("get-user-teams")]
        public async Task<IActionResult> GetUserTeams()
        {
            try
            {
                var userTeams = SQLHelper<object>.ProcedureToList("spGetUserTeam", new string[] { "@DepartmentID" }, new object[] { 0 });
                var usTeam = SQLHelper<object>.GetListData(userTeams, 1);
                return Ok(ApiResponseFactory.Success(usTeam, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy danh sách dự án
        [HttpGet("get-project-modal")]
        public async Task<IActionResult> GetProjectModal()
        {
            try
            {
                List<Model.Entities.Project> prjs = projectRepo.GetAll().OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(prjs, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy danh sách ưu tiên dự án
        [HttpGet("get-project-priority-modal")]
        public async Task<IActionResult> GetProjectPriorityModal(int projectId)
        {
            try
            {
                var prjPriority = projectPriorityRepo.GetAll();

                List<int> checks = new List<int>();
                if (projectId != 0)
                {
                    var listProjectPrioLink = projectPriorityLinkRepo.GetAll().Where(x => x.ProjectID == projectId);
                    if (listProjectPrioLink.Count() > 0)
                    {
                        foreach (var item in listProjectPrioLink)
                        {
                            int id = (int)item.ProjectPriorityID;
                            if (!checks.Contains(id)) checks.Add(id);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(new { prjPriority, checks }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy chi tiết tổng hợp nhân công
        [HttpGet("get-project-worker-synthetic")]
        public async Task<IActionResult> GetProjectWorkerSynthetic(int projectId, int prjWorkerTypeId, string? keyword)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProjectWokerSynthetic",
                new string[] { "@ProjectID", "@ProjectWorkerTypeID", "@Keyword" },
                new object[] { projectId, prjWorkerTypeId, keyword ?? "" });
                var projectWorker = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(projectWorker, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy chi tiết tổng hợp báo cáo công việc
        [HttpGet("get-project-work-report")]
        public async Task<IActionResult> GetProjectWorkReport(int page, int size, int projectId, string? keyword)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetDailyReportTechnical_New",
                new string[] { "@ProjectID", "@FilterText", "@PageSize", "@PageNumber" },
                new object[] { projectId, keyword ?? "", page, size });
                var projectwork = SQLHelper<object>.GetListData(data, 1);
                return Ok(ApiResponseFactory.Success(projectwork, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-worker-type")]
        public async Task<IActionResult> GetWorkerType()
        {
            try
            {
                var workerTypes = projectWorkerTypeRepo.GetAll();
                return Ok(ApiResponseFactory.Success(workerTypes, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region Chức năng gười tham gia dự án
        [HttpGet("get-project-employee")]
        public async Task<IActionResult> GetProjectEmployee(int status)
        {
            try
            {
                var employees = SQLHelper<object>.ProcedureToList("spGetEmployee", new string[] { "@Status" }, new object[] { status });
                var employee = SQLHelper<object>.GetListData(employees, 0);
                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-status-project-employee")]
        public async Task<IActionResult> GetStatusProjectEmployee()
        {
            try
            {
                var status = projectStatusRepo.GetAll();
                return Ok(ApiResponseFactory.Success(status, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-project-type")]
        public async Task<IActionResult> GetProjectType()
        {
            try
            {
                var projectTpye = projectTypeRepo.GetAll();
                return Ok(ApiResponseFactory.Success(projectTpye, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-suggest")]
        public async Task<IActionResult> GetEmployeeSuggest(int projectId)
        {
            try
            {
                var employee = SQLHelper<object>.ProcedureToList("spGetProjectParticipant",
                                            new string[] { "@ProjectID" },
                                            new object[] { projectId });
                var employees = SQLHelper<object>.GetListData(employee, 0);
                return Ok(ApiResponseFactory.Success(employees, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-main")]
        public async Task<IActionResult> GetEmployeeMain(int projectId, int isDeleted)
        {
            try
            {
                var employee = SQLHelper<object>.ProcedureToList("spGetProjectEmployee",
                                            new string[] { "@ProjectID", "@IsDeleted" },
                                            new object[] { projectId, isDeleted });
                var employees = SQLHelper<object>.GetListData(employee, 0);
                return Ok(ApiResponseFactory.Success(employees, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-permission")]
        public async Task<IActionResult> GetEmployeePermission(int projectId, int employeeId)
        {
            try
            {
                bool havePermission = false;
                var employee = SQLHelper<object>.ProcedureToList("spGetProjectEmployeePermisstion",
                                                        new string[] { "@ProjectID", "@EmployeeID" },
                                                        new object[] { projectId, employeeId });
                var data = SQLHelper<object>.GetListData(employee, 0);

                int valueRow = (int)data[0]["RowNumber"];

                Model.Entities.Project project = projectRepo.GetByID(projectId);

                // Check quyền sau khi có phân quyền
                //if(project.ProjectManager == Global.EmployeeID || project.UserID == Global.UserID || Global.IsAdmin || valueRow > 0 || project.UserTechnicalID == Global.UserID)
                return Ok(ApiResponseFactory.Success(havePermission, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion


        #endregion

        #region API POST
        [HttpPost("save-project")]
        public async Task<IActionResult> SaveProject([FromBody] ProjectDTO prj)
        {
            try
            {
                int statusOld = prj.projectStatusOld;
                var pr = prj.project ?? null;
                int projectId = prj.project.ID;

                Model.Entities.Project project = projectId > 0 ? projectRepo.GetByID(projectId) : new Model.Entities.Project();
                project.ProjectCode = prj.project.ProjectCode;
                project.ProjectName = prj.project.ProjectName;
                project.ProjectStatus = prj.project.ProjectStatus;
                project.Note = prj.project.Note;
                project.CustomerID = prj.project.CustomerID;
                project.UserID = prj.project.UserID;
                project.UserTechnicalID = prj.project.UserTechnicalID;
                project.CreatedDate = prj.project.CreatedDate;
                project.ProjectManager = prj.project.ProjectManager;
                project.EndUser = prj.project.EndUser;
                project.Priotity = prj.project.Priotity;
                project.TypeProject = prj.project.TypeProject;
                project.BusinessFieldID = 0;
                project.UpdatedDate = DateTime.Now;


                if (statusOld != project.ProjectStatus)
                {
                    ProjectStatusLog statusLog = new ProjectStatusLog();
                    statusLog.ProjectID = project.ID;
                    statusLog.ProjectStatusID = project.ProjectStatus;
                    statusLog.EmployeeID = prj.projectStatusLog.EmployeeID;
                    statusLog.DateLog = prj.projectStatusLog.DateLog;
                    projectStatusLogRepo.CreateAsync(statusLog);
                }

                if (project.ID > 0) projectRepo.Update(project);
                else
                {
                    projectRepo.Create(project);
                    projectId = project.ID;
                }
                ;

                var updateStatus = SQLHelper<object>.ProcedureToList(
                    "spUpdateProjectStatus",
                    new string[] { "@ProjectID", "@ProjectStatusID" },
                    new object[] { projectId, project.ProjectStatus });


                // Lưu thông tin kiểu và leader tham gia
                var projectTypeLinks = prj.projectTypeLinks;
                if (projectTypeLinks.Count() > 0)
                {
                    foreach (var item in projectTypeLinks)
                    {
                        bool isSelect = (bool)item.Selected;
                        int leaderID = (int)item.LeaderID;
                        int projectTypeID = (int)item.ProjectTypeID;

                        if (isSelect || leaderID > 0)
                        {
                            // Lưu người tham gia dự án 
                            ProjectEmployee model = new ProjectEmployee();

                            int employeeID = item.EmployeeID;

                            var projectEmployee = projectEmployeeRepo
                                .GetAll()
                                .Where(x => x.ProjectID == project.ID && x.EmployeeID == employeeID
                                && x.ProjectTypeID == projectTypeID && x.IsDeleted != true);
                            if (projectEmployee.Count() > 0)
                            {
                                model = projectEmployee.FirstOrDefault();
                            }

                            model.ProjectID = projectId;
                            model.EmployeeID = employeeID;
                            model.ProjectTypeID = projectTypeID;
                            model.IsLeader = true;

                            if (model.ID > 0) projectEmployeeRepo.Update(model);
                            else
                            {
                                var list = projectEmployeeRepo.GetAll().Where(x => x.ProjectID == projectId && x.IsDeleted != true);
                                model.STT = list.Count() + 1;
                                projectEmployeeRepo.Create(model);
                            }
                        }



                    }
                }

                foreach (var item in projectTypeLinks)
                {
                    int projectTypeLinkID = item.ProjectTypeLinkID;
                    ProjectTypeLink prjTypeLink = new ProjectTypeLink();
                    if (projectTypeLinkID > 0) prjTypeLink = projectTypeLinkRepo.GetByID(projectTypeLinkID);

                    prjTypeLink.ProjectID = project.ID;
                    prjTypeLink.LeaderID = item.LeaderID;
                    prjTypeLink.ProjectTypeID = item.ID;
                    prjTypeLink.Selected = item.Selected;

                    if (prjTypeLink.ID > 0) projectTypeLinkRepo.Update(prjTypeLink);
                    else projectTypeLinkRepo.CreateAsync(prjTypeLink);
                }

                // Lưu thông tin người tham gia
                //var projectUser = prj.projectUsers;
                //if (projectUser.Count() > 0)
                //{
                //    foreach (var item in projectUser)
                //    {
                //        ProjectUser model = item.ID > 0 ? projectUserRepo.GetByID(item.ID) : new ProjectUser();

                //        model.ProjectID = projectId;
                //        model.STT = item.STT;
                //        model.UserID = item.UserID;
                //        model.Mission = item.Mission;

                //        if (model.ID > 0) projectUserRepo.UpdateFieldsByID(item.ID, model);
                //        else projectUserRepo.CreateAsync(model);
                //    }
                //}

                var projectCost = projectCostRepo.GetAll().Where(x => x.ProjectID == projectId);
                if (projectCost.Count() <= 0)
                {
                    var update = SQLHelper<object>.ProcedureToList("spSaveProjectCost", new string[] { "@ID" }, new object[] { projectId });
                }

                if (projectId > 0)
                {
                    List<FollowProjectBase> followProjectBases = followProjectBaseRepo.GetAll().Where(x => x.ProjectID == projectId && x.WarehouseID == 1).ToList();
                    FollowProjectBase model = new FollowProjectBase();
                    if (followProjectBases.Count() <= 0)
                    {
                        model.ProjectID = projectId;

                        model.ExpectedPlanDate = prj.followProjectBase.ExpectedPlanDate;
                        model.ExpectedQuotationDate = prj.followProjectBase.ExpectedQuotationDate;
                        model.ExpectedPODate = prj.followProjectBase.ExpectedPODate;
                        model.ExpectedProjectEndDate = prj.followProjectBase.ExpectedProjectEndDate;

                        model.RealityPlanDate = prj.followProjectBase.RealityPlanDate;
                        model.RealityQuotationDate = prj.followProjectBase.RealityQuotationDate;
                        model.RealityPODate = prj.followProjectBase.RealityPODate;
                        model.RealityProjectEndDate = prj.followProjectBase.RealityProjectEndDate;

                        model.FirmBaseID = prj.followProjectBase.FirmBaseID;
                        model.ProjectTypeBaseID = prj.followProjectBase.ProjectTypeBaseID;
                        model.ProjectContactName = prj.followProjectBase.ProjectContactName;

                        model.ProjectStartDate = project.CreatedDate;
                        model.WarehouseID = 1;

                        followProjectBaseRepo.CreateAsync(model);
                    }
                    else
                    {
                        foreach (var item in followProjectBases)
                        {
                            model = item;
                            model.ProjectID = projectId;

                            model.ExpectedPlanDate = prj.followProjectBase.ExpectedPlanDate;
                            model.ExpectedQuotationDate = prj.followProjectBase.ExpectedQuotationDate;
                            model.ExpectedPODate = prj.followProjectBase.ExpectedPODate;
                            model.ExpectedProjectEndDate = prj.followProjectBase.ExpectedProjectEndDate;

                            model.RealityPlanDate = prj.followProjectBase.RealityPlanDate;
                            model.RealityQuotationDate = prj.followProjectBase.RealityQuotationDate;
                            model.RealityPODate = prj.followProjectBase.RealityPODate;
                            model.RealityProjectEndDate = prj.followProjectBase.RealityProjectEndDate;

                            model.FirmBaseID = prj.followProjectBase.FirmBaseID;
                            model.ProjectTypeBaseID = prj.followProjectBase.ProjectTypeBaseID;
                            model.ProjectContactName = prj.followProjectBase.ProjectContactName;

                            model.ProjectStartDate = project.CreatedDate;

                            if (model.ID > 0) followProjectBaseRepo.Update(model);
                            else
                            {
                                model.WarehouseID = 1;
                                followProjectBaseRepo.CreateAsync(model);
                            }
                        }
                    }
                }


                List<ProjectPriorityLink> projectPriorityLink = projectPriorityLinkRepo.GetAll().Where(x => x.ProjectID == projectId).ToList();
                if (projectPriorityLink.Count() > 0)
                {
                    projectPriorityLinkRepo.DeleteRange(projectPriorityLink);
                }

                var listPriorities = prj.listPriorities;
                if (listPriorities.Count() > 0)
                {
                    foreach (var item in listPriorities)
                    {
                        ProjectPriorityLink model = new ProjectPriorityLink();
                        model.ProjectPriorityID = item.ID;
                        model.ProjectID = projectId;
                        projectPriorityLinkRepo.Create(model);
                    }
                }

                return Ok(ApiResponseFactory.Success(true, "Lưu dự án thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // lưu trạng thái dự án 

        [HttpPost("save-project-status")]
        public async Task<IActionResult> SaveProjectStatus(int Stt, string statusName)
        {
            try
            {
                ProjectStatus prjs = new ProjectStatus();
                prjs.STT = Stt;
                prjs.StatusName = statusName;
                projectStatusRepo.Create(prjs);

                return Ok(ApiResponseFactory.Success(true, "Lưu trạng thái dự án thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lưu leader dự án
        [HttpPost("save-project-type-link")]
        public async Task<IActionResult> SaveProjectTypeLink([FromBody] ProjectTypeLinkDTO prjTypeLink)
        {
            try
            {
                if (prjTypeLink.ProjectID > 0)
                {
                    Model.Entities.Project project = projectRepo.GetByID(prjTypeLink.ProjectID);
                    project.ProjectStatus = prjTypeLink.ProjectStatus;
                    projectRepo.Update(project);

                    if (prjTypeLink.prjTypeLinks.Count() > 0)
                    {
                        foreach (var item in prjTypeLink.prjTypeLinks)
                        {
                            ProjectTypeLink model = projectTypeLinkRepo.GetByID(item.ProjectTypeLinkID);
                            model.ProjectTypeID = item.ID;
                            model.ProjectID = prjTypeLink.ProjectID;
                            model.LeaderID = item.LeaderID;
                            model.Selected = item.Selected;
                            if (model.ID > 0) projectTypeLinkRepo.Update(model);
                            else projectTypeLinkRepo.Create(model);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(prjTypeLink.Situlator))
                    {
                        ProjectCurrentSituation situation = new ProjectCurrentSituation();
                        situation.ProjectID = prjTypeLink.ProjectID;
                        situation.EmployeeID = prjTypeLink.GlobalEmployeeId;
                        situation.DateSituation = DateTime.Now;
                        situation.ContentSituation = prjTypeLink.Situlator;
                        projectCurrentSituationRepo.Create(situation);
                    }
                }

                return Ok(ApiResponseFactory.Success(true, "Lưu Leader dự án thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }

        // lưu đợ ưu tiên dự án
        [HttpPost("save-project-priority")]
        public async Task<IActionResult> SaveProjectPriority([FromBody] ProjectPriority projectPriority)
        {
            try
            {
                ProjectPriority model = projectPriority.ID > 0 ? projectPriorityRepo.GetByID(projectPriority.ID) : new ProjectPriority();
                model.Code = projectPriority.Code;
                model.ProjectCheckpoint = projectPriority.ProjectCheckpoint;
                model.Rate = projectPriority.Rate;
                model.Score = projectPriority.Score;
                model.Priority = projectPriority.Rate * projectPriority.Score / 100;
                model.ParentID = projectPriority.ParentID;
                model.UpdatedDate = DateTime.Now;

                if (projectPriority.ID > 0)
                {
                    projectPriorityRepo.Update(model);
                }
                else
                {
                    model.CreatedDate = DateTime.Now;
                    projectPriorityRepo.Create(model);
                }
                return Ok(ApiResponseFactory.Success(true, "Lưu mức độ ưu tiên dự án thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }

        // lưu trạng thái dự án
        [HttpPost("save-project-statuses")]
        public async Task<IActionResult> SaveProjectStatus([FromBody] List<ProjectStatusDetail> projectStatuses)
        {
            try
            {
                if (projectStatuses.Count() > 0)
                {
                    int id = (int)projectStatuses[0].ProjectID;
                    Model.Entities.Project project = projectRepo.GetByID(id);
                    foreach (var item in projectStatuses)
                    {
                        ProjectStatusDetail model = item.ID > 0 ? projectStatusDetailRepo.GetByID(item.ID) : new ProjectStatusDetail();
                        model.ProjectID = id;
                        model.ProjectStatusID = item.ProjectStatusID;
                        model.EstimatedEndDate = item.EstimatedEndDate;
                        model.EstimatedStartDate = item.EstimatedStartDate;
                        model.ActualEndDate = item.ActualEndDate;
                        model.ActualStartDate = item.ActualStartDate;
                        model.STT = model.STT;
                        model.Selected = item.Selected;

                        if (model.Selected == true)
                        {
                            project.ProjectStatus = (int)model.ProjectStatusID;
                            projectRepo.Update(project);
                        }

                        if (model.ID > 0)
                        {
                            projectStatusDetailRepo.Update(model);
                        }
                        else
                        {
                            projectStatusDetailRepo.Create(model);
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(true, "Lưu trạng thái dự án thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Xóa mức độ ưu tiên dự án
        [HttpPost("deleted-project-priority")]
        public async Task<IActionResult> DeletedProjectPriority([FromBody] List<int> projectPriorityIds)
        {
            try
            {
                foreach (int id in projectPriorityIds)
                {
                    if (id > 0)
                    {
                        projectPriorityRepo.Delete(id);
                    }
                }
                return Ok(ApiResponseFactory.Success(true, "Xóa mức độ ưu tiên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        // Lưu độ ưu tiên cá nhân
        [HttpPost("save-project-personal-priority")]
        public async Task<IActionResult> SaveProjectPersonalPriority([FromBody] ProjectPersonalPriotityDTO projectPersonalPriotity)
        {
            try
            {
                if (projectPersonalPriotity.ProjectIDs.Count() > 0)
                {
                    foreach (int id in projectPersonalPriotity.ProjectIDs)
                    {
                        var prjPersonal = projectPersonalPriotityRepo.GetAll()
                    .Where(x => x.ProjectID == id && x.UserID == projectPersonalPriotity.UserID)
                    .FirstOrDefault();

                        ProjectPersonalPriotity model = prjPersonal == null ? new ProjectPersonalPriotity() : projectPersonalPriotityRepo.GetByID(prjPersonal.ID);
                        model.ProjectID = id;
                        model.UserID = projectPersonalPriotity.UserID;
                        model.Priotity = projectPersonalPriotity.Priotity;
                        if (model.ID > 0)
                        {
                            await projectPersonalPriotityRepo.UpdateAsync(model);
                        }
                        else
                        {
                            await projectPersonalPriotityRepo.CreateAsync(model);
                        }
                    }
                }


                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lưu độ ưu chuyển hạng mục công việc 
        [HttpPost("save-project-work-report")]
        public async Task<IActionResult> SaveProjectWorkReport([FromBody] ProjectWorkReportDTO projectPersonalPriotity)
        {
            try
            {
                foreach (int id in projectPersonalPriotity.reportIDs)
                {
                    DailyReportTechnical model = dailyReportTechnicalRepo.GetByID(id);
                    model.ProjectID = projectPersonalPriotity.ProjectIDNew;
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = "";
                    await dailyReportTechnicalRepo.UpdateAsync(model);
                }
                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lưu người tham gia dự án
        [HttpPost("save-project-employee")]
        public async Task<IActionResult> SaveProjectEmployee([FromBody] ProjectEmployeeDTO prjEmployees)
        {
            try
            {
                foreach (ProjectEmployee item in prjEmployees.prjEms)
                {
                    int id = item.ID;
                    ProjectEmployee model = id > 0 ? projectEmployeeRepo.GetByID(id) : new ProjectEmployee();
                    model.STT = item.STT;
                    model.ProjectID = prjEmployees.ProjectID;
                    model.EmployeeID = item.EmployeeID;
                    model.ProjectTypeID = item.ProjectTypeID;
                    model.ReceiverID = item.ReceiverID;
                    model.IsLeader = item.IsLeader;
                    model.Note = item.Note;
                    model.IsDeleted = false;

                    if (id > 0)
                    {
                        await projectEmployeeRepo.UpdateAsync(model);
                    }
                    else
                    {
                        await projectEmployeeRepo.CreateAsync(model);
                    }

                    if (model.ReceiverID > 0)
                    {
                        List<ProjectEmployee> projectEmployee = projectEmployeeRepo.GetAll().
                            Where(x => x.ProjectID == model.ProjectID && x.EmployeeID == model.ReceiverID && x.IsDeleted != true).
                            ToList();

                        if (projectEmployee.Count() <= 0)


                        {
                            ProjectEmployee prjEm = new ProjectEmployee();

                            prjEm.STT = prjEmployees.prjEms.Count() + 1;
                            prjEm.ProjectID = prjEmployees.ProjectID;
                            prjEm.EmployeeID = model.ReceiverID;
                            prjEm.ReceiverID = 0;
                            prjEm.IsLeader = false;
                            prjEm.IsDeleted = false;
                            await projectEmployeeRepo.CreateAsync(prjEm);
                        }
                    }
                }

                if (prjEmployees.deletedIds.Count() > 0)
                {
                    foreach (int id in prjEmployees.deletedIds)
                    {
                        ProjectEmployee model = projectEmployeeRepo.GetByID(id);
                        model.IsDeleted = true;
                        await projectEmployeeRepo.UpdateAsync(model);
                    }
                }

                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region
        [HttpPost("save-firm-base")]
        public async Task<IActionResult> SaveFirmBase([FromBody] FirmBase firmBase)
        {
            try
            {    
                var exists = firmBaseRepo.GetAll()
                    .Where(x => x.FirmCode == firmBase.FirmCode
                                && x.ID != firmBase.ID ).ToList();
                if (exists.Count > 0)
                {
                    return Ok(new { status = 0, message = $"Mã hãng [{firmBase.FirmName}] đã tồn tại!" });
                }
             
                if (firmBase.ID > 0)
                    {
                        await firmBaseRepo.UpdateAsync(firmBase);
                    }
                    else
                    {
                        await firmBaseRepo.CreateAsync(firmBase);
                    }
                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
    }
}