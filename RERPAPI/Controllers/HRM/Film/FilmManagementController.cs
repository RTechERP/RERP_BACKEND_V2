using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.DTO.Film;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.Film;
using System.Collections.Immutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.HRM.Film
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmManagementController : ControllerBase
    {
        FilmManagementRepo _filmManagementRepo;
        FilmManagementDetailRepo _filmManagementDetailRepo;

        public FilmManagementController(FilmManagementRepo filmManagementRepo, FilmManagementDetailRepo filmManagementDetailRepo)
        {
            _filmManagementRepo = filmManagementRepo;
            _filmManagementDetailRepo = filmManagementDetailRepo;
        }
        [RequiresPermission("N1,N44")]
        [HttpGet("get-film")]
        public IActionResult GetFilm([FromQuery] string? filterText, int Size, int Page)
        {
            try
            {
                var films = SQLHelper<dynamic>.ProcedureToList("spGetFilmManagement",
                                                new string[] { "@FilterText", "@PageSize", "@PageNumber" },
                                                new object[] { filterText ?? string.Empty, Size, Page });
                int maxSTT = _filmManagementRepo.getMAXSTT();
                var data = SQLHelper<dynamic>.GetListData(films, 0);
                var TotalPage = SQLHelper<dynamic>.GetListData(films, 1);
                return Ok(ApiResponseFactory.Success(new { data, TotalPage , maxSTT}, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N44")]
        [HttpGet("get-film-detail")]
        public IActionResult GetFilmDetail(string filmManagementID)
        {
            try
            {
                var filmDetail = SQLHelper<dynamic>.ProcedureToList("spGetFilmManagementDetail",
                                                    new string[] { "@FilmManagementID", "@IsAll" },
                                                    new object[] { filmManagementID, 0 });
                var data = SQLHelper<dynamic>.GetListData(filmDetail, 0);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N44")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] FilmMangementDTO dto)
        {
            try
            {

                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ"));
                }

                if (dto.filmManagement != null && dto.filmManagement.IsDeleted != true)
                {
                    if (!_filmManagementRepo.Validate(dto.filmManagement, out string message))
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (dto.filmManagement != null )
                {
                   
                        if (dto.filmManagement.ID <= 0)
                            await _filmManagementRepo.CreateAsync(dto.filmManagement);
                        else
                        await _filmManagementRepo.UpdateAsync(dto.filmManagement);
                   
                }
                if (dto.filmManagementDetails != null && dto.filmManagementDetails.Any())
                {
                    foreach (var item in dto.filmManagementDetails)
                    {
                        item.FilmManagementID = dto.filmManagement.ID;
                        if (item.ID <= 0)
                            await _filmManagementDetailRepo.CreateAsync(item);
                        else
                            await _filmManagementDetailRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
