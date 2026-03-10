using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPICriteriaController : ControllerBase
    {
        private readonly KPISessionRepo _kpiSessionRepo;
        private readonly KPICriteriaDetailRepo _kpiCriteriaDetailRepo;
        private readonly KPICriterionRepo _kpiCriterionRepo;
        public KPICriteriaController(KPICriteriaDetailRepo kpiCriteriaDetailRepo, KPICriterionRepo kpiCriterionRepo, KPISessionRepo kpiSessionRepo)
        {
            _kpiCriteriaDetailRepo = kpiCriteriaDetailRepo;
            _kpiCriterionRepo = kpiCriterionRepo;
            _kpiSessionRepo = kpiSessionRepo;
        }

        [HttpGet("get-data")]
        public IActionResult GetData(int quarter, int year, string keywords = "")
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetKPICriteria",
                                new string[] { "@KPICriteriaQuater", "@KPICriteriaYear", "@Keyword" },
                                new object[] { quarter, year, keywords });
                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-detail")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                var data = _kpiCriteriaDetailRepo.GetAll(x => x.KPICriteriaID == id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete")]
        public IActionResult Delete(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var kpiCriteria = _kpiCriterionRepo.GetByID(id);
                    if (kpiCriteria != null)
                    {
                        kpiCriteria.IsDeleted = true;
                        _kpiCriterionRepo.Update(kpiCriteria);
                    }
                }
                return Ok(ApiResponseFactory.Success("", "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-max-stt")]
        public IActionResult GetMaxSTT(int quarter, int year)
        {
            try
            {
                var list = _kpiCriterionRepo.GetAll(x => x.KPICriteriaQuater == quarter && x.KPICriteriaYear == year && x.IsDeleted != true);
                int STT = list.Any()
                    ? ((list.Max(x => x.STT)) ?? 0) + 1
                    : 1;
                return Ok(ApiResponseFactory.Success(STT, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] SaveKPICriteriaDTO dto)
        {
            try
            {
                if(dto.KPICriterions.CriteriaCode == null || dto.KPICriterions.CriteriaCode.Trim() == "")
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã Tiêu Chí!"));
                }
                if (dto.KPICriterions.CriteriaName == null || dto.KPICriterions.CriteriaName.Trim() == "")
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên Tiêu Chí!"));
                }
                if (dto.KPICriterions.KPICriteriaQuater == null || dto.KPICriterions.KPICriteriaQuater <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Quý!"));
                }
                if (dto.KPICriterions.KPICriteriaYear == null || dto.KPICriterions.KPICriteriaYear <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Năm!"));
                }

                var exists = _kpiCriterionRepo.GetAll()
                    .Any(x => x.ID != dto.KPICriterions.ID
                        && x.CriteriaCode == dto.KPICriterions.CriteriaCode
                        && x.KPICriteriaQuater == dto.KPICriterions.KPICriteriaQuater
                        && x.KPICriteriaYear == dto.KPICriterions.KPICriteriaYear
                        && x.IsDeleted != true);

                if (exists)
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã tiêu chí đã tồn tại!"));

                if (dto.KPICriterions.ID > 0)
                {
                    await _kpiCriterionRepo.UpdateAsync(dto.KPICriterions);
                }
                else
                {
                    await _kpiCriterionRepo.CreateAsync(dto.KPICriterions);
                }
                if (dto.DeletedDetailIds != null && dto.DeletedDetailIds.Count > 0)
                {
                    foreach (var item in dto.DeletedDetailIds)
                    {
                        await _kpiCriteriaDetailRepo.DeleteAsync(item);
                    }
                }
                if (dto.KPICriteriaDetails != null && dto.KPICriteriaDetails.Count > 0)
                {
                    foreach(var item in dto.KPICriteriaDetails)
                    {
                        if(item.ID > 0)
                        {
                            await _kpiCriteriaDetailRepo.UpdateAsync(item);
                        }
                        else
                        {
                            item.KPICriteriaID = dto.KPICriterions.ID;
                            await _kpiCriteriaDetailRepo.CreateAsync(item);
                        }
                    }    
                }    
                return Ok(ApiResponseFactory.Success("", ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("copy-criteria")]
        public IActionResult CopyCriteria(int quarter, int year, int quarterCopyTo, int yearCopyTo)
        {
            try
            {
                var session = _kpiSessionRepo.GetAll(x => x.YearEvaluation == year && x.QuarterEvaluation == quarter && x.IsDeleted == false && x.DepartmentID == 2).FirstOrDefault();
                if(session == null) return BadRequest(ApiResponseFactory.Fail(null, ""));
                var listCriteriaOld = _kpiCriterionRepo.GetAll(x => x.KPICriteriaYear == yearCopyTo && x.KPICriteriaQuater == quarterCopyTo && x.IsDeleted == false).ToList();

                //xóa dl cũ
                foreach(var item in listCriteriaOld)
                {
                    List<KPICriteriaDetail> listDetail = _kpiCriteriaDetailRepo.GetAll(x => x.KPICriteriaID == item.ID);
                    _kpiCriteriaDetailRepo.DeleteRange(listDetail);

                    item.IsDeleted = true;
                    _kpiCriterionRepo.Update(item);
                }

                var listData = _kpiCriterionRepo.GetAll(x => x.IsDeleted == false &&
                                                             x.KPICriteriaQuater == session.QuarterEvaluation &&
                                                             x.KPICriteriaYear == session.YearEvaluation);
                foreach (var src in listData)
                {
                    var newCriteria = new KPICriterion
                    {
                        CriteriaCode = src.CriteriaCode,
                        CriteriaName = src.CriteriaName,
                        STT = src.STT,
                        KPICriteriaQuater = quarterCopyTo,
                        KPICriteriaYear = yearCopyTo,
                        IsDeleted = false
                    };

                    _kpiCriterionRepo.Create(newCriteria);

                    var listDetails = _kpiCriteriaDetailRepo.GetAll(x => x.KPICriteriaID == src.ID);
                    foreach (var d in listDetails)
                    {
                        var newDetail = new KPICriteriaDetail
                        {
                            KPICriteriaID = newCriteria.ID,
                            STT = d.STT,
                            Point = d.Point,
                            PointPercent = d.PointPercent,
                            CriteriaContent = d.CriteriaContent
                        };
                        _kpiCriteriaDetailRepo.Create(newDetail);
                    }
                }

                return Ok(ApiResponseFactory.Success("", ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class SaveKPICriteriaDTO
        {
            public KPICriterion KPICriterions { get; set; } = new KPICriterion();
            public List<KPICriteriaDetail>? KPICriteriaDetails { get; set; } = new List<KPICriteriaDetail>();
            public List<int>? DeletedDetailIds { get; set; }
        }
    }
}
