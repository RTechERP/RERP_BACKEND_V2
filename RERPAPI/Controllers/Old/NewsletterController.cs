using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System.Linq;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NewsletterController : ControllerBase
    {
        private readonly NewsletterRepo _newsletterRepo;
        private readonly NewsletterFileRepo _newsletterFileRepo;

        public NewsletterController(NewsletterRepo newsletterRepo, NewsletterFileRepo newsletterFileRepo)
        {
            _newsletterRepo = newsletterRepo;
            _newsletterFileRepo = newsletterFileRepo;
        }

        [HttpGet]
        public IActionResult GetNewLetter()
        {
            try
            {
                var newsletters = SQLHelper<object>.ProcedureToList("sp_GetNewsletters",
                    new string[] { "@Keyword", "@TypeId", "@FromDate", "@ToDate" },
                    new object[] { "", 0, "", "" });
                var newslettersResult = SQLHelper<object>.GetListData(newsletters, 0) as List<dynamic>;
                return Ok(ApiResponseFactory.Success(newslettersResult, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi GET newsletter: {ex.Message}"));
            }
        }

        [HttpGet("get-limit-newsletter")]
        public IActionResult GetLimitNewLetter()
        {
            try
            {
                int limit = 5;
                var newsletters = SQLHelper<object>.ProcedureToList("sp_GetNewsletters",
                    new string[] { "@Keyword", "@TypeId", "@FromDate", "@ToDate","@Limit" },
                    new object[] { "", 0, "", "" , limit });
                var newslettersResult = SQLHelper<object>.GetListData(newsletters, 0) as List<dynamic>;
                return Ok(ApiResponseFactory.Success(newslettersResult, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi GET newsletter: {ex.Message}"));
            }
        }

        [HttpPost("get-newsletter-by-id")]
        public IActionResult GetNewLetterByID(int id)
        {
            try
            {
                var newsletter = _newsletterRepo.GetAll(x => x.ID == id).FirstOrDefault();

                return Ok(ApiResponseFactory.Success(newsletter, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi GET newsletter by id: {ex.Message}"));
            }
        }

        [HttpPost("get-newsletter-file-by-newsletterid")]
        public IActionResult GetNewLetterFileByNewsletterID(int newsletterid)
        {
            try
            {
                var newsletterFile = _newsletterFileRepo.GetAll(x => x.IsDeleted == false && x.NewsletterID == newsletterid);

                return Ok(ApiResponseFactory.Success(newsletterFile, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi GET newsletter by newsletter id: {ex.Message}"));
            }
        }

        [HttpPost("delete-newsletter-folder-by-id")]
        public async Task<IActionResult> DeleteNewsletterFolderByID(string ids)
        {
            try
            {
                List<int> idList = ids.Split(',')
                      .Select(id => int.Parse(id.Trim()))
                      .ToList();

                var newsletterFile = _newsletterFileRepo.GetAll(x => x.IsDeleted == false).Where(y => idList.Contains(y.ID));
                foreach (var item in newsletterFile)
                {
                    item.IsDeleted = true;
                    await _newsletterFileRepo.UpdateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa File thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi xóa File: {ex.Message}"));
            }
        }


        [HttpPost]
        public IActionResult GetNewsletter(NewsletterParam param)
        {
            try
            {
                var ds = param.FromDate.Date;
                var de = param.ToDate.Date.AddDays(1).AddSeconds(-1);

                var newsletters = SQLHelper<object>.ProcedureToList("sp_GetNewsletters",
                    new string[] { "@Keyword", "@TypeId", "@FromDate", "@ToDate" },
                    new object[] { param.Keyword, param.TypeId, ds, de });

                var newslettersResult = SQLHelper<object>.GetListData(newsletters, 0) as List<dynamic>;

                if (newslettersResult == null || newslettersResult.Count == 0)
                {
                    return Ok(ApiResponseFactory.Success(new List<NewsletterWithFilesDTO>(), "No data found"));
                }

                // Lấy tất cả NewsletterID
                var newsletterIds = new List<int>();
                foreach (var n in newslettersResult)
                {
                    try
                    {
                        var idValue = n.ID as object;
                        if (idValue != null && idValue != DBNull.Value)
                        {
                            newsletterIds.Add(Convert.ToInt32(idValue));
                        }
                    }
                    catch { }
                }

                // Lấy files nếu có newsletter
                Dictionary<int, List<NewsletterFile>> filesByNewsletterId = new Dictionary<int, List<NewsletterFile>>();

                if (newsletterIds.Count > 0)
                {
                    var newsletterIdsString = string.Join(",", newsletterIds);

                    var newsletterFiles = SQLHelper<object>.ProcedureToList("spGetNewsletterFileByNesletterID",
                        new string[] { "@Text" },
                        new object[] { newsletterIdsString });

                    var newsletterFilesResult = SQLHelper<object>.GetListData(newsletterFiles, 0) as List<dynamic>;

                    if (newsletterFilesResult != null && newsletterFilesResult.Count > 0)
                    {
                        foreach (dynamic f in newsletterFilesResult)
                        {
                            try
                            {
                                var newsletterIdValue = f.NewsletterID as object;
                                if (newsletterIdValue != null && newsletterIdValue != DBNull.Value)
                                {
                                    int newsletterId = Convert.ToInt32(newsletterIdValue);

                                    if (!filesByNewsletterId.ContainsKey(newsletterId))
                                    {
                                        filesByNewsletterId[newsletterId] = new List<NewsletterFile>();
                                    }

                                    filesByNewsletterId[newsletterId].Add(new NewsletterFile
                                    {
                                        ID = GetIntValue(f.ID),
                                        NewsletterID = newsletterId,
                                        FileName = GetStringValue(f.FileName),
                                        ServerPath = GetStringValue(f.ServerPath),
                                        OriginPath = GetStringValue(f.OriginPath),
                                        CreatedBy = GetStringValue(f.CreatedBy),
                                        CreatedDate = GetDateTimeValue(f.CreatedDate),
                                        UpdatedBy = GetStringValue(f.UpdatedBy),
                                        UpdatedDate = GetDateTimeValue(f.UpdatedDate),
                                        IsDeleted = GetBoolValue(f.IsDeleted)
                                    });
                                }
                            }
                            catch { }
                        }
                    }
                }

                // Tạo kết quả cuối cùng
                var finalResult = new List<NewsletterWithFilesDTO>();

                foreach (dynamic n in newslettersResult)
                {
                    try
                    {
                        var idValue = n.ID as object;
                        if (idValue == null || idValue == DBNull.Value)
                            continue;

                        int newsletterId = Convert.ToInt32(idValue);

                        List<NewsletterFile> files = filesByNewsletterId.ContainsKey(newsletterId)
                            ? filesByNewsletterId[newsletterId]
                            : new List<NewsletterFile>();

                        // Tạo chuỗi file links
                        var fileLinksList = new List<string>();
                        foreach (var file in files)
                        {
                            if (!string.IsNullOrWhiteSpace(file.ServerPath))
                            {
                                fileLinksList.Add(file.ServerPath.Trim());
                            }
                        }
                        var fileLinks = fileLinksList.Count > 0 ? string.Join(", ", fileLinksList) : "";

                        finalResult.Add(new NewsletterWithFilesDTO
                        {
                            ID = newsletterId,
                            Code = GetStringValue(n.Code),
                            Title = GetStringValue(n.Title),
                            NewsletterContent = GetStringValue(n.NewsletterContent),
                            Type = GetIntValue(n.Type),
                            Image = GetStringValue(n.Image),
                            CreatedBy = GetStringValue(n.CreatedBy),
                            CreatedDate = GetDateTimeValue(n.CreatedDate),
                            UpdatedBy = GetStringValue(n.UpdatedBy),
                            UpdatedDate = GetDateTimeValue(n.UpdatedDate),
                            IsDeleted = GetBoolValue(n.IsDeleted),
                            OriginImgPath = GetStringValue(n.OriginImgPath),
                            ServerImgPath = GetStringValue(n.ServerImgPath),
                            NewsletterTypeCode = GetStringValue(n.NewsletterTypeCode),
                            NewsletterTypeName = GetStringValue(n.NewsletterTypeName),
                            NewsletterFiles = files
                        });
                    }
                    catch { }
                }

                return Ok(ApiResponseFactory.Success(finalResult, $"Found {finalResult.Count} newsletters"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Helper methods
        private string GetStringValue(dynamic value)
        {
            var objValue = value as object;
            if (objValue == null || objValue == DBNull.Value)
                return null;
            return objValue.ToString();
        }

        private int? GetIntValue(dynamic value)
        {
            var objValue = value as object;
            if (objValue == null || objValue == DBNull.Value)
                return null;

            if (int.TryParse(objValue.ToString(), out int result))
                return result;
            return null;
        }

        private DateTime? GetDateTimeValue(dynamic value)
        {
            var objValue = value as object;
            if (objValue == null || objValue == DBNull.Value)
                return null;

            if (DateTime.TryParse(objValue.ToString(), out DateTime result))
                return result;
            return null;
        }

        private bool? GetBoolValue(dynamic value)
        {
            var objValue = value as object;
            if (objValue == null || objValue == DBNull.Value)
                return null;

            if (bool.TryParse(objValue.ToString(), out bool result))
                return result;
            return null;
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveNewsletter([FromBody] NewsletterDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ."));
                }

                // Validate và lưu Newsletter chính
                if (dto.Newsletter.ID <= 0)
                {
                    // Tạo mới
                    await _newsletterRepo.CreateAsync(dto.Newsletter);
                }
                else
                {
                    // Cập nhật
                    await _newsletterRepo.UpdateAsync(dto.Newsletter);
                }

                // Lưu danh sách files nếu có
                if (dto.NewsletterFiles != null && dto.NewsletterFiles.Any())
                {
                    foreach (var file in dto.NewsletterFiles)
                    {
                        // Gán NewsletterID cho file
                        file.NewsletterID = dto.Newsletter.ID;

                        if (file.ID <= 0)
                        {
                            await _newsletterFileRepo.CreateAsync(file);
                        }
                        else
                        {
                            await _newsletterFileRepo.UpdateAsync(file);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thông tin newsletter thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi lưu newsletter: {ex.Message}"));
            }
        }


    }
}
