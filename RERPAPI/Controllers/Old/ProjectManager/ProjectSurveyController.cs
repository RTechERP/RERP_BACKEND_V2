using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Diagnostics;

namespace RERPAPI.Controllers.Old.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectSurveyController : ControllerBase
    {
        #region Khai báo biến
        ProjectRepo projectRepo = new ProjectRepo();
        CustomerRepo customerRepo = new CustomerRepo();
        ProjectSurveyDetailRepo projectSurveyDetailRepo = new ProjectSurveyDetailRepo();
        ProjectSurveyRepo projectSurveyRepo = new ProjectSurveyRepo();
        ProjectSurveyFileRepo projectSurveyFileRepo = new ProjectSurveyFileRepo();
        #endregion

        #region Lấy danh sách tiến độ công việc
        [HttpGet("get-project-survey")]
        public async Task<IActionResult> getProjectSurvey(DateTime dateStart, DateTime dateEnd, int projectId, int technicalId, int saleId, string? keyword)
        {
            try
            {
                var dtx = projectRepo.GetAll();
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var dt = SQLHelper<object>.ProcedureToList("spGetProjectSurvey",
                                                    new string[] { "@DateStart", "@DateEnd", "@ProjectID", "@EmployeeRequestID", "@EmployeeTechID", "@Keyword" },
                                                    new object[] { ds, de, projectId, saleId, technicalId, "" });
                var data = SQLHelper<object>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region Duyệt/ hủy duyệt gấp
        [HttpGet("approved-urgent")]
        public async Task<IActionResult> ApprovedUrgent(bool approvedStatus, string loginName, int globalEmployeeId, [FromQuery] int[] ids)
        {
            try
            {
                if (ids.Count() > 0)
                {
                    foreach (int id in ids)
                    {
                        ProjectSurvey model = projectSurveyRepo.GetByID(id);
                        model.IsApprovedUrgent = approvedStatus;
                        model.ApprovedUrgentID = globalEmployeeId;
                        model.UpdatedBy = loginName;
                        model.UpdatedDate = DateTime.Now;
                        await projectSurveyRepo.UpdateAsync(model);
                    }
                }

                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region Duyệt/ hủy duyệt yêu cầu
        [HttpGet("approved-request")]
        public async Task<IActionResult> approvedRequest(int id, bool status, int employeeID, DateTime dateSurvey, string? reasonCancel, string updatedBy, int surveySession)
        {
            try
            {
                ProjectSurveyDetail model = projectSurveyDetailRepo.GetByID(id);
                model.Status = status ? 1 : 0;
                model.EmployeeID = employeeID;
                model.DateSurvey = dateSurvey;
                model.ReasonCancel = reasonCancel;
                model.UpdatedBy = updatedBy;
                model.UpdatedDate = DateTime.Now;
                model.SurveySession = surveySession;
                await projectSurveyDetailRepo.UpdateAsync(model);

                return Ok(ApiResponseFactory.Success(1
                    , ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region Load dữ liệu chi tiết leader duyệt khảo sát
        [HttpGet("get-tb-detail")]
        public async Task<IActionResult> gettbdetail(int projectSurveyId, int projectId)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProjectSurveyDetail",
                                                    new string[] { "@ProjectSurveyID", "@ProjectID" },
                                                    new object[] { projectSurveyId, projectId });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region Lấy danh sách file đính kém 
        [HttpGet("get-files")]
        public async Task<IActionResult> getfiles(int projectSurveyId)
        {
            try
            {
                var data = projectSurveyFileRepo.GetAll().Where(x => x.ProjectSurveyID == projectSurveyId);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region Lấy thông tin chi tiết khảo sát
        [HttpGet("get-detail")]
        public async Task<IActionResult> getdetail(int projectSurveyId)
        {
            try
            {
                var data = projectSurveyRepo.GetByID(projectSurveyId);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region Lưu thông tin khảo sát dự án 
        [HttpPost("save-project-survey")]
        public async Task<IActionResult> saveprojectsurvey(ProjectSurveyDTO projectSurveyDTO)
        {
            try
            {
                ProjectSurvey model = projectSurveyDTO.projectSurvey;
                if (projectSurveyDTO.projectSurvey.ID > 0)
                {
                    await projectSurveyRepo.UpdateAsync(model);
                }
                else
                {
                    projectSurveyRepo.Create(model);
                }

                if (projectSurveyDTO.projectSurveyDetails.Count() > 0)
                {
                    foreach (var item in projectSurveyDTO.projectSurveyDetails)
                    {

                        ProjectSurveyDetail modelDetail = item;
                        modelDetail.ProjectSurveyID = model.ID;
                        if (item.ID > 0)
                        {
                            await projectSurveyDetailRepo.UpdateAsync(modelDetail);
                        }
                        else
                        {
                            await projectSurveyDetailRepo.CreateAsync(modelDetail);
                        }
                    }
                }

                if (projectSurveyDTO.deletedFiles.Count() > 0)
                {
                    // Mở comment khi chạy chính thức do không kết nối với sever
                    var url = $"http://14.232.152.154:8083/api/Home/removefile?path=";
                    var client = new HttpClient();
                    foreach (var item in projectSurveyDTO.deletedFiles)
                    {
                        {
                            projectSurveyFileRepo.Delete(item);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(model.ID, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region Lưu thông tin file đính kèm dự án 
        [HttpPost("save-project-survey-files")]
        public async Task<IActionResult> saveprojectsurveyfiles([FromForm] int projectSurveyID, [FromForm] int year, [FromForm] string projectCode, [FromForm] List<IFormFile> files)
        {
            try
            {
                string pathServer = @"\\192.168.1.190\duan\Projects\";
                string pathPattern = $@"{year}\{projectCode}\TaiLieuChung\ThongTinKhaoSat";
                string pathUpload = Path.Combine(pathServer, pathPattern);
                var client = new HttpClient();
                List<ProjectSurveyFile> listFiles = new List<ProjectSurveyFile>();
                foreach (var file in files)
                {
                    ProjectSurveyFile fileOrder = new ProjectSurveyFile();
                    fileOrder.ProjectSurveyID = projectSurveyID;
                    fileOrder.FileName = file.Name;
                    fileOrder.OriginPath = "";//file.DirectoryName không lấy được đường dẫn file từ trình duyệt do bảo mật
                    fileOrder.ServerPath = pathUpload;

                    if (file.Length < 0) continue;

                    var fileStream = new FileStream(file.Name, FileMode.Open);
                    byte[] bytes = new byte[file.Length];
                    fileStream.Read(bytes, 0, (int)file.Length);
                    var byteArrayContent = new ByteArrayContent(bytes);

                    MultipartFormDataContent content = new MultipartFormDataContent();
                    content.Add(byteArrayContent, "file", file.Name);

                    var url = $"http://14.232.152.154:8083/api/Home/uploadfile?path={pathUpload}";
                    var result = await client.PostAsync(url, content);
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        projectSurveyFileRepo.Create(fileOrder);
                    }
                }

                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region Xem thông tin file đính kèm
        [HttpGet("see-file")]
        public async Task<IActionResult> seefile(int year, int projectTypeID, string projectCode)
        {
            try
            {
                //if (Global.IsOnline && string.IsNullOrEmpty(_ConnectionString)) pathLocation = @"\\14.232.152.154\duan\Projects";
                //else if (!string.IsNullOrEmpty(_ConnectionString)) pathLocation = @"D:\LeTheAnh\RTC\Project"; //Thư mục test local
                //if (Global.IsOnline)
                //{
                //    pathLocation = @"\\14.232.152.154\duan\Projects";
                //}

                string pathLocation = @"\\192.168.1.190\duan\Projects"; //Thư mục trên server
                string path = $@"{pathLocation}\{year}\{projectCode}";
                var dt = SQLHelper<object>.ProcedureToList("sp_GetProjectTypeTreeFolder",
                                                        new string[] { "@ProjectTypeID" }, new object[] { projectTypeID });
                var folders = SQLHelper<object>.GetListData(dt, 0);
                ProjectTreeFolder parentFolder = folders.Where(x => Convert.ToInt32(x.ParentID) == 0).FirstOrDefault() ?? new ProjectTreeFolder();
                if (string.IsNullOrWhiteSpace(parentFolder.FolderName)) return Ok(ApiResponseFactory.Success(1, ""));
                string folder = Path.Combine(path, parentFolder.FolderName, "KetQuaKhaoSat");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                Process.Start(folder);

                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region Lấy chi tiết khảo sát theo Id master
        [HttpGet("check-status-detail")]
        public async Task<IActionResult> checkstatusdetail(int projectSurveyId)
        {
            try
            {
                var data = projectSurveyDetailRepo.GetAll(x => x.ProjectSurveyID == projectSurveyId && x.Status == 1);
                bool isConfirm = data.Count() > 0 ? true : false;

                return Ok(ApiResponseFactory.Success(isConfirm, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion
        #region Xóa khảo sát dự án
        [HttpGet("deleted-project-survey")]
        public async Task<IActionResult> deletedprojectsurvey(int projectSurveyId)
        {
            try
            {
                ProjectSurvey model = projectSurveyRepo.GetByID(projectSurveyId);
                model.IsDeleted = true;
                model.UpdatedBy = ""; // Chưa có tên người đăng nhập
                model.UpdatedDate = DateTime.Now;
                await projectSurveyRepo.UpdateAsync(model);

                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region cây thư mục
        [HttpGet("open-folder")]
        public async Task<IActionResult> openfolder(int year, string projectCode)
        {
            try
            {
                //if (Global.IsOnline && string.IsNullOrEmpty(_ConnectionString)) pathLocation = @"\\14.232.152.154\duan\Projects";
                //else if (!string.IsNullOrEmpty(_ConnectionString)) pathLocation = @"D:\LeTheAnh\RTC\Project"; //Thư mục test local
                //if (Global.IsOnline)
                //{
                //    pathLocation = @"\\14.232.152.154\duan\Projects";
                //}

                string pathLocation = @"\\192.168.1.190\duan\Projects"; //Thư mục trên server
                //if (Global.IsOnline) // chưa có 
                //{
                //    //pathLocation = @"\\rtctechnologydata.ddns.net\DUAN\Projects\";
                //    pathLocation = @"\\14.232.152.154\DUAN\Projects\";
                //}

                try
                {
                    Directory.CreateDirectory(pathLocation);
                }

                catch (Exception)
                {
                    pathLocation = @"\\rtctechnologydata.ddns.net\DUAN\Projects\";
                }

                string pathPattern = $@"DUAN\Projects\{year}\{projectCode}\TaiLieuChung\ThongTinKhaoSat";

                string path = Path.Combine(pathLocation, pathPattern);
                Process.Start(path);

                try
                {
                    Process.Start(path);
                }
                catch
                {
                    pathPattern = $@"{year}\{projectCode}\TaiLieuChung\ThongTinKhaoSat";
                    path = Path.Combine(pathLocation, pathPattern);
                    Process.Start(path);
                }



                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region Lấy chi tiết khảo sát theo Id detail
        [HttpGet("get-detail-byid")]
        public async Task<IActionResult> getdetailbyid(int projectSurveyDetailId)
        {
            try
            {
                var dt = projectSurveyDetailRepo.GetByID(projectSurveyDetailId);
                if (dt.ID <= 0)
                {
                    return BadRequest();
                }
                List<ProjectSurveyFile> files = projectSurveyFileRepo.GetAll().Where(x => x.ProjectSurveyDetailID == projectSurveyDetailId).ToList();

                var data = new
                {
                    detail = dt,
                    files
                };

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion
        #region Lưu nội dung kết quả khảo sát dự án
        [HttpPost("save-project-survey-result")]
        public async Task<IActionResult> saveprojectsurveyresult([FromForm] int projectSurveyId, [FromForm] int projectId, [FromForm] int projectSurveyDetailId, [FromForm] string result,
            [FromForm] int projectTypeId, [FromForm] List<IFormFile> files)
        {
            try
            {
                ProjectSurveyDetail modelDetail = projectSurveyDetailRepo.GetByID(projectSurveyDetailId);
                modelDetail.Result = result;
                await projectSurveyDetailRepo.UpdateAsync(modelDetail);

                // Upload file
                // nhớ sửa lại đường dẫn này
                //ConfigSystemModel config = SQLHelper<ConfigSystemModel>.FindByAttribute("KeyName", "PathPaymentOrder").FirstOrDefault();
                //if (config == null || string.IsNullOrEmpty(config.KeyValue))
                //{
                //    MessageBox.Show("Vui lòng chọn đường dẫn lưu trên server!", "Thông báo");
                //    return;
                //}
                ProjectSurvey model = projectSurveyRepo.GetByID(projectSurveyId);
                Model.Entities.Project prj = projectRepo.GetByID(projectId);

                var dt = SQLHelper<object>.ProcedureToList("sp_GetProjectTypeTreeFolder",
                                                                new string[] { "@ProjectTypeID" },
                                                                new object[] { projectTypeId });
                var lstPath = SQLHelper<object>.GetListData(dt, 0).Where(x => x.ParentID == 0).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(lstPath.FolderName)) return BadRequest();

                string pathPattern = $@"\\192.168.1.190\duan\Projects\{prj.CreatedDate.Value.Year}\{prj.ProjectCode}\{lstPath.FolderName}\KetQuaKhaoSat";


                var client = new HttpClient();

                List<ProjectSurveyFile> listFiles = new List<ProjectSurveyFile>();
                foreach (var file in files)
                {
                    ProjectSurveyFile fileModel = new ProjectSurveyFile();
                    fileModel.ProjectSurveyDetailID = projectSurveyDetailId;
                    fileModel.FileName = file.Name;
                    fileModel.OriginPath = "";
                    fileModel.ServerPath = pathPattern;

                    if (file.Length < 0) continue;

                    var fileStream = new FileStream(file.Name, FileMode.Open);
                    byte[] bytes = new byte[file.Length];
                    fileStream.Read(bytes, 0, (int)file.Length);
                    var byteArrayContent = new ByteArrayContent(bytes);

                    MultipartFormDataContent content = new MultipartFormDataContent();
                    content.Add(byteArrayContent, "file", file.Name);

                    var url = $"http://14.232.152.154:8083/api/Home/uploadfile?path={pathPattern}";
                    var rs = await client.PostAsync(url, content);
                    if (rs.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        projectSurveyFileRepo.Create(fileModel);
                    }
                }

                return Ok(ApiResponseFactory.Success(1, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        class ProjectTypeFilePath
        {
            public int ID { get; set; }
            public int ParentID { get; set; }
            public string FolderName { get; set; }
        }
        #endregion



    }
}
