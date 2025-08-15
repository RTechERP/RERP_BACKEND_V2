using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
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

namespace RERPAPI.Controllers.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpGet("getfolders")]
        public async Task<IActionResult> getfolders()
        {
            try
            {
                List<ProjectTreeFolder> projectTreeFolders = projectTreeFolderRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = projectTreeFolders
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

        // Danh sách nhân viên khi thêm dự án lấy table 1
        [HttpGet("getpms")]
        public async Task<IActionResult> getpms()
        {
            try
            {
                var pms = getDataUser(0);
                return Ok(new
                {
                    status = 1,
                    data = pms
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

        // Lấy danh sách khách hàng
        [HttpGet("getcustomers")]
        public async Task<IActionResult> getcustomers()
        {
            try
            {
                List<Customer> customers = customerRepo.GetAll().Where(x => x.IsDeleted == false).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(new
                {
                    status = 1,
                    data = customers
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

        // Danh sách nhân viên khi thêm dự án lấy table 2 phụ trách sale/ phụ trách kỹ thuật/ leader
        [HttpGet("getusers")]
        public async Task<IActionResult> getusers()
        {
            try
            {
                var users = getDataUser(1);
                return Ok(new
                {
                    status = 1,
                    data = users
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

        // Danh sách loại dự án 
        [HttpGet("getprojecttypes")]
        public async Task<IActionResult> getprojecttypes()
        {
            try
            {
                List<ProjectType> projectTypes = projectTypeRepo.GetAll().Where(x => x.ID != 4).ToList();
                return Ok(new
                {
                    status = 1,
                    data = projectTypes
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

        // Danh sách loại dự án ProjectTypeLink
        [HttpGet("getprojecttypelinks/{id}")]
        public async Task<IActionResult> getprojecttypelinks(int id)
        {
            try
            {
                //List<ProjectTypeLinkDTO> projectTypeLinkDTOs = SQLHelper<ProjectTypeLinkDTO>
                //    .ProcedureToList("spGetProjectTypeLink", new string[] { "@ProjectID" }, new object[] { id });
                var projectTypeLinkDTOs = SQLHelper<object>.ProcedureToList("spGetProjectTypeLink", new string[] { "@ProjectID" }, new object[] { id });
                return Ok(new

                {
                    status = 1,
                    data = projectTypeLinkDTOs.FirstOrDefault()
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

        // Load Hạng mục công việc
        [HttpGet("getprojectitems")]
        public async Task<IActionResult> getprojectitems(int id)
        {
            try
            {
                if (id <= 0) id--;

                //List<ProjectItemDTO> projectItems = SQLHelper<ProjectItemDTO>
                //    .ProcedureToList("spGetProjectItem", new string[] { "@ProjectID" }, new object[] { id });
                var projectItems = SQLHelper<object>.ProcedureToList("spGetProjectItem", new string[] { "@ProjectID" }, new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = projectItems[0]
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


        // Danh sách trạng thái dự án 
        [HttpGet("getprojectstatus")]
        public async Task<IActionResult> getprojectstatus()
        {
            try
            {
                List<ProjectStatus> projectStatus = projectStatusRepo.GetAll().OrderBy(x => x.STT).ToList();
                return Ok(new
                {
                    status = 1,
                    data = projectStatus
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

        // Danh sách lĩnh vực kinh doanh
        [HttpGet("getbusinessfields")]
        public async Task<IActionResult> getbusinessfields()
        {
            try
            {
                List<BusinessField> businessFields = businessFieldRepo.GetAll().OrderBy(x => x.STT).ToList();
                return Ok(new
                {
                    status = 1,
                    data = businessFields
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

        // Danh sách dự án 
        [HttpGet("getprojects")]
        public async Task<IActionResult> getprojects(int size, int page,
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
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(projects, 0),
                    totalPage = SQLHelper<object>.GetListData(projects, 1).FirstOrDefault().TotalPage
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

        // Lấy danh sách dự án chi tiết ở dưới
        [HttpGet("getprojectdetails/{id}")]
        public async Task<IActionResult> getprojectdetails(int id)
        {
            // Update store spGetProjectDetail p.Note => p.Note AS ProjectNote 
            try
            {
                //List<ProjectDetailDTO> projectDetails = SQLHelper<ProjectDetailDTO>
                //    .ProcedureToList("spGetProjectDetail", new string[] { "@ID" }, new object[] { id });
                var projectDetails = SQLHelper<object>.ProcedureToList("spGetProjectDetail", new string[] { "@ID" }, new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = projectDetails
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



        // Lấy danh sách dự án chi tiết ở dưới
        [HttpGet("getproject/{id}")]
        public async Task<IActionResult> getproject(int id)
        {
            try
            {
                var projectDetail = projectRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = projectDetail
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

        // Lấy hiện trạng dự án 
        [HttpGet("getprojectcurrentsituation/{projectId}/{employeeId}")]
        public async Task<IActionResult> getprojectcurrentsituation(int projectId, int employeeId)
        {
            try
            {
                ProjectCurrentSituation projectDetail = projectCurrentSituationRepo.GetAll()
                    .Where(x => x.ProjectID == projectId && x.EmployeeID == employeeId)
                    .OrderByDescending(x => x.DateSituation).FirstOrDefault();
                projectDetail = projectDetail ?? new ProjectCurrentSituation();

                return Ok(new
                {
                    status = 1,
                    data = projectDetail.ContentSituation
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

        // modal lấy danh sách nhóm file
        [HttpGet("getgroupfiles")]
        public async Task<IActionResult> getgroupfiles()
        {
            try
            {
                List<GroupFile> grf = groupFileRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = grf
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

        // modal lấy danh sách FirmBase
        [HttpGet("getfirmbases")]
        public async Task<IActionResult> getfirmbases()
        {
            try
            {
                List<FirmBase> firmBases = firmBaseRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = firmBases
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
        // modal lấy kiểu dự án Base
        [HttpGet("getprojecttypeBases")]
        public async Task<IActionResult> getprojecttypeBases()
        {
            try
            {
                List<ProjectTypeBase> pfb = projectTypeBaseRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = pfb
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

        // modal lấy người dùng dự án 
        [HttpGet("getprojectusers/{id}")]
        public async Task<IActionResult> getprojectusers(int id)
        {
            try
            {
                var projectUser = SQLHelper<object>.ProcedureToList("spGetProjectUser", new string[] { "@ID" }, new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = projectUser.FirstOrDefault()
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

        //modal lấy dữ liệu FollowProjectBase
        [HttpGet("getfollowprojectbases/{id}")]
        public async Task<IActionResult> getfollowprojectbases(int id)
        {
            try
            {
                FollowProjectBase followProjectBase = followProjectBaseRepo
                    .GetAll().Where(x => x.ProjectID == id)
                    .OrderByDescending(x => x.ExpectedPlanDate).FirstOrDefault();
                return Ok(new
                {
                    status = 1,
                    data = followProjectBase
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

        //modal lấy dữ liệu PriorityType
        [HttpGet("getprioritytype")]
        public async Task<IActionResult> getprioritytype()
        {
            try
            {
                var projectType = projectPriorityRepo.GetAll().Where(x => x.ParentID == 0).ToList();
                return Ok(new
                {
                    status = 1,
                    data = projectType
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

        //modal lấy dữ liệu PriorityType
        [HttpGet("getprojectprioritydetail/{id}")]
        public async Task<IActionResult> getprojectprioritydetail(int id)
        {
            try
            {
                var projectType = projectPriorityRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = projectType
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

        //modal lấy dữ liệu priorytipersion
        [HttpGet("getpersonalpriority/{projectId}/{userId}")]
        public async Task<IActionResult> getpersonalpriority(int projectId, int userId)
        {
            try
            {
                int prio = 0;
                var priority = projectPersonalPriotityRepo.GetAll().Where(x => x.ProjectID == projectId && x.UserID == userId).FirstOrDefault();
                if (priority != null)
                {
                    prio = (int)priority.Priotity;
                }
                return Ok(new
                {
                    status = 1,
                    data = prio
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

        //modal kiểm tra mã ưu tiê
        [HttpGet("checkprojectpriority/{id}/{code}")]
        public async Task<IActionResult> checkprojectpriority(int id, string code)
        {
            try
            {
                bool check = false;
                var projectPriority = projectPriorityRepo.GetAll().Where(x => x.ID != id && x.Code == code);
                if (projectPriority.Count() > 0) check = true;
                return Ok(new
                {
                    status = 1,
                    data = check
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

        // Xóa dữ liệu project
        [HttpGet("deletedproject/{ids}")]
        public async Task<IActionResult> deletedproject(string ids)
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

                return Ok(new
                {
                    status = 1,
                    data = "Đã xóa dự án!"
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

        [HttpGet("checkprojectcode/{id}/{projectCode}")]
        public async Task<IActionResult> checkprojectcode(int id, string projectCode)
        {
            try
            {
                List<Project> projects = new List<Project>();
                if (id > 0)
                {
                    projects = projectRepo.GetAll().Where(x => x.ProjectCode.Contains(projectCode) && x.ID != id).ToList();
                }
                else
                {
                    projects = projectRepo.GetAll().Where(x => x.ProjectCode.Contains(projectCode)).ToList();
                }

                return Ok(new
                {
                    status = 1,
                    data = projects.Count() > 0 ? 0 : 1
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

        [HttpGet("savechangeproject/{projectIdOld}/{projectIdNew}")]
        public async Task<IActionResult> savechangeproject(int projectIdOld, int projectIdNew)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spUpdateProjectIDInDailyReportTechnical_ByNewProjectID",
                    new string[] { "@OldProjectID", "@NewProjectID" }, new object[] { projectIdOld, projectIdNew });

                return Ok(new
                {
                    status = 1,
                    data = true
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

        // lấy trạng thái dự án 
        [HttpGet("getprojectstatuss/{projectId}")]
        public async Task<IActionResult> getprojectstatus(int projectId)
        {
            try
            {
                var status = SQLHelper<object>.ProcedureToList("spGetProjectStatus", new string[] { "@ProjectID" }, new object[] { projectId });
                var data = SQLHelper<object>.GetListData(status, 0);
                return Ok(new
                {
                    status = 1,
                    data
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

        //            try
        //            {
        //                Directory.CreateDirectory(pathLocation);
        //            }
        //            catch (Exception)
        //            {
        //                pathLocation = @"\\rtctechnologydata.ddns.net\DUAN\Projects\";
        //            }

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


        // modal loadProjectCode
        [HttpGet("getprojectcodemodal/{projectId}/{customerShortName}/{projectType}")]
        public async Task<IActionResult> getprojectcodemodal(int projectId, string customerShortName, int projectType)
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

                return Ok(new
                {
                    status = 1,
                    data = returnProjectCode == "" ? projectCode : returnProjectCode
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

        // Lấy danh sách khách hàng
        [HttpGet("getuserteams")]
        public async Task<IActionResult> getuserteams()
        {
            try
            {
                var userTeams = SQLHelper<object>.ProcedureToList("spGetUserTeam", new string[] { "@DepartmentID" }, new object[] { 0 });
                var data = SQLHelper<object>.GetListData(userTeams, 1);
                return Ok(new
                {
                    status = 1,
                    data
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

        // Lấy danh sách dự án
        [HttpGet("getprojectmodal")]
        public async Task<IActionResult> getprojectmodal()
        {
            try
            {
                List<Project> prjs = projectRepo.GetAll().OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(new
                {
                    status = 1,
                    data = prjs
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

        // Lấy danh sách ưu tiên dự án
        [HttpGet("getprojectprioritymodal/{projectId}")]
        public async Task<IActionResult> getprojectprioritymodal(int projectId)
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
                return Ok(new
                {
                    status = 1,
                    data = prjPriority,
                    checks
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

        // Lấy chi tiết tổng hợp nhân công
        [HttpGet("getprojectworkersynthetic")]
        public async Task<IActionResult> getprojectworkersynthetic(int projectId, int prjWorkerTypeId, string? keyword)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProjectWokerSynthetic",
                new string[] { "@ProjectID", "@ProjectWorkerTypeID", "@Keyword" },
                new object[] { projectId, prjWorkerTypeId, keyword ?? "" });

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 0)
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

        // Lấy chi tiết tổng hợp báo cáo công việc
        [HttpGet("getprojectworkreport")]
        public async Task<IActionResult> getprojectworkreport(int page, int size, int projectId, string? keyword)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetDailyReportTechnical_New",
                new string[] { "@ProjectID", "@FilterText", "@PageSize", "@PageNumber" },
                new object[] { projectId, keyword ?? "", page, size });

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 1)
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

        [HttpGet("getworkertype")]
        public async Task<IActionResult> getworkertype()
        {
            try
            {
                var workerTypes = projectWorkerTypeRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = workerTypes
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

        #region Chức năng gười tham gia dự án
        [HttpGet("get-project-employee/{status}")]
        public async Task<IActionResult> getprojectemployee(int status)
        {
            try
            {
                var employees = SQLHelper<object>.ProcedureToList("spGetEmployee", new string[] { "@Status" }, new object[] { status });
                var data = SQLHelper<object>.GetListData(employees, 0);
                return Ok(new
                {
                    status = 1,
                    data = data
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

        [HttpGet("getstatusprojectemployee")]
        public async Task<IActionResult> getstatusprojectemployee()
        {
            try
            {
                var status = projectStatusRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = status
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

        [HttpGet("getprojecttype")]
        public async Task<IActionResult> getprojecttype()
        {
            try
            {
                var projectTpye = projectTypeRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = projectTpye
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

        [HttpGet("getemployeesuggest/{projectId}")]
        public async Task<IActionResult> getemployeesuggest(int projectId)
        {
            try
            {
                var employee = SQLHelper<object>.ProcedureToList("spGetProjectParticipant",
                                            new string[] { "@ProjectID" },
                                            new object[] { projectId });
                var data = SQLHelper<object>.GetListData(employee, 0);
                return Ok(new
                {
                    status = 1,
                    data
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

        [HttpGet("getemployeemain/{projectId}/{isDeleted}")]
        public async Task<IActionResult> getemployeemain(int projectId, int isDeleted)
        {
            try
            {
                var employee = SQLHelper<object>.ProcedureToList("spGetProjectEmployee",
                                            new string[] { "@ProjectID", "@IsDeleted" },
                                            new object[] { projectId, isDeleted });
                var data = SQLHelper<object>.GetListData(employee, 0);
                return Ok(new
                {
                    status = 1,
                    data
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

        [HttpGet("getemployeepermission/{projectId}/{employeeId}")]
        public async Task<IActionResult> getemployeepermission(int projectId, int employeeId)
        {
            try
            {
                bool havePermission = false;
                var employee = SQLHelper<object>.ProcedureToList("spGetProjectEmployeePermisstion",
                                                        new string[] { "@ProjectID", "@EmployeeID" },
                                                        new object[] { projectId, employeeId });
                var data = SQLHelper<object>.GetListData(employee, 0);

                int valueRow = (int)data[0]["RowNumber"];

                Project project = projectRepo.GetByID(projectId);

                // Check quyền sau khi có phân quyền
                //if(project.ProjectManager == Global.EmployeeID || project.UserID == Global.UserID || Global.IsAdmin || valueRow > 0 || project.UserTechnicalID == Global.UserID)
                return Ok(new
                {
                    status = 1,
                    data = havePermission
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

        #endregion


        #endregion

        #region API POST
        [HttpPost("saveproject")]
        public async Task<IActionResult> saveproject([FromBody] ProjectDTO prj)
        {
            try
            {
                int statusOld = prj.projectStatusOld;
                var pr = prj.project ?? null;
                int projectId = prj.project.ID;

                Project project = projectId > 0 ? projectRepo.GetByID(projectId) : new Project();
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

                return Ok(new
                {
                    status = 1,
                    data = true
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

        // lưu trạng thái dự án 

        [HttpPost("saveprojectstatus")]
        public async Task<IActionResult> saveprojectstatus(int Stt, string statusName)
        {
            try
            {
                ProjectStatus prjs = new ProjectStatus();
                prjs.STT = Stt;
                prjs.StatusName = statusName;
                projectStatusRepo.Create(prjs);

                return Ok(new
                {
                    status = 1,
                    data = ""
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

        // Lưu leader dự án
        [HttpPost("saveprojecttypelink")]
        public async Task<IActionResult> saveprojecttypelink([FromBody] ProjectTypeLinkDTO prjTypeLink)
        {
            try
            {
                if (prjTypeLink.ProjectID > 0)
                {
                    Project project = projectRepo.GetByID(prjTypeLink.ProjectID);
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

                return Ok(new
                {
                    status = 1,
                    data = ""
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

        // lưu đợ ưu tiên dự án
        [HttpPost("saveprojectpriority")]
        public async Task<IActionResult> saveprojectpriority([FromBody] ProjectPriority projectPriority)
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
                return Ok(new
                {
                    status = 1,
                    data = ""
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

        // lưu trạng thái dự án
        [HttpPost("saveprojectstatuses")]
        public async Task<IActionResult> saveprojectstatus([FromBody] List<ProjectStatusDetail> projectStatuses)
        {
            try
            {
                if (projectStatuses.Count() > 0)
                {
                    int id = (int)projectStatuses[0].ProjectID;
                    Project project = projectRepo.GetByID(id);
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
                return Ok(new
                {
                    status = 1,
                    data = true
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

        // Xóa mức độ ưu tiên dự án
        [HttpPost("deletedprojectpriority")]
        public async Task<IActionResult> deletedprojectpriority([FromBody] List<int> projectPriorityIds)
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
                return Ok(new
                {
                    status = 1,
                    data = true
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


        // Lưu độ ưu tiên cá nhân
        [HttpPost("saveprojectpersonalpriority")]
        public async Task<IActionResult> saveProjectPersonalPriority([FromBody] ProjectPersonalPriotityDTO projectPersonalPriotity)
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
                           await  projectPersonalPriotityRepo.UpdateAsync(model);
                        }
                        else
                        {
                            await projectPersonalPriotityRepo.CreateAsync(model);
                        }
                    }
                }


                return Ok(new
                {
                    status = 1,
                    data = true
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

        // Lưu độ ưu chuyển hạng mục công việc 
        [HttpPost("saveprojectworkreport")]
        public async Task<IActionResult> saveprojectworkreport([FromBody] ProjectWorkReportDTO projectPersonalPriotity)
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

                return Ok(new
                {
                    status = 1,
                    data = true
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

        // Lưu người tham gia dự án
        [HttpPost("save-project-employee")]
        public async Task<IActionResult> saveprojectemployee([FromBody] ProjectEmployeeDTO prjEmployees)
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

                return Ok(new
                {
                    status = 1,
                    data = true
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
        #endregion
    }
}
