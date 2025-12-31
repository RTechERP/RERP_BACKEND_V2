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
    public class NewsletterController : ControllerBase
    {
        private readonly NewsletterRepo _newsletterRepo;
        private readonly NewsletterFileRepo _newsletterFileRepo;

        public NewsletterController(NewsletterRepo newsletterRepo, NewsletterFileRepo newsletterFileRepo)
        {
            _newsletterRepo = newsletterRepo;
            _newsletterFileRepo = newsletterFileRepo;
        }


        //[HttpPost]
        //public IActionResult GetNesletter(NewsletterParam param)
        //{
        //    try
        //    {
        //        var ds = param.FromDate.Date; ;
        //        var de = param.ToDate.Date.AddDays(+1).AddSeconds(-1);

        //        var newsletters = SQLHelper<object>.ProcedureToList("sp_GetNewsletters", new string[] { "@Keyword", "@TypeId", "@FromDate", "@ToDate" },
        //            new object[] { param.Keyword, param.TypeId, ds, de });

        //        var newslettersResult = SQLHelper<object>.GetListData(newsletters, 0);

        //        // Lấy tất cả NewsletterID
        //        var newsletterIds = newslettersResult.Select(n => n.ID).ToList();

        //        var newsletterIdsString = string.Join(",", newsletterIds);

        //        var newsletterFiles = SQLHelper<object>.ProcedureToList("spGetNewsletterFileByNesletterID", new string[] { "@Text" },
        //            new object[] { newsletterIdsString });

        //        var newsletterFilesResult = SQLHelper<object>.GetListData(newsletterFiles, 0);

        //    // 4. Group files by NewsletterID
        //    var filesByNewsletterId = newsletterFilesResult
        //        .Where(f => f["NewsletterID"] != null)
        //        .GroupBy(f => Convert.ToInt32(f["NewsletterID"]))
        //        .ToDictionary(
        //            g => g.Key,
        //            g => g.Select(f => new NewsletterFile
        //            {
        //                ID = f["ID"] != null ? Convert.ToInt32(f["ID"]) : 0,
        //                NewsletterID = f["NewsletterID"] != null ? Convert.ToInt32(f["NewsletterID"]) : 0,
        //                FileName = f["FileName"]?.ToString(),
        //                ServerPath = f["ServerPath"]?.ToString(),
        //                OriginPath = f["OriginPath"]?.ToString(),
        //                CreatedBy = f["CreatedBy"]?.ToString(),
        //                CreatedDate = f["CreatedDate"] != null ? Convert.ToDateTime(f["CreatedDate"]) : (DateTime?)null,
        //                UpdatedBy = f["UpdatedBy"]?.ToString(),
        //                UpdatedDate = f["UpdatedDate"] != null ? Convert.ToDateTime(f["UpdatedDate"]) : (DateTime?)null,
        //                IsDeleted = f["IsDeleted"] != null ? Convert.ToBoolean(f["IsDeleted"]) : (bool?)null

        //            }).ToList()
        //        );
        //    // 5. Tạo kết quả cuối cùng
        //    var finalResult = newslettersResult.Select(n =>
        //    {
        //        var newsletterId = Convert.ToInt32(n["ID"]);
        //        List<NewsletterFile> files = filesByNewsletterId.ContainsKey(newsletterId)
        //            ? filesByNewsletterId[newsletterId]
        //            : new List<NewsletterFile>();

        //        // Tạo chuỗi file links
        //        var fileLinks = string.Join(", ", files
        //            .Where(f => f.ServerPath != null && !string.IsNullOrEmpty(f.ServerPath.Trim()))
        //            .Select(f => f.ServerPath.Trim()));

        //        return new NewsletterWithFilesDTO
        //        {
        //            ID = newsletterId,
        //            Code = n["Code"]?.ToString(),
        //            Title = n["Title"]?.ToString(),
        //            NewsletterContent = n["NewsletterContent"]?.ToString(),
        //            Type = n["Type"] != null ? Convert.ToInt32(n["Type"]) : (int?)null,
        //            Image = n["Image"]?.ToString(),
        //            CreatedBy = n["CreatedBy"]?.ToString(),
        //            CreatedDate = n["CreatedDate"] != null ? Convert.ToDateTime(n["CreatedDate"]) : (DateTime?)null,
        //            UpdatedBy = n["UpdatedBy"]?.ToString(),
        //            UpdatedDate = n["UpdatedDate"] != null ? Convert.ToDateTime(n["UpdatedDate"]) : (DateTime?)null,
        //            IsDeleted = n["IsDeleted"] != null ? Convert.ToInt32(n["IsDeleted"]) : (int?)null,
        //            OriginImgPath = n["OriginImgPath"]?.ToString(),
        //            ServerImgPath = n["ServerImgPath"]?.ToString(),
        //            NewsletterTypeCode = n["NewsletterTypeCode"]?.ToString(),
        //            NewsletterTypeName = n["NewsletterTypeName"]?.ToString(),
        //            NewsletterFiles = files,

        //        };
        //    }).ToList();

        //    return Ok(ApiResponseFactory.Success(finalResult, ""));
        //    //return Ok(ApiResponseFactory.Success(result, ""));
        //}
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

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

        //[HttpPost("delete-newsletter-folder-by-id")]
        //public async Task<IActionResult> DeleteNewsletterFolderByID(int id)
        //{
        //    try
        //    {
        //        var newsletter = _newsletterFileRepo.GetAll(x => x.ID == id).FirstOrDefault();

        //        if (newsletter == null)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Newsletter không tồn tại."));
        //        }
        //        newsletter.IsDeleted = true;
        //        var result = await _newsletterFileRepo.UpdateAsync(newsletter);

        //        return Ok(ApiResponseFactory.Success(null, "Xóa newsletter thành công!"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi xóa newsletter: {ex.Message}"));
        //    }
        //}


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
