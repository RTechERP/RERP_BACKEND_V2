using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.KPISale;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.KPISale;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RERPAPI.Controllers.KPISale
{
    [Route("api/kpi")]
    [ApiController]
    [Authorize]
    public class KPISaleController : ControllerBase
    {
        private readonly KPISaleRepo _kpiSaleRepo;
        private readonly EmployeeRepo _employeeRepo;

        private static readonly Regex SqlIdentifierRegex = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

        private static readonly Regex AutoTeamCodeRegex = new("^T_[0-9A-Fa-f]+$", RegexOptions.Compiled);

        // Team có TeamCode khớp pattern T_<hex> là team auto-created từ code cũ
        // (CreateOrGetTransientTeamAsync) - không cho phép CRUD từ UI.
        private static bool IsAutoCreatedTeamCode(string? teamCode)
        {
            if (string.IsNullOrEmpty(teamCode)) return false;
            return AutoTeamCodeRegex.IsMatch(teamCode);
        }

        private static readonly HashSet<string> AllowedAggregateTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "SUM",
            "COUNT",
            "COUNT_DISTINCT",
            "SUM_DISTINCT",
            "AVG",
            "MAX",
            "MIN"
        };

        private static readonly HashSet<string> AllowedOperators = new(StringComparer.OrdinalIgnoreCase)
        {
            "=",
            "<>",
            ">",
            ">=",
            "<",
            "<=",
            "LIKE",
            "IN",
            "BETWEEN",
            "IS NULL",
            "IS NOT NULL"
        };

        private static readonly HashSet<string> AllowedLogicOperators = new(StringComparer.OrdinalIgnoreCase)
        {
            "AND",
            "OR"
        };

        private static readonly HashSet<string> AllowedValueTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "STATIC",
            "PARAM",
            "COLUMN"
        };

        private static readonly HashSet<string> AllowedSystemParameters = new(StringComparer.OrdinalIgnoreCase)
        {
            "EmployeeID",
            "DateStart",
            "DateEnd",
            "DepartmentID",
            "PeriodID"
        };

        private static readonly string[] SoftDeleteColumnCandidates =
        {
            "IsDeleted",
            "IsDelete",
            "DeleteFlag"
        };

        public KPISaleController(KPISaleRepo kpiSaleRepo, EmployeeRepo employeeRepo)
        {
            _kpiSaleRepo = kpiSaleRepo;
            _employeeRepo = employeeRepo;
        }

        #region Period

        [HttpGet("periods")]
        public async Task<IActionResult> GetPeriods(string? periodType = null, int? parentPeriodId = null)
        {
            try
            {
                var query = _kpiSaleRepo.KPISalePeriods.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(periodType))
                {
                    var type = periodType.Trim().ToUpperInvariant();
                    query = query.Where(x => x.PeriodType == type);
                }

                if (parentPeriodId.HasValue)
                    query = query.Where(x => x.ParentPeriodID == parentPeriodId.Value);

                var data = await query
                    .OrderBy(x => x.DateStart)
                    .ThenBy(x => x.ID)
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("periods")]
        public async Task<IActionResult> CreatePeriod([FromBody] KPISalePeriod model)
        {
            try
            {
                ValidatePeriod(model);

                model.ID = 0;
                model.PeriodCode = model.PeriodCode.Trim();
                model.PeriodName = model.PeriodName?.Trim();
                model.PeriodType = model.PeriodType.Trim().ToUpperInvariant();
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = null;

                await _kpiSaleRepo.KPISalePeriods.AddAsync(model);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("periods/{id:int}")]
        public async Task<IActionResult> UpdatePeriod(int id, [FromBody] KPISalePeriod request)
        {
            try
            {
                ValidatePeriod(request);

                var model = await _kpiSaleRepo.KPISalePeriods.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy kỳ KPI"));

                model.PeriodCode = request.PeriodCode.Trim();
                model.PeriodName = request.PeriodName?.Trim();
                model.PeriodType = request.PeriodType.Trim().ToUpperInvariant();
                model.DateStart = request.DateStart;
                model.DateEnd = request.DateEnd;
                model.ParentPeriodID = request.ParentPeriodID;
                model.IsClosed = request.IsClosed;
                model.UpdatedDate = DateTime.Now;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("periods/{id:int}")]
        public async Task<IActionResult> DeletePeriod(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISalePeriods.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy kỳ KPI"));

                _kpiSaleRepo.KPISalePeriods.Remove(model);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Period

        #region Template

        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates(bool? isActive = null, string? keyword = null)
        {
            try
            {
                var query = _kpiSaleRepo.KPISaleTemplates.AsNoTracking().AsQueryable();

                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var key = keyword.Trim();
                    query = query.Where(x => x.TemplateCode.Contains(key) || x.TemplateName.Contains(key));
                }

                var data = await query
                    .OrderByDescending(x => x.IsActive)
                    .ThenBy(x => x.TemplateCode)
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("templates")]
        public async Task<IActionResult> CreateTemplate([FromBody] KPISaleTemplate request)
        {
            try
            {
                ValidateTemplate(request);

                var currentUser = GetCurrentUser();
                request.ID = 0;
                request.TemplateCode = request.TemplateCode.Trim();
                request.TemplateName = request.TemplateName.Trim();
                request.Description = request.Description?.Trim();
                request.IsActive = true;
                request.CreatedBy = currentUser.LoginName;
                request.CreatedDate = DateTime.Now;
                request.UpdatedBy = null;
                request.UpdatedDate = null;

                await _kpiSaleRepo.KPISaleTemplates.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("templates/{id:int}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] KPISaleTemplate request)
        {
            try
            {
                ValidateTemplate(request);

                var model = await _kpiSaleRepo.KPISaleTemplates.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy template KPI"));

                var currentUser = GetCurrentUser();
                model.TemplateCode = request.TemplateCode.Trim();
                model.TemplateName = request.TemplateName.Trim();
                model.Description = request.Description?.Trim();
                model.IsActive = request.IsActive;
                model.UpdatedBy = currentUser.LoginName;
                model.UpdatedDate = DateTime.Now;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("templates/{id:int}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleTemplates.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy template KPI"));

                var currentUser = GetCurrentUser();
                model.IsActive = false;
                model.UpdatedBy = currentUser.LoginName;
                model.UpdatedDate = DateTime.Now;
                await _kpiSaleRepo.SaveChangesAsync();

                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("templates/copy")]
        public async Task<IActionResult> CopyTemplate([FromBody] KPISaleCopyTemplateRequest request)
        {
            try
            {
                if (request == null)
                    throw new Exception("Dữ liệu copy không được để trống");

                if (request.SourceTemplateID <= 0)
                    throw new Exception("Vui lòng chọn mẫu nguồn");

                if (request.TargetTemplateID <= 0)
                    throw new Exception("Vui lòng chọn mẫu đích");

                if (request.TargetTemplateID == request.SourceTemplateID)
                    throw new Exception("Mẫu nguồn và mẫu đích không được trùng nhau");

                var result = await CopyTemplateInternalAsync(request);
                var message = $"Đã sao chép {result.CopiedIndexCount} chỉ tiêu";
                if (result.CopiedMappingCount > 0)
                    message += $" và {result.CopiedMappingCount} ánh xạ";
                message += $" sang mẫu \"{result.TargetTemplateName}\"";
                return Ok(ApiResponseFactory.Success(result, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private async Task<KPISaleCopyTemplateResponse> CopyTemplateInternalAsync(KPISaleCopyTemplateRequest request)
        {
            var sourceTemplate = await _kpiSaleRepo.KPISaleTemplates.AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == request.SourceTemplateID)
                    ?? throw new Exception("Không tìm thấy mẫu nguồn");

            var targetTemplate = await _kpiSaleRepo.KPISaleTemplates.AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == request.TargetTemplateID)
                    ?? throw new Exception("Không tìm thấy mẫu đích");

            var currentUser = GetCurrentUser();
            int copiedCount = 0;
            int copiedMappingCount = 0;
            var newIndexIds = new List<int>();

            // idMap: source IndexID -> new IndexID
            var idMap = new Dictionary<int, int>();

            if (request.CopyIndexes)
            {
                var sourceIndexesQuery = _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                    .Where(x => x.TemplateID == request.SourceTemplateID);

                if (!request.IncludeInactiveIndexes)
                    sourceIndexesQuery = sourceIndexesQuery.Where(x => x.IsActive);

                var sourceIndexes = await sourceIndexesQuery
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToListAsync();

                if (sourceIndexes.Count > 0)
                {
                    // Remap ParentID: sắp xếp parent trước rồi insert để idMap luôn có sẵn
                    var sortedIndexes = sourceIndexes
                        .OrderBy(x => x.ParentID.HasValue ? 1 : 0)
                        .ThenBy(x => x.SortOrder)
                        .ThenBy(x => x.ID)
                        .ToList();

                    foreach (var src in sortedIndexes)
                    {
                        int? newParentId = null;
                        if (src.ParentID.HasValue && idMap.TryGetValue(src.ParentID.Value, out var mapped))
                            newParentId = mapped;

                        var clone = new KPISaleIndex
                        {
                            ID = 0,
                            TemplateID = request.TargetTemplateID,
                            ParentID = newParentId,
                            IndexCode = src.IndexCode,
                            IndexName = src.IndexName,
                            IndexType = src.IndexType,
                            UnitType = src.UnitType,
                            WeightPercent = src.WeightPercent,
                            QuarterGoalCalculateType = src.QuarterGoalCalculateType,
                            QuarterResultCalculateType = src.QuarterResultCalculateType,
                            ReportScoreAdjustmentType = src.ReportScoreAdjustmentType,
                            ReportScoreValue = src.ReportScoreValue,
                            SortOrder = src.SortOrder,
                            IsBold = src.IsBold,
                            IsMainIndex = src.IsMainIndex,
                            IsActive = src.IsActive,
                            CreatedBy = currentUser.LoginName,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = null,
                            UpdatedDate = null,
                        };
                        await _kpiSaleRepo.KPISaleIndices.AddAsync(clone);
                        await _kpiSaleRepo.SaveChangesAsync();
                        idMap[src.ID] = clone.ID;
                        newIndexIds.Add(clone.ID);
                        copiedCount++;
                    }
                }
            }

            // Copy mappings và filters nếu được yêu cầu
            if (request.CopyMappings && idMap.Count > 0)
            {
                copiedMappingCount = await CopyMappingsForIndexesAsync(idMap.Keys.ToList(), idMap, currentUser);
            }

            return new KPISaleCopyTemplateResponse
            {
                TargetTemplateID = request.TargetTemplateID,
                TargetTemplateName = targetTemplate.TemplateName,
                CopiedIndexCount = copiedCount,
                CopiedMappingCount = copiedMappingCount,
                NewIndexIDs = newIndexIds,
            };
        }

        /// <summary>
        /// Sao chép tất cả ánh xạ (KPISaleIndexDataMapping) và bộ lọc (FilterGroup, FilterCondition)
        /// từ các chỉ tiêu nguồn sang các chỉ tiêu đích.
        /// </summary>
        private async Task<int> CopyMappingsForIndexesAsync(List<int> sourceIndexIds, Dictionary<int, int> idMap, object currentUser)
        {
            int totalMappingsCopied = 0;

            // Lấy tất cả mappings từ các chỉ tiêu nguồn
            var sourceMappings = await _kpiSaleRepo.KPISaleIndexDataMappings.AsNoTracking()
                .Where(m => sourceIndexIds.Contains(m.KpiIndexID))
                .ToListAsync();

            if (sourceMappings.Count == 0)
                return 0;

            // mappingIdMap: source MappingID -> new MappingID
            var mappingIdMap = new Dictionary<int, int>();

            foreach (var srcMapping in sourceMappings)
            {
                // Tạo mapping mới với KPIIndexID mới
                var newMapping = new KPISaleIndexDataMapping
                {
                    KpiIndexID = idMap[srcMapping.KpiIndexID],
                    DataSourceID = srcMapping.DataSourceID,
                    AggregateType = srcMapping.AggregateType,
                    ValueColumn = srcMapping.ValueColumn,
                    DistinctColumn = srcMapping.DistinctColumn,
                    IsActive = srcMapping.IsActive,
                    //CreatedBy = currentUser?.LoginName ?? "System",
                    //CreatedDate = DateTime.Now,
                    //UpdatedBy = null,
                    //UpdatedDate = null,
                };

                await _kpiSaleRepo.KPISaleIndexDataMappings.AddAsync(newMapping);
                await _kpiSaleRepo.SaveChangesAsync();

                var newMappingId = newMapping.ID;
                mappingIdMap[srcMapping.ID] = newMappingId;
                totalMappingsCopied++;
            }

            // Lấy tất cả filter groups từ các mappings nguồn
            if (mappingIdMap.Count > 0)
            {
                var sourceGroups = await _kpiSaleRepo.KPISaleMappingFilterGroups.AsNoTracking()
                    .Where(g => mappingIdMap.Keys.Contains(g.MappingID))
                    .ToListAsync();

                if (sourceGroups.Count > 0)
                {
                    // groupIdMap: source GroupID -> new GroupID
                    var groupIdMap = new Dictionary<int, int>();

                    // Sắp xếp: root groups trước, rồi children theo thứ tự SortOrder
                    var sortedGroups = sourceGroups
                        .OrderBy(g => g.ParentGroupID.HasValue ? 1 : 0)
                        .ThenBy(g => g.SortOrder)
                        .ThenBy(g => g.ID)
                        .ToList();

                    foreach (var srcGroup in sortedGroups)
                    {
                        int? newParentGroupId = null;
                        if (srcGroup.ParentGroupID.HasValue && groupIdMap.TryGetValue(srcGroup.ParentGroupID.Value, out var mappedGroupId))
                        {
                            newParentGroupId = mappedGroupId;
                        }

                        var newGroup = new KPISaleMappingFilterGroup
                        {
                            MappingID = mappingIdMap[srcGroup.MappingID],
                            ParentGroupID = newParentGroupId,
                            LogicOperator = srcGroup.LogicOperator,
                            SortOrder = srcGroup.SortOrder,
                        };

                        await _kpiSaleRepo.KPISaleMappingFilterGroups.AddAsync(newGroup);
                        await _kpiSaleRepo.SaveChangesAsync();

                        groupIdMap[srcGroup.ID] = newGroup.ID;
                    }

                    // Lấy tất cả filter conditions từ các groups nguồn
                    if (groupIdMap.Count > 0)
                    {
                        var sourceConditions = await _kpiSaleRepo.KPISaleMappingFilterConditions.AsNoTracking()
                            .Where(c => groupIdMap.Keys.Contains(c.FilterGroupID))
                            .ToListAsync();

                        foreach (var srcCond in sourceConditions)
                        {
                            var newCondition = new KPISaleMappingFilterCondition
                            {
                                FilterGroupID = groupIdMap[srcCond.FilterGroupID],
                                ColumnName = srcCond.ColumnName,
                                Operator = srcCond.Operator,
                                ValueType = srcCond.ValueType,
                                Value1 = srcCond.Value1,
                                Value2 = srcCond.Value2,
                                DataType = srcCond.DataType,
                                IsActive = srcCond.IsActive,
                            };

                            await _kpiSaleRepo.KPISaleMappingFilterConditions.AddAsync(newCondition);
                        }

                        await _kpiSaleRepo.SaveChangesAsync();
                    }
                }
            }

            return totalMappingsCopied;
        }

        #endregion Template

        #region Index

        [HttpGet("templates/{templateId:int}/indexes")]
        public async Task<IActionResult> GetIndexes(int templateId, bool? isActive = null)
        {
            try
            {
                var query = _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                    .Where(x => x.TemplateID == templateId);

                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                var indexes = await query
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToListAsync();

                var indexNameMap = indexes.ToDictionary(x => x.ID, x => x.IndexName);
                var data = indexes.Select(x => new
                {
                    x.ID,
                    x.TemplateID,
                    x.ParentID,
                    ParentIndexName = x.ParentID.HasValue && indexNameMap.ContainsKey(x.ParentID.Value)
                        ? indexNameMap[x.ParentID.Value]
                        : null,
                    x.IndexCode,
                    x.IndexName,
                    x.IndexType,
                    x.UnitType,
                    x.WeightPercent,
                    x.QuarterGoalCalculateType,
                    x.QuarterResultCalculateType,
                    x.ReportScoreAdjustmentType,
                    x.SortOrder,
                    x.IsBold,
                    x.IsMainIndex,
                    x.IsActive,
                    x.CreatedBy,
                    x.CreatedDate,
                    x.UpdatedBy,
                    x.UpdatedDate
                }).ToList();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("indexes")]
        public async Task<IActionResult> CreateIndex([FromBody] KPISaleIndex request)
        {
            try
            {
                await ValidateIndexAsync(request);

                var currentUser = GetCurrentUser();
                request.ID = 0;
                request.IndexCode = request.IndexCode.Trim();
                request.IndexName = request.IndexName.Trim();
                request.IndexType = request.IndexType.Trim().ToUpperInvariant();
                request.UnitType = request.UnitType.Trim().ToUpperInvariant();
                request.QuarterGoalCalculateType = NormalizeOptionalCode(request.QuarterGoalCalculateType, "SUM_MONTH");
                request.QuarterResultCalculateType = NormalizeOptionalCode(request.QuarterResultCalculateType, "SUM_MONTH");
                request.ReportScoreAdjustmentType ??= 0;
                request.IsActive = true;
                request.CreatedBy = currentUser.LoginName;
                request.CreatedDate = DateTime.Now;
                request.UpdatedBy = null;
                request.UpdatedDate = null;

                await _kpiSaleRepo.KPISaleIndices.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("indexes/{id:int}")]
        public async Task<IActionResult> UpdateIndex(int id, [FromBody] KPISaleIndex request)
        {
            try
            {
                request.ID = id;
                await ValidateIndexAsync(request);

                var model = await _kpiSaleRepo.KPISaleIndices.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy KPI index"));

                var currentUser = GetCurrentUser();
                model.TemplateID = request.TemplateID;
                model.ParentID = request.ParentID;
                model.IndexCode = request.IndexCode.Trim();
                model.IndexName = request.IndexName.Trim();
                model.IndexType = request.IndexType.Trim().ToUpperInvariant();
                model.UnitType = request.UnitType.Trim().ToUpperInvariant();
                model.WeightPercent = request.WeightPercent;
                model.QuarterGoalCalculateType = NormalizeOptionalCode(request.QuarterGoalCalculateType, "SUM_MONTH");
                model.QuarterResultCalculateType = NormalizeOptionalCode(request.QuarterResultCalculateType, "SUM_MONTH");
                model.ReportScoreAdjustmentType = request.ReportScoreAdjustmentType ?? 0;
                model.SortOrder = request.SortOrder;
                model.IsBold = request.IsBold;
                model.IsMainIndex = request.IsMainIndex;
                model.IsActive = request.IsActive;
                model.UpdatedBy = currentUser.LoginName;
                model.UpdatedDate = DateTime.Now;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("indexes/{id:int}")]
        public async Task<IActionResult> DeleteIndex(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleIndices.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy KPI index"));

                var currentUser = GetCurrentUser();
                model.IsActive = false;
                model.UpdatedBy = currentUser.LoginName;
                model.UpdatedDate = DateTime.Now;
                await _kpiSaleRepo.SaveChangesAsync();

                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Index

        #region Allowed table and column

        [HttpGet("allowed-tables")]
        public async Task<IActionResult> GetAllowedTables(bool? isActive = null)
        {
            try
            {
                var query = _kpiSaleRepo.KPISaleAllowedTables.AsNoTracking().AsQueryable();
                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                var data = await query
                    .OrderBy(x => x.SchemaName)
                    .ThenBy(x => x.TableName)
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("allowed-tables")]
        public async Task<IActionResult> CreateAllowedTable([FromBody] KPISaleAllowedTable request)
        {
            try
            {
                ValidateAllowedTable(request);
                request.ID = 0;
                request.TableName = request.TableName.Trim();
                request.SchemaName = NormalizeOptionalIdentifier(request.SchemaName, "dbo");
                request.DisplayName = request.DisplayName.Trim();
                request.IsActive = true;

                await _kpiSaleRepo.KPISaleAllowedTables.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("allowed-tables/{id:int}")]
        public async Task<IActionResult> UpdateAllowedTable(int id, [FromBody] KPISaleAllowedTable request)
        {
            try
            {
                ValidateAllowedTable(request);

                var model = await _kpiSaleRepo.KPISaleAllowedTables.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy bảng được phép"));

                model.TableName = request.TableName.Trim();
                model.SchemaName = NormalizeOptionalIdentifier(request.SchemaName, "dbo");
                model.DisplayName = request.DisplayName.Trim();
                model.IsActive = request.IsActive;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("allowed-tables/{id:int}")]
        public async Task<IActionResult> DeleteAllowedTable(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleAllowedTables.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy bảng được phép"));

                model.IsActive = false;
                var columns = await _kpiSaleRepo.KPISaleAllowedColumns.Where(x => x.TableID == id).ToListAsync();
                foreach (var column in columns)
                    column.IsActive = false;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("allowed-tables/{tableId:int}/columns")]
        public async Task<IActionResult> GetAllowedColumns(int tableId, bool? isActive = null)
        {
            try
            {
                var query = _kpiSaleRepo.KPISaleAllowedColumns.AsNoTracking()
                    .Where(x => x.TableID == tableId);

                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                var data = await query
                    .OrderByDescending(x => x.IsDateColumn)
                    .ThenByDescending(x => x.IsEmployeeColumn)
                    .ThenBy(x => x.ColumnName)
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("allowed-tables/{tableId:int}/columns/{columnName}/unique-values")]
        public async Task<IActionResult> GetAllowedColumnUniqueValues(int tableId, string columnName)
        {
            try
            {
                var table = await _kpiSaleRepo.KPISaleAllowedTables.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == tableId && x.IsActive);
                if (table == null || table.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy bảng được phép"));

                ValidateIdentifier(table.SchemaName, nameof(table.SchemaName));
                ValidateIdentifier(table.TableName, nameof(table.TableName));
                ValidateIdentifier(columnName, nameof(columnName));

                var column = await _kpiSaleRepo.KPISaleAllowedColumns.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TableID == tableId && x.ColumnName == columnName && x.IsActive);
                if (column == null)
                    return NotFound(ApiResponseFactory.Fail(null, $"Cột '{columnName}' không được phép"));

                if (!string.IsNullOrWhiteSpace(column.LookupTable))
                {
                    string lookupSchema = "dbo";
                    string lookupTable = column.LookupTable.Trim();
                    if (lookupTable.Contains('.'))
                    {
                        var parts = lookupTable.Split('.');
                        lookupSchema = parts[0].Trim();
                        lookupTable = parts[1].Trim();
                    }
                    var valueCol = string.IsNullOrWhiteSpace(column.LookupValueColumn) ? "ID" : column.LookupValueColumn.Trim();
                    var displayCol = string.IsNullOrWhiteSpace(column.LookupDisplayColumn) ? valueCol : column.LookupDisplayColumn.Trim();

                    ValidateIdentifier(lookupSchema, nameof(lookupSchema));
                    ValidateIdentifier(lookupTable, nameof(lookupTable));
                    ValidateIdentifier(valueCol, nameof(valueCol));
                    ValidateIdentifier(displayCol, nameof(displayCol));

                    // LookupPreFilter áp vào bảng lookup
                    var lookupData = await _kpiSaleRepo.GetUniqueValuesAsync(lookupSchema, lookupTable, valueCol, displayCol,
                        column.LookupPreFilterColumn, column.LookupPreFilterOperator, column.LookupPreFilterValue, column.LookupPreFilterValue2);
                    return Ok(ApiResponseFactory.Success(lookupData, ""));
                }
                else if (!string.IsNullOrWhiteSpace(column.ManualValueMapJson))
                {
                    var data = ParseManualLookupValues(column.ManualValueMapJson);
                    return Ok(ApiResponseFactory.Success(data, ""));
                }
                else
                {
                    // Không có lookup -> pre-filter áp vào bảng chính
                    var data = await _kpiSaleRepo.GetUniqueValuesAsync(table.SchemaName ?? "dbo", table.TableName, columnName, columnName);
                    return Ok(ApiResponseFactory.Success(data, ""));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("allowed-columns")]
        public async Task<IActionResult> CreateAllowedColumn([FromBody] KPISaleAllowedColumn request)
        {
            try
            {
                await ValidateAllowedColumnAsync(request);

                request.ID = 0;
                request.ColumnName = request.ColumnName.Trim();
                request.DisplayName = request.DisplayName.Trim();
                request.DataType = request.DataType.Trim().ToUpperInvariant();
                request.ManualValueMapJson = NormalizeManualValueMapJson(request.ManualValueMapJson);
                request.IsActive = true;

                await _kpiSaleRepo.KPISaleAllowedColumns.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("allowed-columns/{id:int}")]
        public async Task<IActionResult> UpdateAllowedColumn(int id, [FromBody] KPISaleAllowedColumn request)
        {
            try
            {
                request.ID = id;
                await ValidateAllowedColumnAsync(request);

                var model = await _kpiSaleRepo.KPISaleAllowedColumns.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy cột được phép"));

                model.TableID = request.TableID;
                model.ColumnName = request.ColumnName.Trim();
                model.DisplayName = request.DisplayName.Trim();
                model.DataType = request.DataType.Trim().ToUpperInvariant();
                model.CanFilter = request.CanFilter;
                model.CanAggregate = request.CanAggregate;
                model.CanDistinct = request.CanDistinct;
                model.IsEmployeeColumn = request.IsEmployeeColumn;
                model.IsDateColumn = request.IsDateColumn;
                model.IsValueColumn = request.IsValueColumn;
                model.LookupValueColumn = request.LookupValueColumn;
                model.LookupDisplayColumn = request.LookupDisplayColumn;
                model.LookupTable = request.LookupTable;
                model.ManualValueMapJson = NormalizeManualValueMapJson(request.ManualValueMapJson);
                model.PreFilterColumn = request.PreFilterColumn;
                model.PreFilterOperator = request.PreFilterOperator;
                model.PreFilterValueType = request.PreFilterValueType;
                model.PreFilterValue = request.PreFilterValue;
                model.PreFilterValue2 = request.PreFilterValue2;
                model.LookupPreFilterColumn = request.LookupPreFilterColumn;
                model.LookupPreFilterOperator = request.LookupPreFilterOperator;
                model.LookupPreFilterValueType = request.LookupPreFilterValueType;
                model.LookupPreFilterValue = request.LookupPreFilterValue;
                model.LookupPreFilterValue2 = request.LookupPreFilterValue2;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("allowed-columns/{id:int}")]
        public async Task<IActionResult> DeleteAllowedColumn(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleAllowedColumns.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy cột được phép"));

                model.IsActive = false;
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Allowed table and column

        #region Data source

        [HttpGet("data-sources")]
        public async Task<IActionResult> GetDataSources(bool? isActive = null)
        {
            try
            {
                var query =
                    from source in _kpiSaleRepo.KPISaleDataSources.AsNoTracking()
                    join table in _kpiSaleRepo.KPISaleAllowedTables.AsNoTracking()
                        on source.AllowedTableID equals table.ID
                    select new
                    {
                        source.ID,
                        source.SourceCode,
                        source.SourceName,
                        source.AllowedTableID,
                        table.SchemaName,
                        table.TableName,
                        TableDisplayName = table.DisplayName,
                        source.DateColumn,
                        source.EmployeeColumn,
                        source.ValueColumn,
                        source.UseEmployeeID,
                        source.IsActive
                    };

                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                var data = await query
                    .OrderBy(x => x.SourceCode)
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("data-sources")]
        public async Task<IActionResult> CreateDataSource([FromBody] KPISaleDataSource request)
        {
            try
            {
                await ValidateDataSourceAsync(request);

                request.ID = 0;
                request.SourceCode = request.SourceCode.Trim().ToUpperInvariant();
                request.SourceName = request.SourceName.Trim();
                request.DateColumn = request.DateColumn.Trim();
                request.EmployeeColumn = NormalizeNullableIdentifier(request.EmployeeColumn);
                request.ValueColumn = NormalizeNullableIdentifier(request.ValueColumn);
                request.UseEmployeeID = request.UseEmployeeID;
                request.IsActive = true;

                await _kpiSaleRepo.KPISaleDataSources.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("data-sources/{id:int}")]
        public async Task<IActionResult> UpdateDataSource(int id, [FromBody] KPISaleDataSource request)
        {
            try
            {
                request.ID = id;
                await ValidateDataSourceAsync(request);

                var model = await _kpiSaleRepo.KPISaleDataSources.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy data source"));

                model.SourceCode = request.SourceCode.Trim().ToUpperInvariant();
                model.SourceName = request.SourceName.Trim();
                model.AllowedTableID = request.AllowedTableID;
                model.DateColumn = request.DateColumn.Trim();
                model.EmployeeColumn = NormalizeNullableIdentifier(request.EmployeeColumn);
                model.ValueColumn = NormalizeNullableIdentifier(request.ValueColumn);
                model.UseEmployeeID = request.UseEmployeeID;
                model.IsActive = request.IsActive;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("data-sources/{id:int}")]
        public async Task<IActionResult> DeleteDataSource(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleDataSources.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy data source"));

                model.IsActive = false;
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Data source

        #region Mapping and filter

        [HttpGet("indexes/{kpiIndexId:int}/mappings")]
        public async Task<IActionResult> GetMappings(int kpiIndexId, bool? isActive = null)
        {
            try
            {
                var query =
                    from mapping in _kpiSaleRepo.KPISaleIndexDataMappings.AsNoTracking()
                    join source in _kpiSaleRepo.KPISaleDataSources.AsNoTracking()
                        on mapping.DataSourceID equals source.ID
                    where mapping.KpiIndexID == kpiIndexId
                    select new
                    {
                        mapping.ID,
                        mapping.KpiIndexID,
                        mapping.DataSourceID,
                        source.SourceCode,
                        source.SourceName,
                        mapping.AggregateType,
                        mapping.ValueColumn,
                        mapping.DistinctColumn,
                        mapping.IsActive,
                        mapping.CreatedBy,
                        mapping.CreatedDate,
                        mapping.UpdatedBy,
                        mapping.UpdatedDate
                    };

                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                var data = await query
                    .OrderBy(x => x.ID)
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("mappings")]
        public async Task<IActionResult> CreateMapping([FromBody] KPISaleIndexDataMapping request)
        {
            try
            {
                await ValidateMappingAsync(request);

                var currentUser = GetCurrentUser();
                request.ID = 0;
                request.AggregateType = request.AggregateType.Trim().ToUpperInvariant();
                request.ValueColumn = NormalizeNullableIdentifier(request.ValueColumn);
                request.DistinctColumn = NormalizeNullableIdentifier(request.DistinctColumn);
                request.IsActive = true;
                request.CreatedBy = currentUser.LoginName;
                request.CreatedDate = DateTime.Now;
                request.UpdatedBy = null;
                request.UpdatedDate = null;

                await _kpiSaleRepo.KPISaleIndexDataMappings.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("mappings/{id:int}")]
        public async Task<IActionResult> UpdateMapping(int id, [FromBody] KPISaleIndexDataMapping request)
        {
            try
            {
                request.ID = id;
                await ValidateMappingAsync(request);

                var model = await _kpiSaleRepo.KPISaleIndexDataMappings.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy KPI mapping"));

                var currentUser = GetCurrentUser();
                model.KpiIndexID = request.KpiIndexID;
                model.DataSourceID = request.DataSourceID;
                model.AggregateType = request.AggregateType.Trim().ToUpperInvariant();
                model.ValueColumn = NormalizeNullableIdentifier(request.ValueColumn);
                model.DistinctColumn = NormalizeNullableIdentifier(request.DistinctColumn);
                model.IsActive = request.IsActive;
                model.UpdatedBy = currentUser.LoginName;
                model.UpdatedDate = DateTime.Now;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("mappings/{id:int}")]
        public async Task<IActionResult> DeleteMapping(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleIndexDataMappings.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy KPI mapping"));

                var currentUser = GetCurrentUser();
                model.IsActive = false;
                model.UpdatedBy = currentUser.LoginName;
                model.UpdatedDate = DateTime.Now;
                await _kpiSaleRepo.SaveChangesAsync();

                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("mappings/{mappingId:int}/columns/{columnName}/unique-values")]
        public async Task<IActionResult> GetColumnUniqueValues(int mappingId, string columnName)
        {
            try
            {
                var mapping = await _kpiSaleRepo.KPISaleIndexDataMappings.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == mappingId);
                if (mapping == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy KPI mapping"));

                var source = await _kpiSaleRepo.KPISaleDataSources.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == mapping.DataSourceID);
                if (source == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy data source"));

                var table = await _kpiSaleRepo.KPISaleAllowedTables.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == source.AllowedTableID);
                if (table == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy bảng được phép"));

                var column = await _kpiSaleRepo.KPISaleAllowedColumns.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TableID == table.ID && x.ColumnName == columnName && x.IsActive);
                if (column == null)
                    return NotFound(ApiResponseFactory.Fail(null, $"Cột không được phép hoặc không hợp lệ: {columnName}"));

                ValidateIdentifier(table.TableName, nameof(table.TableName));
                ValidateIdentifier(table.SchemaName ?? "dbo", nameof(table.SchemaName));
                ValidateIdentifier(column.ColumnName, nameof(column.ColumnName));

                if (!string.IsNullOrWhiteSpace(column.LookupTable))
                {
                    string lookupSchema = "dbo";
                    string lookupTable = column.LookupTable.Trim();
                    if (lookupTable.Contains('.'))
                    {
                        var parts = lookupTable.Split('.');
                        lookupSchema = parts[0].Trim();
                        lookupTable = parts[1].Trim();
                    }
                    var valueColumn = string.IsNullOrWhiteSpace(column.LookupValueColumn) ? "ID" : column.LookupValueColumn.Trim();
                    var displayColumn = string.IsNullOrWhiteSpace(column.LookupDisplayColumn) ? valueColumn : column.LookupDisplayColumn.Trim();

                    ValidateIdentifier(lookupSchema, nameof(lookupSchema));
                    ValidateIdentifier(lookupTable, nameof(lookupTable));
                    ValidateIdentifier(valueColumn, nameof(valueColumn));
                    ValidateIdentifier(displayColumn, nameof(displayColumn));

                    // LookupPreFilter áp vào bảng lookup (vì cột này là FK)
                    var lookupData = await _kpiSaleRepo.GetUniqueValuesAsync(lookupSchema, lookupTable, valueColumn, displayColumn,
                        column.LookupPreFilterColumn, column.LookupPreFilterOperator, column.LookupPreFilterValue, column.LookupPreFilterValue2);
                    return Ok(ApiResponseFactory.Success(lookupData, ""));
                }
                else if (!string.IsNullOrWhiteSpace(column.ManualValueMapJson))
                {
                    var data = ParseManualLookupValues(column.ManualValueMapJson);
                    return Ok(ApiResponseFactory.Success(data, ""));
                }
                else
                {
                    // Không có lookup -> pre-filter áp vào bảng chính (ít dùng)
                    var data = await _kpiSaleRepo.GetUniqueValuesAsync(table.SchemaName ?? "dbo", table.TableName, column.ColumnName, column.ColumnName);
                    return Ok(ApiResponseFactory.Success(data, ""));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("mappings/{mappingId:int}/filters")]
        public async Task<IActionResult> GetFilterTree(int mappingId)
        {
            try
            {
                var mapping =
                    await (from m in _kpiSaleRepo.KPISaleIndexDataMappings.AsNoTracking()
                           join source in _kpiSaleRepo.KPISaleDataSources.AsNoTracking()
                               on m.DataSourceID equals source.ID
                           where m.ID == mappingId
                           select new
                           {
                               m.ID,
                               m.KpiIndexID,
                               m.DataSourceID,
                               source.SourceCode,
                               source.SourceName,
                               source.AllowedTableID,
                               m.AggregateType,
                               m.ValueColumn,
                               m.DistinctColumn,
                               m.IsActive
                           }).FirstOrDefaultAsync();

                if (mapping == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy KPI mapping"));

                var groups = await _kpiSaleRepo.KPISaleMappingFilterGroups.AsNoTracking()
                    .Where(x => x.MappingID == mappingId)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToListAsync();

                var conditions = await _kpiSaleRepo.KPISaleMappingFilterConditions.AsNoTracking()
                    .Where(x => groups.Select(g => g.ID).Contains(x.FilterGroupID) && x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToListAsync();

                var columns = await _kpiSaleRepo.KPISaleAllowedColumns.AsNoTracking()
                    .Where(x => x.TableID == mapping.AllowedTableID && x.IsActive)
                    .ToListAsync();
                var colMap = columns.ToDictionary(c => c.ColumnName, c => c);

                var enrichedConditions = new List<KPISaleFilterConditionDto>();
                foreach (var cond in conditions)
                {
                    string? value1Display = null;
                    string? value2Display = null;

                    if (cond.ValueType == "STATIC" && colMap.TryGetValue(cond.ColumnName, out var col))
                    {
                        if (!string.IsNullOrWhiteSpace(col.LookupTable))
                        {
                            string lookupSchema = "dbo";
                            string lookupTable = col.LookupTable.Trim();
                            if (lookupTable.Contains('.'))
                            {
                                var parts = lookupTable.Split('.');
                                lookupSchema = parts[0].Trim();
                                lookupTable = parts[1].Trim();
                            }
                            var valueColumn = string.IsNullOrWhiteSpace(col.LookupValueColumn) ? "ID" : col.LookupValueColumn.Trim();
                            var displayCol = string.IsNullOrWhiteSpace(col.LookupDisplayColumn) ? valueColumn : col.LookupDisplayColumn.Trim();

                            try
                            {
                                ValidateIdentifier(lookupSchema, nameof(lookupSchema));
                                ValidateIdentifier(lookupTable, nameof(lookupTable));
                                ValidateIdentifier(valueColumn, nameof(valueColumn));
                                ValidateIdentifier(displayCol, nameof(displayCol));

                                value1Display = await BuildLookupDisplayTextAsync(lookupSchema, lookupTable, valueColumn, displayCol, cond.Value1);
                                value2Display = await BuildLookupDisplayTextAsync(lookupSchema, lookupTable, valueColumn, displayCol, cond.Value2);
                            }
                            catch (Exception)
                            {
                                // Fail-safe
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(col.ManualValueMapJson))
                        {
                            value1Display = BuildManualDisplayText(col.ManualValueMapJson, cond.Value1);
                            value2Display = BuildManualDisplayText(col.ManualValueMapJson, cond.Value2);
                        }
                    }

                    enrichedConditions.Add(new KPISaleFilterConditionDto
                    {
                        ID = cond.ID,
                        FilterGroupID = cond.FilterGroupID,
                        ColumnName = cond.ColumnName,
                        Operator = cond.Operator,
                        ValueType = cond.ValueType,
                        Value1 = cond.Value1,
                        Value2 = cond.Value2,
                        DataType = cond.DataType,
                        IsActive = cond.IsActive,
                        SortOrder = cond.SortOrder,
                        Value1Display = value1Display,
                        Value2Display = value2Display
                    });
                }

                var nodes = BuildFilterTree(groups, enrichedConditions);

                return Ok(ApiResponseFactory.Success(new KPISaleFilterTreeResult
                {
                    Mapping = mapping,
                    Groups = nodes
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("filter-groups")]
        public async Task<IActionResult> CreateFilterGroup([FromBody] KPISaleMappingFilterGroup request)
        {
            try
            {
                await ValidateFilterGroupAsync(request);

                request.ID = 0;
                request.LogicOperator = NormalizeLogicOperator(request.LogicOperator);

                await _kpiSaleRepo.KPISaleMappingFilterGroups.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("filter-groups/{id:int}")]
        public async Task<IActionResult> UpdateFilterGroup(int id, [FromBody] KPISaleMappingFilterGroup request)
        {
            try
            {
                request.ID = id;
                await ValidateFilterGroupAsync(request);

                var model = await _kpiSaleRepo.KPISaleMappingFilterGroups.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy filter group"));

                model.MappingID = request.MappingID;
                model.ParentGroupID = request.ParentGroupID;
                model.LogicOperator = NormalizeLogicOperator(request.LogicOperator);
                model.SortOrder = request.SortOrder;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("filter-groups/{id:int}")]
        public async Task<IActionResult> DeleteFilterGroup(int id)
        {
            try
            {
                var group = await _kpiSaleRepo.KPISaleMappingFilterGroups.FindAsync(id);
                if (group == null || group.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy filter group"));

                var groups = await _kpiSaleRepo.KPISaleMappingFilterGroups
                    .Where(x => x.MappingID == group.MappingID)
                    .ToListAsync();

                var groupIds = GetDescendantGroupIds(id, groups);
                var conditions = await _kpiSaleRepo.KPISaleMappingFilterConditions
                    .Where(x => groupIds.Contains(x.FilterGroupID))
                    .ToListAsync();

                _kpiSaleRepo.KPISaleMappingFilterConditions.RemoveRange(conditions);
                _kpiSaleRepo.KPISaleMappingFilterGroups.RemoveRange(groups.Where(x => groupIds.Contains(x.ID)));
                await _kpiSaleRepo.SaveChangesAsync();

                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("filter-conditions")]
        public async Task<IActionResult> CreateFilterCondition([FromBody] KPISaleMappingFilterCondition request)
        {
            try
            {
                await ValidateFilterConditionAsync(request);

                request.ID = 0;
                NormalizeFilterCondition(request);
                request.IsActive = true;

                await _kpiSaleRepo.KPISaleMappingFilterConditions.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("filter-conditions/{id:int}")]
        public async Task<IActionResult> UpdateFilterCondition(int id, [FromBody] KPISaleMappingFilterCondition request)
        {
            try
            {
                request.ID = id;
                await ValidateFilterConditionAsync(request);

                var model = await _kpiSaleRepo.KPISaleMappingFilterConditions.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy filter condition"));

                NormalizeFilterCondition(request);
                model.FilterGroupID = request.FilterGroupID;
                model.ColumnName = request.ColumnName;
                model.Operator = request.Operator;
                model.ValueType = request.ValueType;
                model.Value1 = request.Value1;
                model.Value2 = request.Value2;
                model.DataType = request.DataType;
                model.SortOrder = request.SortOrder;
                model.IsActive = request.IsActive;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("filter-conditions/{id:int}")]
        public async Task<IActionResult> DeleteFilterCondition(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleMappingFilterConditions.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy filter condition"));

                model.IsActive = false;
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Mapping and filter

        #region Formula and scoring

        [HttpGet("indexes/{kpiIndexId:int}/formula-items")]
        public async Task<IActionResult> GetFormulaItems(int kpiIndexId)
        {
            try
            {
                var data = await _kpiSaleRepo.KPISaleIndexFormulaItems.AsNoTracking()
                    .Where(x => x.ParentKpiIndexID == kpiIndexId)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("formula-items")]
        public async Task<IActionResult> CreateFormulaItem([FromBody] KPISaleIndexFormulaItem request)
        {
            try
            {
                await ValidateFormulaItemAsync(request);

                request.ID = 0;
                request.Operator = NormalizeFormulaOperator(request.Operator);

                await _kpiSaleRepo.KPISaleIndexFormulaItems.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("formula-items/{id:int}")]
        public async Task<IActionResult> UpdateFormulaItem(int id, [FromBody] KPISaleIndexFormulaItem request)
        {
            try
            {
                request.ID = id;
                await ValidateFormulaItemAsync(request);

                var model = await _kpiSaleRepo.KPISaleIndexFormulaItems.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy formula item"));

                model.ParentKpiIndexID = request.ParentKpiIndexID;
                model.ChildKpiIndexID = request.ChildKpiIndexID;
                model.Operator = NormalizeFormulaOperator(request.Operator);
                model.SortOrder = request.SortOrder;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("formula-items/{id:int}")]
        public async Task<IActionResult> DeleteFormulaItem(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleIndexFormulaItems.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy formula item"));

                _kpiSaleRepo.KPISaleIndexFormulaItems.Remove(model);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("indexes/{kpiIndexId:int}/scoring-rules")]
        public async Task<IActionResult> GetScoringRules(int kpiIndexId)
        {
            try
            {
                var data = await _kpiSaleRepo.KPISaleScoringRules.AsNoTracking()
                    .Where(x => x.KpiIndexID == kpiIndexId)
                    .OrderBy(x => x.ID)
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("scoring-rules")]
        public async Task<IActionResult> CreateScoringRule([FromBody] KPISaleScoringRule request)
        {
            try
            {
                await ValidateScoringRuleAsync(request);

                request.ID = 0;
                request.ScoreType = NormalizeOptionalCode(request.ScoreType, "NORMAL_PERCENT");

                await _kpiSaleRepo.KPISaleScoringRules.AddAsync(request);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(request, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("scoring-rules/{id:int}")]
        public async Task<IActionResult> UpdateScoringRule(int id, [FromBody] KPISaleScoringRule request)
        {
            try
            {
                request.ID = id;
                await ValidateScoringRuleAsync(request);

                var model = await _kpiSaleRepo.KPISaleScoringRules.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy scoring rule"));

                model.KpiIndexID = request.KpiIndexID;
                model.ScoreType = NormalizeOptionalCode(request.ScoreType, "NORMAL_PERCENT");
                model.MaxAchievedPercent = request.MaxAchievedPercent;
                model.FormulaJson = request.FormulaJson;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("scoring-rules/{id:int}")]
        public async Task<IActionResult> DeleteScoringRule(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleScoringRules.FindAsync(id);
                if (model == null || model.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy scoring rule"));

                _kpiSaleRepo.KPISaleScoringRules.Remove(model);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Formula and scoring

        #region Target and result

        [HttpGet("targets")]
        public async Task<IActionResult> GetTargets(int? employeeId = null, int? periodId = null, int? templateId = null)
        {
            try
            {
                // Lấy danh sách period để xác định child periods nếu chọn QUÝ/NĂM
                var allPeriods = await _kpiSaleRepo.KPISalePeriods.AsNoTracking().ToListAsync();

                // Xác định danh sách periodId cần lấy (bao gồm child periods)
                HashSet<int> periodIdsToQuery = new();
                if (periodId.HasValue)
                {
                    var selectedPeriod = allPeriods.FirstOrDefault(p => p.ID == periodId.Value);
                    if (selectedPeriod != null)
                    {
                        if (selectedPeriod.PeriodType == "MONTH")
                        {
                            // Chỉ lấy tháng đó
                            periodIdsToQuery.Add(periodId.Value);
                        }
                        else if (selectedPeriod.PeriodType == "QUARTER")
                        {
                            // Lấy cả bản ghi QUARTER (để frontend cascade sum lên QUARTER target)
                            periodIdsToQuery.Add(periodId.Value);
                            // Lấy tất cả tháng con
                            var childMonths = allPeriods.Where(p => p.PeriodType == "MONTH" && p.ParentPeriodID == periodId.Value);
                            foreach (var m in childMonths)
                                periodIdsToQuery.Add(m.ID);
                        }
                        else if (selectedPeriod.PeriodType == "YEAR")
                        {
                            // Lấy tất cả quý con và tháng con của quý
                            var childQuarters = allPeriods.Where(p => p.PeriodType == "QUARTER" && p.ParentPeriodID == periodId.Value);
                            var quarterIds = childQuarters.Select(q => q.ID).ToHashSet();
                            var childMonths = allPeriods.Where(p => p.PeriodType == "MONTH" && p.ParentPeriodID != null && quarterIds.Contains(p.ParentPeriodID.Value));
                            foreach (var m in childMonths)
                                periodIdsToQuery.Add(m.ID);
                        }
                    }
                }

                // Tạo dictionary để lookup ParentPeriodCode nhanh (tránh lỗi CS8072 khi dùng trong LINQ query)
                var periodLookup = allPeriods.ToDictionary(p => p.ID, p => p.PeriodCode);

                // Lấy dữ liệu thô từ DB
                var rawQuery =
                    from target in _kpiSaleRepo.KPISaleTargets.AsNoTracking()
                    join index in _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                        on target.KpiIndexID equals index.ID
                    join period in _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                        on target.PeriodID equals period.ID
                    select new
                    {
                        target.ID,
                        target.EmployeeID,
                        target.PeriodID,
                        period.PeriodCode,
                        period.PeriodName,
                        period.PeriodType,
                        period.ParentPeriodID,
                        target.KpiIndexID,
                        index.TemplateID,
                        index.IndexCode,
                        index.IndexName,
                        index.UnitType,
                        index.SortOrder,
                        target.GoalValue,
                        target.WeightPercent,
                        target.ProposedGoalValue,
                        target.ApprovalStatus,
                        target.ApprovedBy,
                        target.ApprovedDate,
                        target.RejectedBy,
                        target.RejectedDate,
                        target.CreatedBy,
                        target.CreatedDate,
                        target.UpdatedBy,
                        target.UpdatedDate
                    };

                if (employeeId.HasValue)
                    rawQuery = rawQuery.Where(x => x.EmployeeID == employeeId.Value);

                // Lọc theo danh sách periodIds (đã bao gồm child periods)
                if (periodId.HasValue && periodIdsToQuery.Count > 0)
                    rawQuery = rawQuery.Where(x => periodIdsToQuery.Contains(x.PeriodID));
                else if (periodId.HasValue)
                    rawQuery = rawQuery.Where(x => x.PeriodID == periodId.Value);

                if (templateId.HasValue)
                    rawQuery = rawQuery.Where(x => x.TemplateID == templateId.Value);

                var rawData = await rawQuery.ToListAsync();

                // Project sang DTO in-memory (sau khi đã fetch từ DB, tránh CS8072)
                var data = rawData
                    .Select(x => new
                    {
                        x.ID,
                        x.EmployeeID,
                        x.PeriodID,
                        x.PeriodCode,
                        x.PeriodName,
                        x.PeriodType,
                        x.ParentPeriodID,
                        // ParentPeriodCode để frontend group theo quý
                        ParentPeriodCode = x.ParentPeriodID.HasValue && periodLookup.TryGetValue(x.ParentPeriodID.Value, out var parentCode)
                            ? parentCode
                            : null,
                        x.KpiIndexID,
                        x.TemplateID,
                        x.IndexCode,
                        x.IndexName,
                        x.UnitType,
                        x.SortOrder,
                        x.GoalValue,
                        x.WeightPercent,
                        x.ProposedGoalValue,
                        x.ApprovalStatus,
                        x.ApprovedBy,
                        x.ApprovedDate,
                        x.RejectedBy,
                        x.RejectedDate,
                        x.CreatedBy,
                        x.CreatedDate,
                        x.UpdatedBy,
                        x.UpdatedDate
                    })
                    .OrderBy(x => x.ParentPeriodCode)
                    .ThenBy(x => x.PeriodID)
                    .ThenBy(x => x.SortOrder)
                    .ToList();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("targets")]
        public async Task<IActionResult> SaveTarget([FromBody] KPISaleTargetUpsertRequest request)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var model = await UpsertTargetAsync(request, currentUser.LoginName);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("targets/propose")]
        public async Task<IActionResult> ProposeTarget([FromBody] KPISaleTargetProposeRequest request)
        {
            try
            {
                if (request == null || request.ID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Thiếu ID mục tiêu"));

                var currentUser = GetCurrentUser();
                var target = await _kpiSaleRepo.KPISaleTargets.FirstOrDefaultAsync(x => x.ID == request.ID);
                if (target == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy mục tiêu"));

                if (target.ApprovalStatus == "Approved")
                    return BadRequest(ApiResponseFactory.Fail(null, "Mục tiêu đã duyệt, không thể đề xuất lại"));

                target.ProposedGoalValue = request.ProposedGoalValue;
                target.ApprovalStatus = "Pending";
                target.UpdatedBy = currentUser.LoginName;
                target.UpdatedDate = DateTime.Now;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(target, "Gửi đề xuất thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("targets/{id:int}/approve")]
        public async Task<IActionResult> ApproveTarget(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var target = await _kpiSaleRepo.KPISaleTargets.FirstOrDefaultAsync(x => x.ID == id);
                if (target == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy mục tiêu"));

                if (target.ApprovalStatus != "Pending")
                    return BadRequest(ApiResponseFactory.Fail(null, "Chỉ duyệt được mục tiêu đang ở trạng thái chờ duyệt"));

                if (target.ProposedGoalValue.HasValue)
                    target.GoalValue = target.ProposedGoalValue.Value;

                target.ApprovalStatus = "Approved";
                target.ApprovedBy = currentUser.LoginName;
                target.ApprovedDate = DateTime.Now;
                target.UpdatedBy = currentUser.LoginName;
                target.UpdatedDate = DateTime.Now;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(target, "Duyệt mục tiêu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("targets/{id:int}/weight")]
        public async Task<IActionResult> UpdateTargetWeight(int id, [FromBody] KPISaleTargetUpdateWeightRequest request)
        {
            try
            {
                var currentUser = GetCurrentUser();

                // Permission: only N1 admin or Leader
                //var isN1 = HasPermission("N1");
                var permissions = currentUser.Permissions?.Split(',').Select(p => p.Trim()).ToList() ?? new List<string>();

                var isLeader = currentUser?.EmployeeID != null &&
                    await _kpiSaleRepo.KPISaleTeams.AnyAsync(t => t.IsActive && t.LeaderEmployeeID == currentUser.EmployeeID);
                if (!permissions.Contains("N1") && !isLeader)
                    return BadRequest(ApiResponseFactory.Fail(null, "Chỉ admin (N1) hoặc Leader mới được chỉnh sửa trọng số"));

                if (request.WeightPercent.HasValue && (request.WeightPercent.Value < 0 || request.WeightPercent.Value > 100))
                    return BadRequest(ApiResponseFactory.Fail(null, "Trọng số phải nằm trong khoảng 0 - 100%"));

                var target = await _kpiSaleRepo.KPISaleTargets.FirstOrDefaultAsync(x => x.ID == id);
                if (target == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy mục tiêu"));

                target.WeightPercent = request.WeightPercent;
                target.UpdatedBy = currentUser.LoginName;
                target.UpdatedDate = DateTime.Now;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(target, "Cập nhật trọng số thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("targets/{id:int}/reject")]
        public async Task<IActionResult> RejectTarget(int id, [FromBody] KPISaleTargetRejectRequest? body = null)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var target = await _kpiSaleRepo.KPISaleTargets.FirstOrDefaultAsync(x => x.ID == id);
                if (target == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy mục tiêu"));

                if (target.ApprovalStatus == "Approved")
                    return BadRequest(ApiResponseFactory.Fail(null, "Mục tiêu đã duyệt, không thể hủy"));

                target.ApprovalStatus = "Rejected";
                target.RejectedBy = currentUser.LoginName;
                target.RejectedDate = DateTime.Now;
                target.ProposedGoalValue = null;
                target.UpdatedBy = currentUser.LoginName;
                target.UpdatedDate = DateTime.Now;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(target, "Đã hủy đề xuất"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("targets")]
        public async Task<IActionResult> SaveTargets([FromBody] List<KPISaleTargetUpsertRequest> requests)
        {
            try
            {
                if (requests == null || requests.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách target không được để trống"));

                var currentUser = GetCurrentUser();
                var data = new List<KPISaleTarget>();

                foreach (var request in requests)
                    data.Add(await UpsertTargetAsync(request, currentUser.LoginName));

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(data, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("targets/auto-create")]
        public async Task<IActionResult> AutoCreateMissingTargets([FromBody] AutoCreateTargetRequest request)
        {
            try
            {
                if (request.EmployeeID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "EmployeeID không hợp lệ"));
                if (request.TemplateID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "TemplateID không hợp lệ"));
                if (request.PeriodID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "PeriodID không hợp lệ"));

                var currentUser = GetCurrentUser();

                // Lấy template + indexes
                var template = await _kpiSaleRepo.KPISaleTemplates
                    .FirstOrDefaultAsync(t => t.ID == request.TemplateID);
                if (template == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy template"));

                var indexes = await _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                    .Where(x => x.TemplateID == request.TemplateID && x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();

                // Lấy tất cả period cần tạo (tháng con nếu là QUARTER/YEAR)
                var period = await _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ID == request.PeriodID);
                if (period == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy kỳ KPI"));

                var periodIds = new List<int> { request.PeriodID };
                if (period.PeriodType == "QUARTER" || period.PeriodType == "YEAR")
                {
                    var childMonths = await GetChildMonthPeriodsAsync(period);
                    periodIds.AddRange(childMonths.Select(x => x.ID));
                }

                // Lấy target đã tồn tại
                var existingTargets = await _kpiSaleRepo.KPISaleTargets.AsNoTracking()
                    .Where(x => x.EmployeeID == request.EmployeeID
                        && periodIds.Contains(x.PeriodID)
                        && indexes.Select(i => i.ID).Contains(x.KpiIndexID))
                    .Select(x => new { x.PeriodID, x.KpiIndexID })
                    .ToListAsync();

                var existingSet = existingTargets
                    .Select(x => (x.PeriodID, x.KpiIndexID))
                    .ToHashSet();

                // Tạo target mới cho index chưa có target
                var toCreate = new List<KPISaleTarget>();
                foreach (var periodId in periodIds)
                {
                    foreach (var index in indexes)
                    {
                        if (existingSet.Contains((periodId, index.ID)))
                            continue;

                        var model = new KPISaleTarget
                        {
                            EmployeeID = request.EmployeeID,
                            PeriodID = periodId,
                            KpiIndexID = index.ID,
                            GoalValue = 0,
                            WeightPercent = 0,
                            ProposedGoalValue = 0,
                            ApprovalStatus = "Pending",
                            CreatedBy = currentUser.LoginName,
                            UpdatedBy = currentUser.LoginName,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        };
                        toCreate.Add(model);
                    }
                }

                if (toCreate.Count > 0)
                {
                    await _kpiSaleRepo.KPISaleTargets.AddRangeAsync(toCreate);
                    await _kpiSaleRepo.SaveChangesAsync();
                }

                return Ok(ApiResponseFactory.Success(new
                {
                    CreatedCount = toCreate.Count,
                    TotalIndexes = indexes.Count,
                    PeriodCount = periodIds.Count
                }, $"Đã thêm {toCreate.Count} chỉ tiêu mới (weight=0, goal=0)"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("targets/import-excel")]
        public async Task<IActionResult> ImportTargets([FromBody] List<KPISaleTargetUpsertRequest> requests)
        {
            try
            {
                if (requests == null || requests.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu import không được để trống"));

                var currentUser = GetCurrentUser();
                var created = 0;
                var updated = 0;
                var data = new List<KPISaleTarget>();

                foreach (var request in requests)
                {
                    var existed = await FindTargetAsync(request);
                    data.Add(await UpsertTargetAsync(request, currentUser.LoginName, existed));

                    if (existed == null || existed.ID <= 0)
                        created++;
                    else
                        updated++;
                }

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(new { created, updated, data }, "Import target thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ============== Employee Template Assignment APIs ==============

        [HttpGet("employee-templates")]
        public async Task<IActionResult> GetEmployeeTemplates(
            int? employeeId = null,
            int? templateId = null,
            bool? isActive = null,
            int? periodId = null,
            string? periodValue = null,
            string? periodType = null)
        {
            try
            {
                var employeeList = _employeeRepo.GetAll() ?? new List<Employee>();
                var employeeById = employeeList
                    .GroupBy(e => e.ID)
                    .ToDictionary(g => g.Key, g => g.First());

                var templates = _kpiSaleRepo.KPISaleTemplates.AsNoTracking().ToList();
                var templateById = templates
                    .GroupBy(t => t.ID)
                    .ToDictionary(g => g.Key, g => g.First());

                // Load periods để trả về PeriodName
                var periods = _kpiSaleRepo.KPISalePeriods.AsNoTracking().ToList();
                var periodById = periods
                    .GroupBy(p => p.ID)
                    .ToDictionary(g => g.Key, g => g.First());

                var query = _kpiSaleRepo.KPISaleEmployeeTemplates.AsNoTracking().AsQueryable();

                if (employeeId.HasValue)
                    query = query.Where(x => x.EmployeeID == employeeId.Value);

                if (templateId.HasValue)
                    query = query.Where(x => x.TemplateID == templateId.Value);

                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                if (periodId.HasValue)
                    query = query.Where(x => x.PeriodID == periodId.Value);

                if (!string.IsNullOrWhiteSpace(periodValue))
                    query = query.Where(x => x.PeriodValue == periodValue);

                if (!string.IsNullOrWhiteSpace(periodType))
                    query = query.Where(x => x.PeriodType == periodType);

                var employeeTemplates = await query.ToListAsync();

                var data = employeeTemplates
                    .Select(et =>
                    {
                        employeeById.TryGetValue(et.EmployeeID, out var emp);
                        templateById.TryGetValue(et.TemplateID, out var tpl);
                        periodById.TryGetValue(et.PeriodID ?? 0, out var period);
                        return new
                        {
                            et.ID,
                            et.EmployeeID,
                            EmployeeCode = emp != null ? emp.Code : null,
                            EmployeeName = emp != null ? emp.FullName : null,
                            et.TemplateID,
                            TemplateCode = tpl != null ? tpl.TemplateCode : null,
                            TemplateName = tpl != null ? tpl.TemplateName : null,
                            et.PeriodType,
                            et.PeriodValue,
                            et.PeriodID,
                            PeriodName = period != null ? period.PeriodName : null,
                            et.StartDate,
                            et.EndDate,
                            et.IsActive,
                            et.AssignedDate,
                            et.AssignedBy,
                            et.UpdatedDate,
                            et.Note
                        };
                    })
                    .OrderBy(x => x.EmployeeCode)
                    .ThenBy(x => x.PeriodValue)
                    .ToList();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("employee-templates")]
        public async Task<IActionResult> CreateEmployeeTemplate([FromBody] KPISaleEmployeeTemplateUpsertRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không được để trống"));

                if (request.EmployeeID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "EmployeeID không hợp lệ"));

                if (request.TemplateID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "TemplateID không hợp lệ"));

                // Mặc định là Month nếu không truyền
                var periodType = string.IsNullOrWhiteSpace(request.PeriodType) ? "Month" : request.PeriodType.Trim();
                if (!string.Equals(periodType, "Quarter", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(periodType, "Month", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "PeriodType chỉ chấp nhận 'Quarter' hoặc 'Month'"));
                }

                if (string.IsNullOrWhiteSpace(request.PeriodValue))
                    return BadRequest(ApiResponseFactory.Fail(null, "PeriodValue không được để trống (vd: '2026-Q1' hoặc '2026-03')"));

                var currentUser = GetCurrentUser();
                var now = DateTime.Now;

                // Bước 1: Tìm các period mục tiêu từ KPISalePeriod
                List<KPISalePeriod> targetPeriods;
                if (string.Equals(periodType, "Quarter", StringComparison.OrdinalIgnoreCase))
                {
                    // Lấy quý theo PeriodValue (vd: "2026-Q1")
                    var quarter = await _kpiSaleRepo.KPISalePeriods
                        .FirstOrDefaultAsync(p => p.PeriodType == "Quarter" && p.PeriodCode == request.PeriodValue);
                    if (quarter == null)
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy kỳ quý '{request.PeriodValue}' trong hệ thống"));

                    var childMonths = await _kpiSaleRepo.KPISalePeriods
                        .Where(p => p.PeriodType == "Month" && p.ParentPeriodID == quarter.ID)
                        .OrderBy(p => p.DateStart)
                        .ToListAsync();

                    if (childMonths.Count == 0)
                        return BadRequest(ApiResponseFactory.Fail(null, $"Quý '{request.PeriodValue}' chưa có tháng con nào được cấu hình trong KPISalePeriod"));

                    // Gán cả quý và các tháng con
                    targetPeriods = new List<KPISalePeriod>();
                    targetPeriods.Add(quarter);
                    targetPeriods.AddRange(childMonths);
                }
                else // Month
                {
                    var month = await _kpiSaleRepo.KPISalePeriods
                        .FirstOrDefaultAsync(p => p.PeriodType == "Month" && p.PeriodCode == request.PeriodValue);
                    if (month == null)
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy kỳ tháng '{request.PeriodValue}' trong hệ thống"));

                    targetPeriods = new List<KPISalePeriod> { month };
                }
                // Bước 2: Với mỗi period mục tiêu, deactivate các assignment active hiện tại của employee trong period đó
                var targetPeriodIds = targetPeriods.Select(p => p.ID).ToList();
                var existingActives = await _kpiSaleRepo.KPISaleEmployeeTemplates
                    .Where(x => x.EmployeeID == request.EmployeeID
                                && x.IsActive == true
                                && x.PeriodID.HasValue
                                && targetPeriodIds.Contains(x.PeriodID.Value))
                    .ToListAsync();
                foreach (var item in existingActives)
                {
                    item.IsActive = false;
                    item.UpdatedDate = now;
                }

                // Bước 3: Với mỗi period mục tiêu, tìm assignment inactive cũ (cùng template + period) để reactivate, nếu không có thì tạo mới
                var createdRows = new List<KPISaleEmployeeTemplate>();
                foreach (var period in targetPeriods)
                {
                    var existingInactive = await _kpiSaleRepo.KPISaleEmployeeTemplates
                        .FirstOrDefaultAsync(x => x.EmployeeID == request.EmployeeID
                            && x.TemplateID == request.TemplateID
                            && x.PeriodID == period.ID
                            && x.IsActive != true);

                    KPISaleEmployeeTemplate model;
                    if (existingInactive != null)
                    {
                        existingInactive.IsActive = true;
                        existingInactive.AssignedDate = now;
                        existingInactive.AssignedBy = currentUser.LoginName;
                        existingInactive.PeriodType = "Month";
                        existingInactive.PeriodValue = period.PeriodCode;
                        existingInactive.PeriodID = period.ID;
                        existingInactive.StartDate = period.DateStart.ToDateTime(TimeOnly.MinValue);
                        existingInactive.EndDate = period.DateEnd.ToDateTime(TimeOnly.MaxValue);
                        existingInactive.Note = request.Note;
                        existingInactive.UpdatedDate = now;
                        model = existingInactive;
                    }
                    else
                    {
                        model = new KPISaleEmployeeTemplate
                        {
                            EmployeeID = request.EmployeeID,
                            TemplateID = request.TemplateID,
                            PeriodType = "Month",
                            PeriodValue = period.PeriodCode,
                            PeriodID = period.ID,
                            StartDate = period.DateStart.ToDateTime(TimeOnly.MinValue),
                            EndDate = period.DateEnd.ToDateTime(TimeOnly.MaxValue),
                            IsActive = true,
                            AssignedDate = now,
                            AssignedBy = currentUser.LoginName,
                            Note = request.Note
                        };
                        await _kpiSaleRepo.KPISaleEmployeeTemplates.AddAsync(model);
                    }
                    createdRows.Add(model);
                }

                await _kpiSaleRepo.SaveChangesAsync();

        var message = string.Equals(periodType, "Quarter", StringComparison.OrdinalIgnoreCase)
            ? $"Gán mẫu KPI cho quý {request.PeriodValue} và {targetPeriods.Count - 1} tháng thuộc quý thành công"
            : $"Gán mẫu KPI cho tháng {request.PeriodValue} thành công";

                return Ok(ApiResponseFactory.Success(createdRows, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("employee-templates/{id}")]
        public async Task<IActionResult> UpdateEmployeeTemplate(int id, [FromBody] KPISaleEmployeeTemplateUpsertRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không được để trống"));

                var currentUser = GetCurrentUser();
                var model = await _kpiSaleRepo.KPISaleEmployeeTemplates.FirstOrDefaultAsync(x => x.ID == id);
                if (model == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản ghi"));

                model.IsActive = request.IsActive ?? model.IsActive;
                model.Note = request.Note;
                model.UpdatedDate = DateTime.Now;
                model.AssignedBy = currentUser.LoginName;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Cập nhật thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("employee-templates/{id}")]
        public async Task<IActionResult> DeleteEmployeeTemplate(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleEmployeeTemplates.FirstOrDefaultAsync(x => x.ID == id);
                if (model == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản ghi"));

                _kpiSaleRepo.KPISaleEmployeeTemplates.Remove(model);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(model, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region Team Template

        /// <summary>
        /// Lấy danh sách gán mẫu KPI theo team.
        /// </summary>
        [HttpGet("team-templates")]
        public async Task<IActionResult> GetTeamTemplates(
            int? teamId = null,
            bool? isActive = null,
            string? periodValue = null)
        {
            try
            {
                var query = _kpiSaleRepo.KPISaleTeamTemplates.AsNoTracking().AsQueryable();

                if (teamId.HasValue)
                    query = query.Where(x => x.TeamID == teamId.Value);

                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                if (!string.IsNullOrWhiteSpace(periodValue))
                    query = query.Where(x => x.PeriodValue == periodValue);

                var items = await query
                    .OrderByDescending(x => x.AssignedDate)
                    .ToListAsync();

                var teams = _kpiSaleRepo.KPISaleTeams.AsNoTracking().ToList();
                var templates = _kpiSaleRepo.KPISaleTemplates.AsNoTracking().ToList();

                var data = items.Select(tt =>
                {
                    var team = teams.FirstOrDefault(t => t.ID == tt.TeamID);
                    var tpl = templates.FirstOrDefault(t => t.ID == tt.TemplateID);
                    return new KPISaleTeamTemplateResponse
                    {
                        ID = tt.ID,
                        TeamID = tt.TeamID,
                        TeamCode = team?.TeamCode,
                        TeamName = team?.TeamName,
                        TemplateID = tt.TemplateID,
                        TemplateCode = tpl?.TemplateCode,
                        TemplateName = tpl?.TemplateName,
                        PeriodType = tt.PeriodType,
                        PeriodValue = tt.PeriodValue,
                        StartDate = tt.StartDate,
                        EndDate = tt.EndDate,
                        IsActive = tt.IsActive,
                        AssignedDate = tt.AssignedDate,
                        AssignedBy = tt.AssignedBy,
                        Note = tt.Note
                    };
                }).ToList();

                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Gán mẫu KPI cho team theo quý.
        /// Tự động ghi đè EmployeeTemplate của tất cả thành viên trong team cho quý đó + các tháng con.
        /// </summary>
        [HttpPost("team-templates")]
        public async Task<IActionResult> CreateTeamTemplate([FromBody] KPISaleTeamTemplateUpsertRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không được để trống"));

                if (request.TeamID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "TeamID không hợp lệ"));

                if (request.TemplateID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "TemplateID không hợp lệ"));

                if (string.IsNullOrWhiteSpace(request.PeriodValue))
                    return BadRequest(ApiResponseFactory.Fail(null, "PeriodValue không được để trống (vd: '2026-Q1')"));

                // Tìm quý
                var quarter = await _kpiSaleRepo.KPISalePeriods
                    .FirstOrDefaultAsync(p => p.PeriodType == "Quarter" && p.PeriodCode == request.PeriodValue);
                if (quarter == null)
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy kỳ quý '{request.PeriodValue}'"));

                // Lấy các tháng con
                var childMonths = await _kpiSaleRepo.KPISalePeriods
                    .Where(p => p.PeriodType == "Month" && p.ParentPeriodID == quarter.ID)
                    .OrderBy(p => p.DateStart)
                    .ToListAsync();

                if (childMonths.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, $"Quý '{request.PeriodValue}' chưa có tháng con"));

                // Lấy danh sách thành viên team
                var memberIds = await _kpiSaleRepo.KPISaleTeamMembers
                    .Where(m => m.TeamID == request.TeamID)
                    .Select(m => m.EmployeeID)
                    .ToListAsync();

                if (memberIds.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Team không có thành viên nào"));

                var currentUser = GetCurrentUser();
                var now = DateTime.Now;
                var targetPeriods = new List<KPISalePeriod> { quarter };
                targetPeriods.AddRange(childMonths);
                var targetPeriodIds = targetPeriods.Select(p => p.ID).ToList();

                // Bước 1: Deactivate team template cũ cùng quý (chỉ giữ 1 active/team/quý)
                var existingTeamTemplates = await _kpiSaleRepo.KPISaleTeamTemplates
                    .Where(x => x.TeamID == request.TeamID && x.PeriodValue == request.PeriodValue && x.IsActive == true)
                    .ToListAsync();
                foreach (var item in existingTeamTemplates)
                {
                    item.IsActive = false;
                }

                // Bước 2: Deactivate EmployeeTemplate của tất cả thành viên trong các period mục tiêu
                var existingEmpActives = await _kpiSaleRepo.KPISaleEmployeeTemplates
                    .Where(x => memberIds.Contains(x.EmployeeID)
                                && x.IsActive == true
                                && x.PeriodID.HasValue
                                && targetPeriodIds.Contains(x.PeriodID.Value))
                    .ToListAsync();
                foreach (var item in existingEmpActives)
                {
                    item.IsActive = false;
                    item.UpdatedDate = now;
                }

                // Bước 3: Tạo team template mới
                var teamTemplate = new KPISaleTeamTemplate
                {
                    TeamID = request.TeamID,
                    TemplateID = request.TemplateID,
                    PeriodType = "Quarter",
                    PeriodValue = request.PeriodValue,
                    StartDate = quarter.DateStart.ToDateTime(TimeOnly.MinValue),
                    EndDate = quarter.DateEnd.ToDateTime(TimeOnly.MaxValue),
                    IsActive = true,
                    AssignedDate = now,
                    AssignedBy = currentUser.LoginName,
                    Note = request.Note
                };
                await _kpiSaleRepo.KPISaleTeamTemplates.AddAsync(teamTemplate);

                // Bước 4: Tạo EmployeeTemplate cho từng thành viên — QUÝ trước, rồi đến từng THÁNG con
                var createdEmpTemplates = new List<KPISaleEmployeeTemplate>();
                foreach (var empId in memberIds)
                {
                    // 4a: Tạo record cho QUÝ
                    var existingInactiveQuarter = await _kpiSaleRepo.KPISaleEmployeeTemplates
                        .FirstOrDefaultAsync(x => x.EmployeeID == empId
                            && x.TemplateID == request.TemplateID
                            && x.PeriodID == quarter.ID
                            && x.IsActive != true);

                    KPISaleEmployeeTemplate empTemplateQuarter;
                    if (existingInactiveQuarter != null)
                    {
                        existingInactiveQuarter.IsActive = true;
                        existingInactiveQuarter.AssignedDate = now;
                        existingInactiveQuarter.AssignedBy = currentUser.LoginName;
                        existingInactiveQuarter.PeriodType = "Quarter";
                        existingInactiveQuarter.PeriodValue = quarter.PeriodCode;
                        existingInactiveQuarter.StartDate = quarter.DateStart.ToDateTime(TimeOnly.MinValue);
                        existingInactiveQuarter.EndDate = quarter.DateEnd.ToDateTime(TimeOnly.MaxValue);
                        existingInactiveQuarter.Note = $"[Team Override] {request.Note ?? ""}";
                        existingInactiveQuarter.UpdatedDate = now;
                        empTemplateQuarter = existingInactiveQuarter;
                    }
                    else
                    {
                        empTemplateQuarter = new KPISaleEmployeeTemplate
                        {
                            EmployeeID = empId,
                            TemplateID = request.TemplateID,
                            PeriodType = "Quarter",
                            PeriodValue = quarter.PeriodCode,
                            PeriodID = quarter.ID,
                            StartDate = quarter.DateStart.ToDateTime(TimeOnly.MinValue),
                            EndDate = quarter.DateEnd.ToDateTime(TimeOnly.MaxValue),
                            IsActive = true,
                            AssignedDate = now,
                            AssignedBy = currentUser.LoginName,
                            Note = $"[Team Override] {request.Note ?? ""}"
                        };
                        await _kpiSaleRepo.KPISaleEmployeeTemplates.AddAsync(empTemplateQuarter);
                    }
                    createdEmpTemplates.Add(empTemplateQuarter);

                    // 4b: Tạo record cho từng THÁNG con
                    foreach (var period in childMonths)
                    {
                        var existingInactive = await _kpiSaleRepo.KPISaleEmployeeTemplates
                            .FirstOrDefaultAsync(x => x.EmployeeID == empId
                                && x.TemplateID == request.TemplateID
                                && x.PeriodID == period.ID
                                && x.IsActive != true);

                        KPISaleEmployeeTemplate empTemplate;
                        if (existingInactive != null)
                        {
                            existingInactive.IsActive = true;
                            existingInactive.AssignedDate = now;
                            existingInactive.AssignedBy = currentUser.LoginName;
                            existingInactive.PeriodType = "Month";
                            existingInactive.PeriodValue = period.PeriodCode;
                            existingInactive.StartDate = period.DateStart.ToDateTime(TimeOnly.MinValue);
                            existingInactive.EndDate = period.DateEnd.ToDateTime(TimeOnly.MaxValue);
                            existingInactive.Note = $"[Team Override] {request.Note ?? ""}";
                            existingInactive.UpdatedDate = now;
                            empTemplate = existingInactive;
                        }
                        else
                        {
                            empTemplate = new KPISaleEmployeeTemplate
                            {
                                EmployeeID = empId,
                                TemplateID = request.TemplateID,
                                PeriodType = "Month",
                                PeriodValue = period.PeriodCode,
                                PeriodID = period.ID,
                                StartDate = period.DateStart.ToDateTime(TimeOnly.MinValue),
                                EndDate = period.DateEnd.ToDateTime(TimeOnly.MaxValue),
                                IsActive = true,
                                AssignedDate = now,
                                AssignedBy = currentUser.LoginName,
                                Note = $"[Team Override] {request.Note ?? ""}"
                            };
                            await _kpiSaleRepo.KPISaleEmployeeTemplates.AddAsync(empTemplate);
                        }
                        createdEmpTemplates.Add(empTemplate);
                    }
                }

                await _kpiSaleRepo.SaveChangesAsync();

                var team = await _kpiSaleRepo.KPISaleTeams.FirstOrDefaultAsync(t => t.ID == request.TeamID);
                return Ok(ApiResponseFactory.Success(new
                {
                    TeamTemplate = teamTemplate,
                    AffectedEmployees = memberIds.Count,
                    CreatedEmployeeTemplates = createdEmpTemplates.Count
                }, $"Gán mẫu KPI '{quarter.PeriodCode}' cho team '{team?.TeamName}' ({memberIds.Count} thành viên) thành công. Đã ghi đè {createdEmpTemplates.Count} bản ghi EmployeeTemplate (QUÝ + {childMonths.Count} tháng × {memberIds.Count} người)."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Xóa team template (deactivate) — không ảnh hưởng EmployeeTemplate đã bị ghi đè.
        /// </summary>
        [HttpDelete("team-templates/{id}")]
        public async Task<IActionResult> DeleteTeamTemplate(int id)
        {
            try
            {
                var model = await _kpiSaleRepo.KPISaleTeamTemplates.FirstOrDefaultAsync(x => x.ID == id);
                if (model == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản ghi"));

                model.IsActive = false;
                await _kpiSaleRepo.SaveChangesAsync();

                return Ok(ApiResponseFactory.Success(model, "Đã xóa gán mẫu team"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] KPISaleCalculateRequest request)
        {
            try
            {
                ValidateCalculateRequest(request);

                var result = await CalculateInternalAsync(request);
                var response = new KPISaleCalculateResponse
                {
                    Items = result.Items,
                    TotalPerformance = result.TotalPerformance
                };
                return Ok(ApiResponseFactory.Success(response, "Tính KPI thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("calculate-team")]
        public async Task<IActionResult> CalculateTeam([FromBody] KPISaleTeamCalculateRequest request)
        {
            try
            {
                ValidateTeamCalculateRequest(request);

                var result = await CalculateTeamInternalAsync(request);
                return Ok(ApiResponseFactory.Success(result, "Tính KPI tổng hợp nhóm thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private static void ValidateTeamCalculateRequest(KPISaleTeamCalculateRequest request)
        {
            if (request == null)
                throw new Exception("Dữ liệu tính KPI nhóm không được để trống");

            if (request.EmployeeIDs == null || request.EmployeeIDs.Count == 0)
                throw new Exception("Vui lòng chọn ít nhất 1 nhân viên");

            if (request.PeriodID <= 0)
                throw new Exception("PeriodID không hợp lệ");

            if (request.TemplateID <= 0)
                throw new Exception("TemplateID không hợp lệ");
        }

        [HttpGet("results")]
        public async Task<IActionResult> GetResults(int? employeeId = null, int? periodId = null, int? templateId = null, int? teamId = null)
        {
            try
            {
                // Khi có teamId nhưng không có templateId → resolve template từ team
                int? resolvedTemplateId = templateId;
                if (teamId.HasValue && !templateId.HasValue)
                {
                    var teamTemplate = await _kpiSaleRepo.KPISaleTeamTemplates
                        .Where(x => x.TeamID == teamId.Value && x.IsActive == true)
                        .OrderByDescending(x => x.AssignedDate)
                        .FirstOrDefaultAsync();
                    if (teamTemplate == null && periodId.HasValue)
                    {
                        // Fallback: tìm theo PeriodValue của period được chọn
                        var period = await _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                            .FirstOrDefaultAsync(p => p.ID == periodId.Value);
                        if (period != null)
                        {
                            teamTemplate = await _kpiSaleRepo.KPISaleTeamTemplates
                                .Where(x => x.TeamID == teamId.Value
                                    && x.PeriodValue == period.PeriodCode
                                    && x.IsActive == true)
                                .OrderByDescending(x => x.AssignedDate)
                                .FirstOrDefaultAsync();
                        }
                    }
                    if (teamTemplate != null)
                        resolvedTemplateId = teamTemplate.TemplateID;
                }

                var query =
                    from result in _kpiSaleRepo.KPISaleResults.AsNoTracking()
                    join index in _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                        on result.KpiIndexID equals index.ID
                    join period in _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                        on result.PeriodID equals period.ID
                    select new
                    {
                        result.ID,
                        result.EmployeeID,
                        result.TeamID,
                        result.PeriodID,
                        period.PeriodCode,
                        period.PeriodName,
                        result.KpiIndexID,
                        index.TemplateID,
                        index.ParentID,
                        index.IndexCode,
                        index.IndexName,
                        index.UnitType,
                        index.SortOrder,
                        index.IsMainIndex,
                        index.IsBold,
                        index.IndexType,
                        result.GoalValue,
                        result.ResultValue,
                        result.AchievedPercent,
                        result.WeightPercent,
                        result.FinalScore,
                        result.ReportScoreAdjustmentType,
                        result.ReportScoreValue,
                        result.CalculatedDate
                    };

                var totalPerformanceQuery = _kpiSaleRepo.KPISaleTotalPerformances.AsNoTracking().AsQueryable();

                if (teamId.HasValue)
                {
                    query = query.Where(x => x.TeamID == teamId.Value);
                    totalPerformanceQuery = totalPerformanceQuery.Where(x => x.TeamID == teamId.Value);
                }
                else if (employeeId.HasValue)
                {
                    query = query.Where(x => x.EmployeeID == employeeId.Value && x.TeamID == null);
                    totalPerformanceQuery = totalPerformanceQuery.Where(x => x.EmployeeID == employeeId.Value && x.TeamID == null);
                }

                if (periodId.HasValue)
                {
                    query = query.Where(x => x.PeriodID == periodId.Value);
                    totalPerformanceQuery = totalPerformanceQuery.Where(x => x.PeriodID == periodId.Value);
                }

                if (resolvedTemplateId.HasValue)
                {
                    query = query.Where(x => x.TemplateID == resolvedTemplateId.Value);
                    totalPerformanceQuery = totalPerformanceQuery.Where(x => x.TemplateID == resolvedTemplateId.Value);
                }

                var data = await query
                    .OrderBy(x => x.TeamID)
                    .ThenBy(x => x.EmployeeID)
                    .ThenBy(x => x.PeriodID)
                    .ThenBy(x => x.SortOrder)
                    .ToListAsync();

                var totalPerformance = await totalPerformanceQuery
                    .OrderByDescending(x => x.CalculatedDate)
                    .ThenByDescending(x => x.ID)
                    .FirstOrDefaultAsync();

                var response = new KPISaleCalculateResponse
                {
                    Items = data.Select(x => new KPISaleCalculateResult
                    {
                        KpiIndexID = x.KpiIndexID,
                        IndexCode = x.IndexCode,
                        IndexName = x.IndexName,
                        IndexType = x.IndexType,
                        GoalValue = x.GoalValue,
                        ResultValue = x.ResultValue,
                        AchievedPercent = x.AchievedPercent,
                        WeightPercent = x.WeightPercent,
                        FinalScore = x.FinalScore,
                        UnitType = x.UnitType,
                        ReportScoreAdjustmentType = x.ReportScoreAdjustmentType,
                        ReportScoreValue = x.ReportScoreValue,
                        SortOrder = x.SortOrder,
                        IsMainIndex = x.IsMainIndex,
                        IsBold = x.IsBold,
                        ParentID = x.ParentID,
                        EmployeeID = x.EmployeeID,
                        TeamID = x.TeamID,
                        PeriodID = x.PeriodID,
                        PeriodCode = x.PeriodCode,
                        CalculatedDate = x.CalculatedDate
                    }).ToList(),
                    TotalPerformance = totalPerformance == null
                        ? null
                        : new KPISaleTotalPerformanceDto
                        {
                            ID = totalPerformance.ID,
                            EmployeeID = totalPerformance.EmployeeID,
                            PeriodID = totalPerformance.PeriodID,
                            TemplateID = totalPerformance.TemplateID,
                            FinalScore = totalPerformance.FinalScore,
                            CalculatedDate = totalPerformance.CalculatedDate
                        }
                };

                return Ok(ApiResponseFactory.Success(response, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Target and result

        #region Team (aggregate KPI cho nhóm nhiều nhân viên)

        [HttpGet("teams")]
        public async Task<IActionResult> GetTeams(string? keyword = null)
        {
            try
            {
                var teamsQuery = _kpiSaleRepo.KPISaleTeams.AsNoTracking();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var kw = keyword.Trim();
                    teamsQuery = teamsQuery.Where(t =>
                        t.TeamCode.Contains(kw) || (t.TeamName != null && t.TeamName.Contains(kw)));
                }

                var teams = await teamsQuery.OrderByDescending(t => t.IsActive).ThenByDescending(t => t.ID)
                    .Select(t => new
                    {
                        t.ID,
                        t.TeamCode,
                        t.TeamName,
                        t.Description,
                        t.LeaderEmployeeID,
                        t.IsActive,
                        t.CreatedDate
                    })
                    .ToListAsync();

                var leaderIds = teams.Where(t => t.LeaderEmployeeID.HasValue).Select(t => t.LeaderEmployeeID!.Value).Distinct().ToList();
                var leaderEmployees = _employeeRepo.GetAll(e => leaderIds.Contains(e.ID))
                    .ToDictionary(e => e.ID, e => e.FullName ?? "");

                var teamIds = teams.Select(t => t.ID).ToList();
                var members = await _kpiSaleRepo.KPISaleTeamMembers.AsNoTracking()
                    .Where(m => teamIds.Contains(m.TeamID))
                    .Select(m => new { m.TeamID, m.EmployeeID, m.IsAdmin, m.IsPM })
                    .ToListAsync();

                var result = teams.Select(t => new
                {
                    t.ID,
                    t.TeamCode,
                    t.TeamName,
                    t.Description,
                    t.LeaderEmployeeID,
                    LeaderEmployeeName = t.LeaderEmployeeID.HasValue ? leaderEmployees.GetValueOrDefault(t.LeaderEmployeeID.Value) : null,
                    t.IsActive,
                    t.CreatedDate,
                    EmployeeIDs = members.Where(m => m.TeamID == t.ID).Select(m => new { EmployeeId = m.EmployeeID, m.IsAdmin, m.IsPM }).ToList()
                });

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("teams/upsert")]
        public async Task<IActionResult> UpsertTeam([FromBody] KPISaleTeamUpsertRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu team không được để trống"));
                if (string.IsNullOrWhiteSpace(request.TeamCode))
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã team không được để trống"));
                if (request.TeamCode.Length > 50)
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã team tối đa 50 ký tự"));
                if (string.IsNullOrWhiteSpace(request.TeamName))
                    return BadRequest(ApiResponseFactory.Fail(null, "Tên team không được để trống"));
                if (request.TeamName.Length > 200)
                    return BadRequest(ApiResponseFactory.Fail(null, "Tên team tối đa 200 ký tự"));
                if (request.EmployeeIDs == null || request.EmployeeIDs.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Team phải có ít nhất 1 thành viên"));
                if (request.EmployeeIDs.Count > 50)
                    return BadRequest(ApiResponseFactory.Fail(null, "Team tối đa 50 thành viên"));

                var memberItems = request.EmployeeIDs
                    .GroupBy(m => m.EmployeeId)
                    .Select(g => g.First())
                    .ToList();
                var distinctIds = memberItems.Select(m => m.EmployeeId).ToList();

                if (request.LeaderEmployeeID.HasValue && !distinctIds.Contains(request.LeaderEmployeeID.Value))
                    return BadRequest(ApiResponseFactory.Fail(null, "Trưởng nhóm phải là thành viên trong team"));

                // Cấm dùng mã trùng pattern team auto-created cũ (T_<hex>)
                if (IsAutoCreatedTeamCode(request.TeamCode))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã team '{request.TeamCode}' không hợp lệ (trùng pattern team tự động)"));

                if (request.ID.HasValue && request.ID.Value > 0)
                {
                    // Update
                    var existing = await _kpiSaleRepo.KPISaleTeams
                        .FirstOrDefaultAsync(t => t.ID == request.ID.Value);
                    if (existing == null)
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy team"));

                    if (IsAutoCreatedTeamCode(existing.TeamCode))
                        return BadRequest(ApiResponseFactory.Fail(null, "Team tự động (auto-created) không thể sửa từ UI"));

                    // Check trùng mã (nếu đổi mã)
                    if (!string.Equals(existing.TeamCode, request.TeamCode, StringComparison.Ordinal))
                    {
                        var dup = await _kpiSaleRepo.KPISaleTeams.AsNoTracking()
                            .AnyAsync(t => t.TeamCode == request.TeamCode && t.ID != existing.ID);
                        if (dup)
                            return BadRequest(ApiResponseFactory.Fail(null, $"Mã team '{request.TeamCode}' đã tồn tại"));
                    }

                    existing.TeamCode = request.TeamCode;
                    existing.TeamName = request.TeamName;
                    existing.Description = request.Description;
                    existing.LeaderEmployeeID = request.LeaderEmployeeID;
                    existing.UpdatedDate = DateTime.Now;

                    // Xóa member cũ, thêm lại
                    var oldMembers = await _kpiSaleRepo.KPISaleTeamMembers
                        .Where(m => m.TeamID == existing.ID)
                        .ToListAsync();
                    if (oldMembers.Count > 0)
                        _kpiSaleRepo.KPISaleTeamMembers.RemoveRange(oldMembers);

                    foreach (var item in memberItems)
                    {
                        await _kpiSaleRepo.KPISaleTeamMembers.AddAsync(new KPISaleTeamMember
                        {
                            TeamID = existing.ID,
                            EmployeeID = item.EmployeeId,
                            IsAdmin = item.IsAdmin,
                            IsPM = item.IsPM,
                            CreatedDate = DateTime.Now
                        });
                    }
                    await _kpiSaleRepo.SaveChangesAsync();

                    return Ok(ApiResponseFactory.Success(new { id = existing.ID }, "Cập nhật team thành công"));
                }
                else
                {
                    // Create
                    var dup = await _kpiSaleRepo.KPISaleTeams.AsNoTracking()
                        .AnyAsync(t => t.TeamCode == request.TeamCode);
                    if (dup)
                        return BadRequest(ApiResponseFactory.Fail(null, $"Mã team '{request.TeamCode}' đã tồn tại"));

                    var team = new KPISaleTeam
                    {
                        TeamCode = request.TeamCode,
                        TeamName = request.TeamName,
                        Description = request.Description,
                        LeaderEmployeeID = request.LeaderEmployeeID,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };
                    await _kpiSaleRepo.KPISaleTeams.AddAsync(team);
                    await _kpiSaleRepo.SaveChangesAsync();

                    foreach (var item in memberItems)
                    {
                        await _kpiSaleRepo.KPISaleTeamMembers.AddAsync(new KPISaleTeamMember
                        {
                            TeamID = team.ID,
                            EmployeeID = item.EmployeeId,
                            IsAdmin = item.IsAdmin,
                            IsPM = item.IsPM,
                            CreatedDate = DateTime.Now
                        });
                    }
                    await _kpiSaleRepo.SaveChangesAsync();

                    return Ok(ApiResponseFactory.Success(new { id = team.ID }, "Tạo team thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("teams/{id:int}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            try
            {
                var team = await _kpiSaleRepo.KPISaleTeams
                    .FirstOrDefaultAsync(t => t.ID == id);
                if (team == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy team"));

                if (IsAutoCreatedTeamCode(team.TeamCode))
                    return BadRequest(ApiResponseFactory.Fail(null, "Team tự động (auto-created) không thể xóa từ UI"));

                var hasResult = await _kpiSaleRepo.KPISaleResults
                    .AnyAsync(r => r.TeamID == id);
                if (hasResult)
                    return BadRequest(ApiResponseFactory.Fail(null, "Team đã có kết quả KPI, không thể xóa. Có thể ngưng dùng (IsActive=false) thay thế."));

                // Soft delete: set IsActive = false
                team.IsActive = false;
                team.UpdatedDate = DateTime.Now;
                await _kpiSaleRepo.SaveChangesAsync();

                return Ok(ApiResponseFactory.Success(null, "Ngưng sử dụng team thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("teams/my-leader-teams")]
        public async Task<IActionResult> GetMyLeaderTeams()
        {
            try
            {
                var currentUser = GetCurrentUser();
                if (currentUser?.EmployeeID == null)
                    return Ok(ApiResponseFactory.Success(new { isLeader = false, teams = Array.Empty<object>() }));

                var myTeams = await _kpiSaleRepo.KPISaleTeams.AsNoTracking()
                    .Where(t => t.IsActive && t.LeaderEmployeeID == currentUser.EmployeeID)
                    .Select(t => new
                    {
                        t.ID,
                        t.TeamCode,
                        t.TeamName,
                        t.LeaderEmployeeID
                    })
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(new
                {
                    isLeader = myTeams.Count > 0,
                    teams = myTeams
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("teams/{id:int}/members")]
        public async Task<IActionResult> GetTeamMembers(int id)
        {
            try
            {
                var team = await _kpiSaleRepo.KPISaleTeams.AsNoTracking()
                    .FirstOrDefaultAsync(t => t.ID == id);
                if (team == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy team"));

                var members = await _kpiSaleRepo.KPISaleTeamMembers.AsNoTracking()
                    .Where(m => m.TeamID == id)
                    .Select(m => new { m.EmployeeID, m.IsAdmin, m.IsPM })
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(members, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private async Task<KPISaleCalculateResponse> CalculateTeamInternalAsync(KPISaleTeamCalculateRequest request)
        {
            // 1. Bắt buộc phải chọn team đã khai báo - KHÔNG tự tạo team nữa
            if (!request.TeamID.HasValue || request.TeamID.Value <= 0)
                throw new Exception("Vui lòng chọn team đã khai báo trước khi tính KPI nhóm");

            var team = await _kpiSaleRepo.KPISaleTeams.AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == request.TeamID.Value);
            if (team == null)
                throw new Exception("Không tìm thấy team");

            var teamMemberIds = await _kpiSaleRepo.KPISaleTeamMembers.AsNoTracking()
                .Where(m => m.TeamID == team.ID)
                .Select(m => m.EmployeeID)
                .ToListAsync();
            if (teamMemberIds.Count == 0)
                throw new Exception("Team không có thành viên");

            var teamId = team.ID;

            // 2. Tính KPI cho từng employee
            //    - Nếu RecalcPerEmployee=true: gọi CalculateInternalAsync với SaveSnapshot=true để lưu lại data cá nhân
            //    - Nếu RecalcPerEmployee=false: chỉ lấy kết quả tạm (không lưu), dùng cho việc tổng hợp team
            var perEmployeeResults = new List<KPISaleCalculateResponse>();
            foreach (var empId in request.EmployeeIDs)
            {
                var singleRequest = new KPISaleCalculateRequest
                {
                    EmployeeID = empId,
                    PeriodID = request.PeriodID,
                    TemplateID = request.TemplateID,
                    DepartmentID = request.DepartmentID,
                    SaveSnapshot = request.RecalcPerEmployee,
                    ReportAdjustments = new List<KPISaleReportAdjustmentInputDto>()
                };
                perEmployeeResults.Add(await CalculateInternalAsync(singleRequest));
            }

            // 3. Load template + indexes + scoring rules
            var period = await _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == request.PeriodID);
            if (period == null || period.ID <= 0)
                throw new Exception("Không tìm thấy kỳ KPI");

            var indexes = await _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                .Where(x => x.TemplateID == request.TemplateID && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .ToListAsync();

            var indexIds = indexes.Select(x => x.ID).ToList();
            var scoringRulesList = await _kpiSaleRepo.KPISaleScoringRules.AsNoTracking()
                .Where(x => indexIds.Contains(x.KpiIndexID))
                .ToListAsync();
            var scoringRules = scoringRulesList
                .GroupBy(x => x.KpiIndexID)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.ID).First());

            var allItems = perEmployeeResults
                .SelectMany(r => r.Items ?? new List<KPISaleCalculateResult>())
                .ToList();

            // 4. Build dictionary of REPORT adjustment overrides from request
            var reportOverrides = (request.ReportAdjustments ?? new List<KPISaleReportAdjustmentInputDto>())
                .Where(a => a != null && a.KpiIndexID > 0)
                .ToDictionary(a => a.KpiIndexID);

            // 4.1. Nếu kỳ là QUARTER/YEAR: với REPORT, tổng hợp từ dữ liệu các tháng con của cả team
            //      (sum reportScoreValue của TẤT CẢ employee trong team cho mỗi tháng, rồi TB cộng các tháng / totalMonthCount).
            var reportAggregationsByIndex = new Dictionary<int, (decimal AvgReport, int AdjType)>();
            var periodTypeNorm = NormalizeOptionalCode(period.PeriodType, "MONTH");
            if (periodTypeNorm == "QUARTER" || periodTypeNorm == "YEAR")
            {
                var childMonthPeriods = await GetChildMonthPeriodsAsync(period);
                if (childMonthPeriods.Count > 0)
                {
                    var childPeriodIds = childMonthPeriods.Select(x => x.ID).ToList();
                    var totalMonthCount = childMonthPeriods.Count;

                    var reportIndexIds = indexes
                        .Where(x => NormalizeOptionalCode(x.IndexType, "DETAIL") == "REPORT")
                        .Select(x => x.ID)
                        .ToList();

                    if (reportIndexIds.Count > 0)
                    {
                        // Lấy dữ liệu REPORT của TEAM ở các tháng con (lưu bởi SaveTeamSnapshotAsync khi user
                        // tính KPI team ở từng tháng). KHÔNG fallback từ employee - dữ liệu REPORT của team
                        // phải do user tự nhập theo từng tháng.
                        var childTeamResults = await _kpiSaleRepo.KPISaleResults.AsNoTracking()
                            .Where(x => x.TeamID == teamId
                                && childPeriodIds.Contains(x.PeriodID)
                                && reportIndexIds.Contains(x.KpiIndexID)
                                && x.KpiIndexID > 0)
                            .ToListAsync();

                        // Group theo (PeriodID, KpiIndexID) rồi sum ReportScoreValue (mỗi tháng 1 record / team).
                        // Sau đó cộng các tháng / totalMonthCount = TB cộng các tháng của team.
                        // AdjustmentType lấy từ tháng có dữ liệu mới nhất (PeriodID lớn nhất, value > 0).
                        var monthAggByIndex = childTeamResults
                            .GroupBy(x => x.KpiIndexID)
                            .ToDictionary(
                                g => g.Key,
                                g => new
                                {
                                    Sum = g.Sum(x => x.ReportScoreValue ?? 0),
                                    AdjustmentType = g
                                        .Where(x => (x.ReportScoreValue ?? 0) > 0)
                                        .OrderByDescending(x => x.PeriodID)
                                        .FirstOrDefault()?.ReportScoreAdjustmentType
                                        ?? g.OrderByDescending(x => x.PeriodID).FirstOrDefault()?.ReportScoreAdjustmentType
                                        ?? 0
                                });

                        foreach (var indexId in reportIndexIds)
                        {
                            if (monthAggByIndex.TryGetValue(indexId, out var data))
                            {
                                // TB cộng các tháng (chia cho số tháng cố định - tháng thiếu coi = 0)
                                reportAggregationsByIndex[indexId] = (data.Sum / totalMonthCount, data.AdjustmentType);
                            }
                            else
                            {
                                reportAggregationsByIndex[indexId] = (0m, 0);
                            }
                        }
                    }
                }
            }

            // 5. Aggregate Items theo KpiIndexID
            var aggregatedItems = new List<KPISaleCalculateResult>();
            foreach (var index in indexes)
            {
                var itemsForIndex = allItems.Where(i => i.KpiIndexID == index.ID).ToList();
                var indexType = NormalizeOptionalCode(index.IndexType, "DETAIL");

                if (indexType == "REPORT")
                {
                    decimal avgReport;
                    int adjType;

                    // Ưu tiên rule QUARTER/YEAR: TB cộng các tháng của team
                    if (reportAggregationsByIndex.TryGetValue(index.ID, out var quarterAgg))
                    {
                        avgReport = quarterAgg.AvgReport;
                        adjType = quarterAgg.AdjType;
                    }
                    else
                    {
                        // MONTH hoặc không có dữ liệu tháng: lấy trung bình ReportScoreValue từ các employee trong team
                        // cho cùng period, giữ AdjustmentType từ lần ghi nhận mới nhất.
                        var reportItems = itemsForIndex
                            .Where(i => i.ReportScoreValue.HasValue && i.ReportScoreValue.Value > 0)
                            .ToList();
                        avgReport = reportItems.Count > 0
                            ? reportItems.Average(i => i.ReportScoreValue ?? 0)
                            : 0;
                        adjType = reportItems
                            .OrderByDescending(i => i.CalculatedDate ?? DateTime.MinValue)
                            .FirstOrDefault()?.ReportScoreAdjustmentType ?? 0;
                    }

                    // Áp dụng override từ request (người dùng tự điền tay)
                    if (reportOverrides.TryGetValue(index.ID, out var overrideAdj))
                    {
                        if (overrideAdj.ReportScoreAdjustmentType.HasValue)
                            adjType = overrideAdj.ReportScoreAdjustmentType.Value;
                        if (overrideAdj.ReportScoreValue.HasValue)
                            avgReport = overrideAdj.ReportScoreValue.Value;
                    }

                    var reportFinalScore = adjType == 2
                        ? avgReport
                        : adjType == 1
                            ? -avgReport
                            : 0;

                    aggregatedItems.Add(new KPISaleCalculateResult
                    {
                        KpiIndexID = index.ID,
                        ParentID = index.ParentID,
                        EmployeeID = 0,
                        TeamID = teamId,
                        PeriodID = request.PeriodID,
                        PeriodCode = period?.PeriodCode,
                        CalculatedDate = DateTime.Now,
                        IndexCode = index.IndexCode,
                        IndexName = index.IndexName,
                        IndexType = index.IndexType,
                        GoalValue = 0,
                        ResultValue = 0,
                        AchievedPercent = 0,
                        WeightPercent = itemsForIndex.FirstOrDefault(i => i.WeightPercent > 0)?.WeightPercent ?? 0,
                        FinalScore = reportFinalScore,
                        UnitType = index.UnitType,
                        ReportScoreAdjustmentType = adjType,
                        ReportScoreValue = avgReport,
                        SortOrder = index.SortOrder,
                        IsMainIndex = index.IsMainIndex,
                        IsBold = index.IsBold
                    });
                }
                else
                {
                    // REPORT: đã xử lý ở trên
                    // DETAIL: chỉ aggregate từ employee có weightPercent hợp lệ
                    // GROUP/FORMULA: vẫn xử lý ngay cả khi không có validItems
                    //   (goal sẽ được sum từ các con ở CalculateInternalAsync, đã chạy rồi)
                    var validItems = itemsForIndex
                        .Where(i => i.WeightPercent > 0)
                        .ToList();

                    // Với GROUP/FORMULA, luôn tạo aggregated item nếu có dữ liệu từ team.
                    // validItems.Count == 0 chỉ có nghĩa là không employee nào trong team
                    // có weight trực tiếp cho index này. Điều đó không có nghĩa là
                    // GROUP/FORMULA không có goal — goal đến từ con.
                    if (validItems.Count == 0 && indexType != "GROUP" && indexType != "FORMULA")
                        continue;

                    decimal sumGoal = validItems.Sum(i => i.GoalValue);
                    decimal sumResult = validItems.Sum(i => i.ResultValue);

                    scoringRules.TryGetValue(index.ID, out var scoringRule);
                    var scoreType = NormalizeOptionalCode(scoringRule?.ScoreType, "NORMAL_PERCENT");
                    var achievedPercent = CalculateAchievedPercent(sumGoal, sumResult, scoreType);
                    // Với GROUP/FORMULA, weight lấy từ index đầu tiên có weight hợp lệ trong team;
                    // nếu không có, weightPercent = 0 (GROUP không cần weight để sum goal)
                    decimal weightPercent = validItems.Count > 0 ? validItems[0].WeightPercent : 0;
                    var finalScore = CalculateFinalScore(achievedPercent, weightPercent, scoringRule, scoreType);

                    aggregatedItems.Add(new KPISaleCalculateResult
                    {
                        KpiIndexID = index.ID,
                        ParentID = index.ParentID,
                        EmployeeID = 0,
                        TeamID = teamId,
                        PeriodID = request.PeriodID,
                        PeriodCode = period?.PeriodCode,
                        CalculatedDate = DateTime.Now,
                        IndexCode = index.IndexCode,
                        IndexName = index.IndexName,
                        IndexType = index.IndexType,
                        GoalValue = sumGoal,
                        ResultValue = sumResult,
                        AchievedPercent = achievedPercent,
                        WeightPercent = weightPercent,
                        FinalScore = finalScore,
                        UnitType = index.UnitType,
                        ReportScoreAdjustmentType = 0,
                        ReportScoreValue = 0,
                        SortOrder = index.SortOrder,
                        IsMainIndex = index.IsMainIndex,
                        IsBold = index.IsBold
                    });
                }
            }

            // 6. Tính Total Performance
            var totalFinalScore = aggregatedItems.Sum(x => x.FinalScore);
            var totalPerformance = new KPISaleTotalPerformanceDto
            {
                EmployeeID = 0,
                PeriodID = request.PeriodID,
                TemplateID = request.TemplateID,
                FinalScore = totalFinalScore,
                CalculatedDate = DateTime.Now
            };

            // 7. Snapshot: xoá dữ liệu team cũ + insert mới
            if (request.SaveSnapshot)
            {
                await SaveTeamSnapshotAsync(teamId, request, aggregatedItems, totalPerformance);
            }

            return new KPISaleCalculateResponse
            {
                Items = aggregatedItems,
                TotalPerformance = totalPerformance
            };
        }

        private async Task SaveTeamSnapshotAsync(int teamId,
            KPISaleTeamCalculateRequest request,
            List<KPISaleCalculateResult> results,
            KPISaleTotalPerformanceDto totalPerformance)
        {
            var kpiIndexIds = results.Select(x => x.KpiIndexID).ToList();
            var oldResults = await _kpiSaleRepo.KPISaleResults
                .Where(x => x.TeamID == teamId
                    && x.PeriodID == request.PeriodID
                    && kpiIndexIds.Contains(x.KpiIndexID))
                .ToListAsync();

            if (oldResults.Count > 0)
                _kpiSaleRepo.KPISaleResults.RemoveRange(oldResults);

            var oldTotal = await _kpiSaleRepo.KPISaleTotalPerformances
                .FirstOrDefaultAsync(x => x.TeamID == teamId
                    && x.PeriodID == request.PeriodID
                    && x.TemplateID == request.TemplateID);

            if (oldTotal != null)
                _kpiSaleRepo.KPISaleTotalPerformances.Remove(oldTotal);

            var now = DateTime.Now;
            var newResults = results.Select(x => new KPISaleResult
            {
                EmployeeID = 0,
                TeamID = teamId,
                PeriodID = x.PeriodID ?? request.PeriodID,
                KpiIndexID = x.KpiIndexID,
                GoalValue = x.GoalValue,
                ResultValue = x.ResultValue,
                AchievedPercent = x.AchievedPercent,
                WeightPercent = x.WeightPercent,
                FinalScore = x.FinalScore,
                ReportScoreAdjustmentType = x.ReportScoreAdjustmentType ?? 0,
                ReportScoreValue = x.ReportScoreValue ?? 0,
                CalculatedDate = now
            }).ToList();

            await _kpiSaleRepo.KPISaleResults.AddRangeAsync(newResults);
            await _kpiSaleRepo.KPISaleTotalPerformances.AddAsync(new KPISaleTotalPerformance
            {
                EmployeeID = 0,
                TeamID = teamId,
                PeriodID = request.PeriodID,
                TemplateID = request.TemplateID,
                FinalScore = totalPerformance.FinalScore,
                CalculatedDate = totalPerformance.CalculatedDate ?? now
            });

            await _kpiSaleRepo.SaveChangesAsync();
        }

        private async Task<int> CreateOrGetTransientTeamAsync(List<int> employeeIds)
        {
            // Nếu chưa có team entity, tạo 1 team "transient" với mã theo hash tổ hợp EmployeeIDs
            var sortedIds = employeeIds.Distinct().OrderBy(x => x).ToList();
            var code = $"T_{Math.Abs(string.Join(",", sortedIds).GetHashCode()):X}";

            var existing = await _kpiSaleRepo.KPISaleTeams.AsNoTracking()
                .FirstOrDefaultAsync(t => t.TeamCode == code);
            if (existing != null)
                return existing.ID;

            var team = new KPISaleTeam
            {
                TeamCode = code,
                TeamName = $"Team {code} ({sortedIds.Count} người)",
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            await _kpiSaleRepo.KPISaleTeams.AddAsync(team);
            await _kpiSaleRepo.SaveChangesAsync();

            foreach (var empId in sortedIds)
            {
                var existsMember = await _kpiSaleRepo.KPISaleTeamMembers
                    .AnyAsync(m => m.TeamID == team.ID && m.EmployeeID == empId);
                if (existsMember) continue;

                await _kpiSaleRepo.KPISaleTeamMembers.AddAsync(new KPISaleTeamMember
                {
                    TeamID = team.ID,
                    EmployeeID = empId,
                    CreatedDate = DateTime.Now
                });
            }
            await _kpiSaleRepo.SaveChangesAsync();
            return team.ID;
        }

        #endregion Team

        #region Reward & Ranking

        [HttpGet("ranking/results")]
        public async Task<IActionResult> GetRankingResults(int periodId, int templateId, string? teamCode = null)
        {
            try
            {
                var query = _kpiSaleRepo.KPISaleRankingResult.AsNoTracking()
                    .Where(r => r.PeriodId == periodId && r.TemplateId == templateId);

                if (!string.IsNullOrWhiteSpace(teamCode))
                    query = query.Where(r => r.TeamCode == teamCode);

                var results = await query
                    .OrderBy(r => r.Ranking)
                    .Select(r => new KPISaleRankingSummaryDto
                    {
                        EmployeeId = r.EmployeeId ?? 0,
                        EmployeeCode = r.EmployeeCode ?? "",
                        EmployeeName = r.EmployeeName ?? "",
                        TeamCode = r.TeamCode ?? "",
                        PositionType = r.PositionType ?? "",
                        TotalSalesAmount = r.TotalSalesAmount ?? 0,
                        TotalRevenue = r.TotalRevenue,
                        AchievementPercent = r.AchievementPercent ?? 0,
                        Coefficient = r.Coefficient ?? 0,
                        Rank = r.Ranking,
                        SalesBonusAmount = r.SalesBonusAmount ?? 0,
                        RankingBonusAmount = r.RankingBonusAmount ?? 0,
                        NewAccountCount = r.NewAccountCount ?? 0,
                        NewAccountBonus = r.NewAccountBonus ?? 0,
                        OtherBonus = r.OtherBonus ?? 0,
                        TotalBonus = r.TotalBonus ?? 0
                    })
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(results, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("ranking/calculate")]
        public async Task<IActionResult> CalculateRanking([FromBody] KPISaleRankingCalculateRequest request)
        {
            try
            {
                // Load reward config - ưu tiên Config có TemplateId khớp với Template đang chọn,
                // nếu không có thì fallback Config chung (TemplateId = null).
                var configs = await _kpiSaleRepo.KPISaleRewardConfig.AsNoTracking()
                    .Where(c => c.IsActive == true && (c.TemplateId == null || c.TemplateId == request.TemplateId))
                    .ToListAsync();

                // Sắp xếp theo độ ưu tiên: config có TemplateId khớp trước, null sau
                var prioritizedConfigs = configs
                    .OrderByDescending(c => c.TemplateId == request.TemplateId)
                    .ToList();

                // Helper: lấy config theo EmployeeType, ưu tiên TemplateId khớp rồi mới fallback null
                KPISaleRewardConfig? GetConfig(string employeeType) =>
                    prioritizedConfigs.FirstOrDefault(c => c.EmployeeType == employeeType);

                var salesConfig = GetConfig("SALES");
                var adminConfig = GetConfig("ADMIN");
                var pmConfig = GetConfig("PM");
                var leaderConfig = GetConfig("SALES_LEADER");

                var defaultRank1Bonus = salesConfig?.Rank1BonusAmount ?? 3000000m;
                var defaultRewardRate = salesConfig?.RewardRate ?? 0.01m;

                // 2. Load coefficients
                var coefficients = await _kpiSaleRepo.KPISaleRewardCoefficient.AsNoTracking()
                    .Where(c => c.IsActive == true)
                    .OrderBy(c => c.Priority)
                    .ToListAsync();

                // 3. Load KPI Total Performance
                var totalPerformances = await _kpiSaleRepo.KPISaleTotalPerformances.AsNoTracking()
                    .Where(tp => tp.PeriodID == request.PeriodId && tp.TemplateID == request.TemplateId
                        && tp.EmployeeID.HasValue && tp.EmployeeID.Value > 0)
                    .ToListAsync();

                if (totalPerformances.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu KPI để tính ranking"));

                // 4. Load employee -> reward mapping
                var employeeIds = totalPerformances
                    .Where(tp => tp.EmployeeID.HasValue && tp.EmployeeID.Value > 0)
                    .Select(tp => tp.EmployeeID!.Value)
                    .Distinct()
                    .ToList();

                var mappings = await _kpiSaleRepo.KPISaleEmployeeRewardMapping.AsNoTracking()
                    .Where(m => m.EmployeeId.HasValue && employeeIds.Contains(m.EmployeeId.Value) && m.IsActive == true)
                    .ToListAsync();
                var mappingByEmp = mappings.ToDictionary(m => m.EmployeeId!.Value, m => m);

                // 4.1 Load Team Members -> Team lookup (ưu tiên từ KPISaleTeamMember, fallback sang mapping)
                var teamMembers = await _kpiSaleRepo.KPISaleTeamMembers.AsNoTracking()
                    .Where(m => employeeIds.Contains(m.EmployeeID))
                    .ToListAsync();
                var teamIds = teamMembers.Select(m => m.TeamID).Distinct().ToList();
                var teams = await _kpiSaleRepo.KPISaleTeams.AsNoTracking()
                    .Where(t => teamIds.Contains(t.ID))
                    .ToListAsync();
                var teamById = teams.ToDictionary(t => t.ID, t => t);
                var teamCodeByEmp = teamMembers.ToDictionary(m => m.EmployeeID, m => teamById.GetValueOrDefault(m.TeamID)?.TeamCode ?? "");
                var leaderIdsByTeam = teams
                    .Where(t => t.LeaderEmployeeID.HasValue)
                    .ToDictionary(t => t.ID, t => t.LeaderEmployeeID!.Value);

                // 4.3 Load team-level KPI data để LEADER lấy KPI của cả team
                //    - KPISaleTotalPerformance(TeamID, EmployeeID=0) cho FinalScore
                //    - KPISaleResults(TeamID, EmployeeID=0) cho TotalSales / TotalRevenue
                var teamLevelTotalPerf = await _kpiSaleRepo.KPISaleTotalPerformances.AsNoTracking()
                    .Where(tp => tp.TeamID.HasValue && tp.EmployeeID == 0
                        && tp.PeriodID == request.PeriodId && tp.TemplateID == request.TemplateId)
                    .ToDictionaryAsync(tp => tp.TeamID!.Value, tp => tp.FinalScore ?? 0);

                var teamLevelResultIndexIds = new HashSet<int>();
                if (teamLevelTotalPerf.Count > 0)
                {
                    var teamIdsWithData = teamLevelTotalPerf.Keys.ToList();
                    teamLevelResultIndexIds = (await _kpiSaleRepo.KPISaleResults.AsNoTracking()
                        .Where(r => teamIdsWithData.Contains(r.TeamID ?? 0)
                            && r.EmployeeID == 0 && r.PeriodID == request.PeriodId
                            && r.TeamID.HasValue && r.TeamID > 0)
                        .Select(r => r.KpiIndexID)
                        .Distinct()
                        .ToListAsync()).ToHashSet();
                }

                // teamLevelResults[key=TeamID, value=dict(key=KpiIndexID, value=ResultValue)]
                var teamLevelResults = new Dictionary<int, Dictionary<int, decimal>>();
                if (teamLevelResultIndexIds.Count > 0 && teamIds.Count > 0)
                {
                    var rawTeamResults = await _kpiSaleRepo.KPISaleResults.AsNoTracking()
                        .Where(r => teamIds.Contains(r.TeamID ?? 0)
                            && r.EmployeeID == 0 && r.PeriodID == request.PeriodId
                            && teamLevelResultIndexIds.Contains(r.KpiIndexID))
                        .ToListAsync();
                    teamLevelResults = rawTeamResults
                        .GroupBy(r => r.TeamID ?? 0)
                        .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.KpiIndexID, x => x.ResultValue));
                }

                // 4.2 Load Employee info (EmployeeId trong KPISaleRankingResult thực ra là Employee.UserID)
                var employees = _employeeRepo.GetAll(e => e.UserID.HasValue && employeeIds.Contains(e.UserID.Value));
                var employeeByUserId = employees
                    .Where(e => e.UserID.HasValue)
                    .ToDictionary(e => e.UserID!.Value, e => (e.Code ?? "", e.FullName ?? ""));

                // 5. Build per-employee metrics (2 pass: tính hệ số/doanh số trước, rồi mới tính thưởng DS theo team)
                var empMetrics = new List<(int EmpId, string EmpCode, string EmpName, string TeamCode, string PositionType,
                    decimal Achievement, decimal Coefficient, decimal TotalSales, decimal TotalRevenue,
                    decimal NewAccountCount, decimal NewAccountBonus)>();

                foreach (var tp in totalPerformances)
                {
                    if (!tp.EmployeeID.HasValue || tp.EmployeeID.Value <= 0) continue;
                    var empId = tp.EmployeeID.Value;

                    var mapping = mappingByEmp.GetValueOrDefault(empId);

                    // Ưu tiên PositionType: Leader team > IsAdmin/IsPM > Mapping > SALES_STAFF
                    string positionType;
                    var memberInfo = teamMembers.FirstOrDefault(m => m.EmployeeID == empId);
                    var leaderTeamId = memberInfo?.TeamID;
                    if (leaderTeamId.HasValue && leaderIdsByTeam.GetValueOrDefault(leaderTeamId.Value) == empId)
                    {
                        positionType = "LEADER";
                    }
                    else if (memberInfo?.IsAdmin == true)
                    {
                        positionType = "ADMIN";
                    }
                    else if (memberInfo?.IsPM == true)
                    {
                        positionType = "PM";
                    }
                    else
                    {
                        positionType = mapping?.PositionType ?? "SALES_STAFF";
                    }

                    employeeByUserId.TryGetValue(empId, out var empInfo);
                    var employeeCode = empInfo.Item1;
                    var employeeName = empInfo.Item2;

                    var rewardRate = positionType switch
                    {
                        "ADMIN" => adminConfig?.RewardRate ?? 0.001m,
                        "PM" => pmConfig?.RewardRate ?? 0.003m,
                        "LEADER" => leaderConfig?.RewardRate ?? salesConfig?.RewardRate ?? defaultRewardRate,
                        _ => salesConfig?.RewardRate ?? defaultRewardRate
                    };

                    var achievementPercent = tp.FinalScore ?? 0;
                    var coefficient = CalculateCoefficient(achievementPercent, positionType, coefficients);

                    // LEADER: lấy FinalScore từ KPI cả team (đã tính ở bước Tính KPI theo Team)
                    var personalAchievementPercent = achievementPercent;
                    var personalCoefficient = coefficient;
                    if (positionType == "LEADER" && leaderTeamId.HasValue)
                    {
                        if (teamLevelTotalPerf.TryGetValue(leaderTeamId.Value, out var teamScore))
                        {
                            achievementPercent = teamScore;
                            coefficient = CalculateCoefficient(achievementPercent, positionType, coefficients);
                        }
                    }

                    // TotalSalesAmount từ chỉ số "Doanh số" nếu có
                    var salesAmountKpiIndexId = positionType switch
                    {
                        "ADMIN" => adminConfig?.SalesAmountKpiIndexId,
                        "PM" => pmConfig?.SalesAmountKpiIndexId,
                        "LEADER" => leaderConfig?.SalesAmountKpiIndexId ?? salesConfig?.SalesAmountKpiIndexId,
                        _ => salesConfig?.SalesAmountKpiIndexId
                    };
                    var totalSales = GetTotalSalesAmount(empId, request.PeriodId, request.TemplateId, salesAmountKpiIndexId);

                    // TotalRevenue từ chỉ số "Doanh số" (khác với Tiền về) nếu có
                    var revenueKpiIndexId = positionType switch
                    {
                        "ADMIN" => adminConfig?.RevenueKpiIndexId,
                        "PM" => pmConfig?.RevenueKpiIndexId,
                        "LEADER" => leaderConfig?.RevenueKpiIndexId ?? salesConfig?.RevenueKpiIndexId,
                        _ => salesConfig?.RevenueKpiIndexId
                    };
                    var totalRevenue = GetTotalRevenue(empId, request.PeriodId, request.TemplateId, revenueKpiIndexId);

                    // LEADER: lấy TotalSales và TotalRevenue từ KPI cả team
                    var personalTotalSales = totalSales;
                    var personalTotalRevenue = totalRevenue;
                    if (positionType == "LEADER" && leaderTeamId.HasValue
                        && teamLevelResults.TryGetValue(leaderTeamId.Value, out var leaderTeamResults))
                    {
                        if (salesAmountKpiIndexId.HasValue && leaderTeamResults.TryGetValue(salesAmountKpiIndexId.Value, out var teamSales))
                            totalSales = teamSales;
                        if (revenueKpiIndexId.HasValue && leaderTeamResults.TryGetValue(revenueKpiIndexId.Value, out var teamRevenue))
                            totalRevenue = teamRevenue;
                    }

                    // NewAccountCount từ chỉ số KPI thực tế (dựa vào config NewAccountKpiIndexId)
                    var newAccountBonusAmount = positionType switch
                    {
                        "ADMIN" => adminConfig?.NewAccountBonusAmount ?? 0,
                        "PM" => pmConfig?.NewAccountBonusAmount ?? 0,
                        "LEADER" => leaderConfig?.NewAccountBonusAmount ?? salesConfig?.NewAccountBonusAmount ?? 500000,
                        _ => salesConfig?.NewAccountBonusAmount ?? 500000
                    };
                    var newAccountKpiIndexId = positionType switch
                    {
                        "ADMIN" => adminConfig?.NewAccountKpiIndexId,
                        "PM" => pmConfig?.NewAccountKpiIndexId,
                        "LEADER" => leaderConfig?.NewAccountKpiIndexId ?? salesConfig?.NewAccountKpiIndexId,
                        _ => salesConfig?.NewAccountKpiIndexId
                    };
                    var newAccountCount = GetNewAccountCount(empId, request.PeriodId, request.TemplateId, newAccountKpiIndexId);

                    // LEADER: lấy NewAccountCount từ KPI cả team
                    var personalNewAccountCount = newAccountCount;
                    if (positionType == "LEADER" && leaderTeamId.HasValue
                        && teamLevelResults.TryGetValue(leaderTeamId.Value, out var leaderNewAccResults))
                    {
                        if (newAccountKpiIndexId.HasValue && leaderNewAccResults.TryGetValue(newAccountKpiIndexId.Value, out var teamNewAcc))
                            newAccountCount = teamNewAcc;
                    }
                    var newAccountBonus = newAccountCount * newAccountBonusAmount;

                    if (positionType == "LEADER")
                    {
                        // Dòng 1: SALES_STAFF — data cá nhân (tính thưởng như sales thường)
                        empMetrics.Add((empId, employeeCode, employeeName,
                            teamCodeByEmp.GetValueOrDefault(empId) ?? mapping?.TeamCode ?? "",
                            "SALES_STAFF", personalAchievementPercent, personalCoefficient,
                            personalTotalSales, personalTotalRevenue,
                            personalNewAccountCount, personalNewAccountCount * newAccountBonusAmount));

                        // Dòng 2: LEADER — data cả team (không tính thưởng new account)
                        empMetrics.Add((empId, employeeCode, employeeName,
                            teamCodeByEmp.GetValueOrDefault(empId) ?? mapping?.TeamCode ?? "",
                            "LEADER", achievementPercent, coefficient,
                            totalSales, totalRevenue,
                            newAccountCount, 0));
                    }
                    else
                    {
                        empMetrics.Add((empId, employeeCode, employeeName,
                            teamCodeByEmp.GetValueOrDefault(empId) ?? mapping?.TeamCode ?? "",
                            positionType, achievementPercent, coefficient, totalSales, totalRevenue,
                            newAccountCount, newAccountBonus));
                    }
                }

                // 5.1 Tính tổng weighted revenue = Σ(Coef_i × TotalRevenue_i) và tổng hệ số theo từng team
                // LEADER không đóng góp vào tổng thưởng doanh số cả team
                var teamTotals = empMetrics
                    .Where(e => !string.IsNullOrEmpty(e.TeamCode) && e.PositionType != "LEADER")
                    .GroupBy(e => e.TeamCode)
                    .ToDictionary(
                        g => g.Key,
                        g => new
                        {
                            TotalWeightedRevenue = g.Sum(x => x.Coefficient * x.TotalRevenue),
                            TotalCoefficient = g.Sum(x => x.Coefficient)
                        });

                // 5.2 Tính thưởng DS cho từng người
                // SALES_STAFF: Coef_i × (Tổng thưởng DS cả team / Tổng hệ số cả team)
                //   Trong đó Tổng thưởng DS cả team = salesConfig.RewardRate × Σ(Coef_j × Revenue_j)
                //   → Nếu team có PM/ADMIN thì những vị trí đó vẫn dùng rewardRate riêng (pmConfig/adminConfig.RewardRate)
                // LEADER: leaderConfig.RewardRate × Coef_i × TotalRevenue (TotalRevenue = doanh số cả team)
                var rankingResults = new List<KPISaleRankingSummaryDto>();
                foreach (var m in empMetrics)
                {
                    decimal salesBonus = 0m;
                    decimal rewardRate = 0m;
                    int? rewardConfigId = null;

                    if (m.PositionType == "LEADER")
                    {
                        rewardRate = leaderConfig?.RewardRate ?? salesConfig?.RewardRate ?? defaultRewardRate;
                        rewardConfigId = leaderConfig?.Id ?? salesConfig?.Id;
                        salesBonus = rewardRate * m.Coefficient * m.TotalRevenue;
                    }
                    else if (m.PositionType == "ADMIN" || m.PositionType == "PM")
                    {
                        rewardRate = m.PositionType == "ADMIN"
                            ? (adminConfig?.RewardRate ?? 0.001m)
                            : (pmConfig?.RewardRate ?? 0.003m);
                        rewardConfigId = m.PositionType == "ADMIN" ? adminConfig?.Id : pmConfig?.Id;
                        salesBonus = rewardRate * m.Coefficient * m.TotalSales;
                    }
                    else
                    {
                        // Lấy rewardRate theo đúng EmployeeType của từng dòng
                        rewardRate = m.PositionType switch
                        {
                            "ADMIN" => adminConfig?.RewardRate ?? 0.001m,
                            "PM" => pmConfig?.RewardRate ?? 0.003m,
                            _ => salesConfig?.RewardRate ?? defaultRewardRate
                        };
                        rewardConfigId = m.PositionType switch
                        {
                            "ADMIN" => adminConfig?.Id,
                            "PM" => pmConfig?.Id,
                            _ => salesConfig?.Id
                        };

                        if (!string.IsNullOrEmpty(m.TeamCode) && teamTotals.TryGetValue(m.TeamCode, out var tt)
                            && tt.TotalWeightedRevenue > 0 && tt.TotalCoefficient > 0)
                        {
                            var totalTeamBonus = rewardRate * tt.TotalWeightedRevenue;
                            salesBonus = m.Coefficient * (totalTeamBonus / tt.TotalCoefficient);
                        }
                    }

                    rankingResults.Add(new KPISaleRankingSummaryDto
                    {
                        EmployeeId = m.EmpId,
                        EmployeeCode = m.EmpCode,
                        EmployeeName = m.EmpName,
                        TeamCode = m.TeamCode,
                        PositionType = m.PositionType,
                        TotalSalesAmount = m.TotalSales,
                        TotalRevenue = m.TotalRevenue,
                        AchievementPercent = m.Achievement,
                        Coefficient = m.Coefficient,
                        Rank = null,
                        SalesBonusAmount = salesBonus,
                        RankingBonusAmount = 0,
                        NewAccountCount = (int)m.NewAccountCount,
                        NewAccountBonus = m.NewAccountBonus,
                        OtherBonus = 0,
                        TotalBonus = salesBonus + m.NewAccountBonus
                    });
                }

                // 6. Ranking cho SALES_STAFF theo AchievementPercent giảm dần (LEADER không xếp hạng)
                var salesList = rankingResults
                    .Where(r => r.PositionType == "SALES_STAFF")
                    .OrderByDescending(r => r.AchievementPercent)
                    .ToList();

                int rank = 1;
                foreach (var item in salesList)
                {
                    item.Rank = rank;
                    if (rank == 1)
                    {
                        item.RankingBonusAmount = defaultRank1Bonus;
                        item.TotalBonus += item.RankingBonusAmount;
                    }
                    rank++;
                }

                // 7. Lưu vào DB (xóa dữ liệu cũ trước)
                var oldResults = await _kpiSaleRepo.KPISaleRankingResult
                    .Where(x => x.PeriodId == request.PeriodId && x.TemplateId == request.TemplateId)
                    .ToListAsync();
                if (oldResults.Count > 0)
                    _kpiSaleRepo.KPISaleRankingResult.RemoveRange(oldResults);

                foreach (var r in rankingResults)
                {
                    int? rewardConfigId = r.PositionType switch
                    {
                        "ADMIN" => adminConfig?.Id,
                        "PM" => pmConfig?.Id,
                        "LEADER" => leaderConfig?.Id ?? salesConfig?.Id,
                        _ => salesConfig?.Id
                    };

                    await _kpiSaleRepo.KPISaleRankingResult.AddAsync(new KPISaleRankingResult
                    {
                        EmployeeId = r.EmployeeId,
                        EmployeeCode = r.EmployeeCode,
                        EmployeeName = r.EmployeeName,
                        PeriodId = request.PeriodId,
                        TemplateId = request.TemplateId,
                        TeamCode = r.TeamCode,
                        PositionType = r.PositionType,
                        TotalSalesAmount = r.TotalSalesAmount,
                        TotalRevenue = r.TotalRevenue,
                        AchievementPercent = r.AchievementPercent,
                        Coefficient = r.Coefficient,
                        Ranking = r.Rank,
                        SalesBonusAmount = r.SalesBonusAmount,
                        RankingBonusAmount = r.RankingBonusAmount,
                        NewAccountCount = r.NewAccountCount,
                        NewAccountBonus = r.NewAccountBonus,
                        OtherBonus = r.OtherBonus,
                        TotalBonus = r.TotalBonus,
                        RewardConfigId = rewardConfigId,
                        IsCalculated = true,
                        CalculatedDate = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    });
                }

                await _kpiSaleRepo.SaveChangesAsync();

                return Ok(ApiResponseFactory.Success(rankingResults, "Tính ranking thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("ranking/config")]
        public async Task<IActionResult> GetRewardConfig()
        {
            try
            {
                // Tách 2 query rồi ghép ở memory để tránh EF không dịch được
                // khi entity có cột chưa được map column (vd RevenueKpiIndexId)
                var configsRaw = await _kpiSaleRepo.KPISaleRewardConfig.AsNoTracking()
                    .Where(c => c.IsActive == true)
                    .ToListAsync();

                var allIndexIds = configsRaw
                    .SelectMany(c => new int?[] { c.NewAccountKpiIndexId, c.SalesAmountKpiIndexId, c.RevenueKpiIndexId })
                    .Where(id => id.HasValue && id.Value > 0)
                    .Select(id => id!.Value)
                    .Distinct()
                    .ToList();

                var indexMap = await _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                    .Where(i => allIndexIds.Contains(i.ID))
                    .ToDictionaryAsync(i => i.ID, i => i);

                var configs = configsRaw.Select(c =>
                {
                    KPISaleIndex? na = c.NewAccountKpiIndexId.HasValue && indexMap.TryGetValue(c.NewAccountKpiIndexId.Value, out var n) ? n : null;
                    KPISaleIndex? sa = c.SalesAmountKpiIndexId.HasValue && indexMap.TryGetValue(c.SalesAmountKpiIndexId.Value, out var s) ? s : null;
                    KPISaleIndex? rv = c.RevenueKpiIndexId.HasValue && indexMap.TryGetValue(c.RevenueKpiIndexId.Value, out var r) ? r : null;

                    return new KPISaleRewardConfigDto
                    {
                        ID = c.Id,
                        ConfigCode = c.ConfigCode,
                        ConfigName = c.ConfigName,
                        EmployeeType = c.EmployeeType,
                        TemplateId = c.TemplateId,
                        RewardRate = c.RewardRate,
                        Rank1BonusAmount = c.Rank1BonusAmount,
                        NewAccountBonusAmount = c.NewAccountBonusAmount,
                        NewAccountKpiIndexId = c.NewAccountKpiIndexId,
                        NewAccountKpiIndexCode = na?.IndexCode,
                        NewAccountKpiIndexName = na?.IndexName,
                        SalesAmountKpiIndexId = c.SalesAmountKpiIndexId,
                        SalesAmountKpiIndexCode = sa?.IndexCode,
                        SalesAmountKpiIndexName = sa?.IndexName,
                        RevenueKpiIndexId = c.RevenueKpiIndexId,
                        RevenueKpiIndexCode = rv?.IndexCode,
                        RevenueKpiIndexName = rv?.IndexName,
                        IsActive = c.IsActive
                    };
                }).ToList();

                return Ok(ApiResponseFactory.Success(configs, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("ranking/config")]
        public async Task<IActionResult> SaveRewardConfig([FromBody] KPISaleRewardConfigDto request)
        {
            try
            {
                if (request.ID <= 0)
                {
                    await _kpiSaleRepo.KPISaleRewardConfig.AddAsync(new KPISaleRewardConfig
                    {
                        ConfigCode = request.ConfigCode,
                        ConfigName = request.ConfigName,
                        EmployeeType = request.EmployeeType,
                        TemplateId = request.TemplateId,
                        RewardRate = request.RewardRate,
                        Rank1BonusAmount = request.Rank1BonusAmount,
                        NewAccountBonusAmount = request.NewAccountBonusAmount,
                        NewAccountKpiIndexId = request.NewAccountKpiIndexId,
                        SalesAmountKpiIndexId = request.SalesAmountKpiIndexId,
                        RevenueKpiIndexId = request.RevenueKpiIndexId,
                        IsActive = request.IsActive,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    });
                }
                else
                {
                    var existing = await _kpiSaleRepo.KPISaleRewardConfig
                        .FirstOrDefaultAsync(c => c.Id == request.ID);
                    if (existing == null)
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy config"));

                    existing.ConfigCode = request.ConfigCode;
                    existing.ConfigName = request.ConfigName;
                    existing.EmployeeType = request.EmployeeType;
                    existing.TemplateId = request.TemplateId;
                    existing.RewardRate = request.RewardRate;
                    existing.Rank1BonusAmount = request.Rank1BonusAmount;
                    existing.NewAccountBonusAmount = request.NewAccountBonusAmount;
                    existing.NewAccountKpiIndexId = request.NewAccountKpiIndexId;
                    existing.SalesAmountKpiIndexId = request.SalesAmountKpiIndexId;
                    existing.RevenueKpiIndexId = request.RevenueKpiIndexId;
                    existing.IsActive = request.IsActive;
                    existing.ModifiedDate = DateTime.Now;
                }

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Lưu cấu hình thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ============== Reward Coefficient CRUD ==============

        [HttpGet("ranking/coefficients")]
        public async Task<IActionResult> GetRewardCoefficients(int? configId = null, string? employeeType = null)
        {
            try
            {
                var query = _kpiSaleRepo.KPISaleRewardCoefficient.AsNoTracking().AsQueryable();

                if (configId.HasValue)
                    query = query.Where(c => c.ConfigId == configId.Value);

                if (!string.IsNullOrWhiteSpace(employeeType))
                    query = query.Where(c => c.EmployeeType == employeeType);

                var result = await query
                    .OrderBy(c => c.Priority)
                    .ThenBy(c => c.MinPerformance)
                    .Select(c => new KPISaleRewardCoefficientDto
                    {
                        ID = c.Id,
                        ConfigId = c.ConfigId,
                        EmployeeType = c.EmployeeType,
                        MinPerformance = c.MinPerformance,
                        MaxPerformance = c.MaxPerformance,
                        Coefficient = c.Coefficient,
                        Priority = c.Priority,
                        IsActive = c.IsActive
                    })
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("ranking/coefficients")]
        public async Task<IActionResult> SaveRewardCoefficient([FromBody] KPISaleRewardCoefficientDto request)
        {
            try
            {
                if (request.ID <= 0)
                {
                    await _kpiSaleRepo.KPISaleRewardCoefficient.AddAsync(new KPISaleRewardCoefficient
                    {
                        ConfigId = request.ConfigId,
                        EmployeeType = request.EmployeeType,
                        MinPerformance = request.MinPerformance,
                        MaxPerformance = request.MaxPerformance,
                        Coefficient = request.Coefficient,
                        Priority = request.Priority,
                        IsActive = request.IsActive ?? true
                    });
                }
                else
                {
                    var existing = await _kpiSaleRepo.KPISaleRewardCoefficient
                        .FirstOrDefaultAsync(c => c.Id == request.ID);
                    if (existing == null)
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy hệ số"));

                    existing.ConfigId = request.ConfigId;
                    existing.EmployeeType = request.EmployeeType;
                    existing.MinPerformance = request.MinPerformance;
                    existing.MaxPerformance = request.MaxPerformance;
                    existing.Coefficient = request.Coefficient;
                    existing.Priority = request.Priority;
                    existing.IsActive = request.IsActive ?? existing.IsActive;
                }

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Lưu hệ số thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("ranking/coefficients")]
        public async Task<IActionResult> SaveRewardCoefficients([FromBody] List<KPISaleRewardCoefficientDto> request)
        {
            try
            {
                if (request == null || request.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách hệ số trống"));

                // Lấy danh sách ID hiện tại của (configId, employeeType) này
                var firstItem = request.First();
                if (!firstItem.ConfigId.HasValue)
                    return BadRequest(ApiResponseFactory.Fail(null, "Thiếu ConfigId"));

                var configId = firstItem.ConfigId.Value;
                var employeeType = firstItem.EmployeeType;

                var existing = await _kpiSaleRepo.KPISaleRewardCoefficient
                    .Where(c => c.ConfigId == configId
                        && (employeeType == null || c.EmployeeType == employeeType))
                    .ToListAsync();

                var requestIds = request.Where(x => x.ID > 0).Select(x => x.ID).ToHashSet();
                var toDelete = existing.Where(e => !requestIds.Contains(e.Id)).ToList();
                if (toDelete.Count > 0)
                    _kpiSaleRepo.KPISaleRewardCoefficient.RemoveRange(toDelete);

                foreach (var dto in request)
                {
                    if (dto.ID > 0)
                    {
                        var entity = existing.FirstOrDefault(e => e.Id == dto.ID);
                        if (entity == null) continue;
                        entity.ConfigId = dto.ConfigId;
                        entity.EmployeeType = dto.EmployeeType;
                        entity.MinPerformance = dto.MinPerformance;
                        entity.MaxPerformance = dto.MaxPerformance;
                        entity.Coefficient = dto.Coefficient;
                        entity.Priority = dto.Priority;
                        entity.IsActive = dto.IsActive ?? entity.IsActive;
                    }
                    else
                    {
                        await _kpiSaleRepo.KPISaleRewardCoefficient.AddAsync(new KPISaleRewardCoefficient
                        {
                            ConfigId = dto.ConfigId,
                            EmployeeType = dto.EmployeeType,
                            MinPerformance = dto.MinPerformance,
                            MaxPerformance = dto.MaxPerformance,
                            Coefficient = dto.Coefficient,
                            Priority = dto.Priority,
                            IsActive = dto.IsActive ?? true
                        });
                    }
                }

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Lưu bậc thang hệ số thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("ranking/coefficients/{id}")]
        public async Task<IActionResult> DeleteRewardCoefficient(int id)
        {
            try
            {
                var existing = await _kpiSaleRepo.KPISaleRewardCoefficient
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (existing == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy hệ số"));

                _kpiSaleRepo.KPISaleRewardCoefficient.Remove(existing);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Xóa hệ số thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ============== Employee Reward Mapping CRUD ==============

        [HttpGet("ranking/mappings")]
        public async Task<IActionResult> GetRewardMappings(int? configId = null, int? employeeId = null, bool? isActive = null)
        {
            try
            {
                var query = _kpiSaleRepo.KPISaleEmployeeRewardMapping.AsNoTracking().AsQueryable();

                if (configId.HasValue)
                    query = query.Where(m => m.RewardConfigId == configId.Value);

                if (employeeId.HasValue)
                    query = query.Where(m => m.EmployeeId == employeeId.Value);

                if (isActive.HasValue)
                    query = query.Where(m => m.IsActive == isActive.Value);

                var result = await query
                    .OrderBy(m => m.EmployeeId)
                    .Select(m => new KPISaleEmployeeRewardMappingDto
                    {
                        ID = m.Id,
                        EmployeeId = m.EmployeeId,
                        RewardConfigId = m.RewardConfigId,
                        PositionType = m.PositionType,
                        TeamCode = m.TeamCode,
                        ProjectIds = m.ProjectIds,
                        IsActive = m.IsActive,
                        EffectiveFromDate = m.EffectiveFromDate,
                        EffectiveToDate = m.EffectiveToDate
                    })
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("ranking/mappings")]
        public async Task<IActionResult> SaveRewardMapping([FromBody] KPISaleEmployeeRewardMappingDto request)
        {
            try
            {
                if (request.ID <= 0)
                {
                    await _kpiSaleRepo.KPISaleEmployeeRewardMapping.AddAsync(new KPISaleEmployeeRewardMapping
                    {
                        EmployeeId = request.EmployeeId,
                        RewardConfigId = request.RewardConfigId,
                        PositionType = request.PositionType,
                        TeamCode = request.TeamCode,
                        ProjectIds = request.ProjectIds,
                        IsActive = request.IsActive ?? true,
                        EffectiveFromDate = request.EffectiveFromDate,
                        EffectiveToDate = request.EffectiveToDate
                    });
                }
                else
                {
                    var existing = await _kpiSaleRepo.KPISaleEmployeeRewardMapping
                        .FirstOrDefaultAsync(m => m.Id == request.ID);
                    if (existing == null)
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy mapping"));

                    existing.EmployeeId = request.EmployeeId;
                    existing.RewardConfigId = request.RewardConfigId;
                    existing.PositionType = request.PositionType;
                    existing.TeamCode = request.TeamCode;
                    existing.ProjectIds = request.ProjectIds;
                    existing.IsActive = request.IsActive ?? existing.IsActive;
                    existing.EffectiveFromDate = request.EffectiveFromDate;
                    existing.EffectiveToDate = request.EffectiveToDate;
                }

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Lưu mapping thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPut("ranking/mappings/{id}")]
        public async Task<IActionResult> UpdateRewardMapping(int id, [FromBody] KPISaleEmployeeRewardMappingDto request)
        {
            try
            {
                var existing = await _kpiSaleRepo.KPISaleEmployeeRewardMapping
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (existing == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy mapping"));

                existing.EmployeeId = request.EmployeeId;
                existing.RewardConfigId = request.RewardConfigId;
                existing.PositionType = request.PositionType;
                existing.TeamCode = request.TeamCode;
                existing.ProjectIds = request.ProjectIds;
                existing.IsActive = request.IsActive ?? existing.IsActive;
                existing.EffectiveFromDate = request.EffectiveFromDate;
                existing.EffectiveToDate = request.EffectiveToDate;

                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Cập nhật mapping thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("ranking/mappings/{id}")]
        public async Task<IActionResult> DeleteRewardMapping(int id)
        {
            try
            {
                var existing = await _kpiSaleRepo.KPISaleEmployeeRewardMapping
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (existing == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy mapping"));

                _kpiSaleRepo.KPISaleEmployeeRewardMapping.Remove(existing);
                await _kpiSaleRepo.SaveChangesAsync();
                return Ok(ApiResponseFactory.Success(null, "Xóa mapping thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private decimal GetTotalSalesAmount(int employeeId, int periodId, int templateId, int? salesAmountKpiIndexId = null)
        {
            // Lấy chỉ số doanh số từ KPISaleResult
            var salesResults = _kpiSaleRepo.KPISaleResults.AsNoTracking()
                .Where(r => r.EmployeeID == employeeId && r.PeriodID == periodId && r.TeamID == null)
                .Join(_kpiSaleRepo.KPISaleIndices.AsNoTracking(),
                    r => r.KpiIndexID,
                    i => i.ID,
                    (r, i) => new { r.ResultValue, i.IndexCode, i.IndexName, i.UnitType })
                .ToList();

            // Nếu có config cụ thể → dùng KPI Index đó
            if (salesAmountKpiIndexId.HasValue)
            {
                var specific = salesResults.FirstOrDefault(x =>
                    _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                        .Where(i => i.ID == salesAmountKpiIndexId.Value)
                        .Select(i => i.IndexCode)
                        .Contains(x.IndexCode));
                return specific?.ResultValue ?? 0m;
            }

            // Không config → không tính
            return 0m;
        }

        private decimal GetTotalRevenue(int employeeId, int periodId, int templateId, int? revenueKpiIndexId)
        {
            // Tương tự GetTotalSalesAmount nhưng dùng cho cột "Doanh số" (khác với Tiền về)
            if (!revenueKpiIndexId.HasValue) return 0m;

            return _kpiSaleRepo.KPISaleResults.AsNoTracking()
                .Where(r => r.EmployeeID == employeeId && r.PeriodID == periodId && r.TeamID == null
                    && r.KpiIndexID == revenueKpiIndexId.Value)
                .Select(r => r.ResultValue)
                .FirstOrDefault();
        }

        private decimal GetNewAccountCount(int employeeId, int periodId, int templateId, int? newAccountKpiIndexId)
        {
            // Chỉ tính new account khi có chọn NewAccountKpiIndexId trong config
            if (!newAccountKpiIndexId.HasValue) return 0m;

            return _kpiSaleRepo.KPISaleResults.AsNoTracking()
                .Where(r => r.EmployeeID == employeeId && r.PeriodID == periodId && r.TeamID == null
                    && r.KpiIndexID == newAccountKpiIndexId.Value)
                .Select(r => r.ResultValue)
                .FirstOrDefault();
        }

        private static decimal CalculateCoefficient(decimal achievementPercent, string positionType, List<KPISaleRewardCoefficient> coefficients)
        {
            var employeeType = positionType switch
            {
                "SALES_STAFF" => "SALES",
                "LEADER" => "SALES_LEADER",
                "ADMIN" => "ADMIN",
                "PM" => "PM",
                _ => "SALES"
            };

            var applicableCoefficients = coefficients
                .Where(c => c.EmployeeType == employeeType && c.MinPerformance.HasValue)
                .OrderByDescending(c => c.MinPerformance)
                .ToList();

            foreach (var coef in applicableCoefficients)
            {
                var min = coef.MinPerformance.GetValueOrDefault();
                var max = coef.MaxPerformance;
                if (achievementPercent >= min && (!max.HasValue || achievementPercent < max.Value))
                    return coef.Coefficient.GetValueOrDefault();
            }

            return 0;
        }

        #endregion Reward & Ranking

        #region KPI Summary (quarterly with monthly breakdown)

        [HttpGet("summary")]
        public async Task<IActionResult> GetKpiSummary(int employeeId, int quarterPeriodId, int templateId)
        {
            try
            {
                // 1. Validate & load quarter period
                var quarterPeriod = await _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == quarterPeriodId);
                if (quarterPeriod == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy kỳ quý"));

                var periodType = NormalizeOptionalCode(quarterPeriod.PeriodType, "MONTH");
                if (periodType != "QUARTER" && periodType != "YEAR")
                    return BadRequest(ApiResponseFactory.Fail(null, "Chỉ hỗ trợ kỳ QUÝ hoặc NĂM"));

                // 2. Load child month periods
                var childMonthPeriods = await GetChildMonthPeriodsAsync(quarterPeriod);
                if (childMonthPeriods.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy các tháng con"));

                // 3. Load template indexes
                var indexes = await _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                    .Where(x => x.TemplateID == templateId && x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToListAsync();

                if (indexes.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Template không có index nào"));

                var indexIds = indexes.Select(x => x.ID).ToList();

                // 4. Load all targets for employee across all relevant periods
                var allPeriodIds = childMonthPeriods.Select(x => x.ID)
                    .Concat(new[] { quarterPeriodId })
                    .ToList();

                var targets = await _kpiSaleRepo.KPISaleTargets.AsNoTracking()
                    .Where(x => x.EmployeeID == employeeId
                        && allPeriodIds.Contains(x.PeriodID)
                        && indexIds.Contains(x.KpiIndexID))
                    .ToListAsync();

                var targetsByPeriodIndex = targets
                    .ToDictionary(x => (x.PeriodID, x.KpiIndexID), x => x.GoalValue);

                // 5. Load all results for employee across all relevant periods
                var results = await _kpiSaleRepo.KPISaleResults.AsNoTracking()
                    .Where(x => x.EmployeeID == employeeId
                        && allPeriodIds.Contains(x.PeriodID)
                        && indexIds.Contains(x.KpiIndexID))
                    .ToListAsync();

                var resultsByPeriodIndex = results
                    .ToDictionary(x => (x.PeriodID, x.KpiIndexID), x => x);

                // 6. Calculate quarterly results for each index (SUM_MONTH)
                var quarterResultByIndex = new Dictionary<int, KpiSummaryValueDto>();
                foreach (var index in indexes)
                {
                    var quarterResultType = NormalizeOptionalCode(index.QuarterResultCalculateType, "SUM_MONTH");
                    decimal sumResult = 0;
                    decimal sumScore = 0;
                    decimal sumGoal = 0;

                    foreach (var month in childMonthPeriods)
                    {
                        if (resultsByPeriodIndex.TryGetValue((month.ID, index.ID), out var r))
                        {
                            sumResult += r.ResultValue;
                            sumScore += r.FinalScore;
                        }
                        if (targetsByPeriodIndex.TryGetValue((month.ID, index.ID), out var g))
                            sumGoal += g;
                    }

                    quarterResultByIndex[index.ID] = new KpiSummaryValueDto
                    {
                        Goal = sumGoal,
                        Result = sumResult,
                        Score = sumScore,
                        AchievedPercent = sumGoal > 0 ? Math.Round(sumResult / sumGoal * 100, 2) : 0
                    };
                }

                // 7. Build monthly + quarterly values for each index
                var summaryRows = new List<KpiSummaryRowDto>();
                var warnings = new List<string>();

                // Tạo dictionary children để tra cứu nhanh
                var childrenByParent = indexes
                    .Where(x => x.ParentID.HasValue)
                    .GroupBy(x => x.ParentID!.Value)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var index in indexes)
                {
                    var monthlyValues = new List<KpiSummaryValueDto>();
                    foreach (var month in childMonthPeriods)
                    {
                        var hasResult = resultsByPeriodIndex.ContainsKey((month.ID, index.ID));
                        if (!hasResult)
                            warnings.Add($"Chưa tính KPI cho {month.PeriodCode} - {index.IndexName}");

                        decimal goal = targetsByPeriodIndex.GetValueOrDefault((month.ID, index.ID));
                        
                        // Nếu goal = 0 và là GROUP, sum từ các con
                        if (goal == 0)
                        {
                            var indexType = NormalizeOptionalCode(index.IndexType, "DETAIL");
                            if (indexType == "GROUP" || indexType == "FORMULA")
                            {
                                goal = SumGoalFromChildrenForPeriod(index.ID, month.ID, targetsByPeriodIndex, childrenByParent, indexes);
                            }
                        }
                        
                        resultsByPeriodIndex.TryGetValue((month.ID, index.ID), out var r);

                        monthlyValues.Add(new KpiSummaryValueDto
                        {
                            Goal = goal,
                            Result = r?.ResultValue ?? 0,
                            Score = r?.FinalScore ?? 0,
                            AchievedPercent = r?.AchievedPercent ?? 0
                        });
                    }

                    quarterResultByIndex.TryGetValue(index.ID, out var qVal);
                    var quarterGoal = targetsByPeriodIndex.GetValueOrDefault((quarterPeriodId, index.ID));
                    
                    // Nếu quarterGoal = 0 và là GROUP, sum từ các con
                    if (quarterGoal == 0)
                    {
                        var indexType = NormalizeOptionalCode(index.IndexType, "DETAIL");
                        if (indexType == "GROUP" || indexType == "FORMULA")
                        {
                            quarterGoal = SumGoalFromChildrenForPeriod(index.ID, quarterPeriodId, targetsByPeriodIndex, childrenByParent, indexes);
                        }
                    }
                    
                    if (quarterGoal > 0)
                        qVal.Goal = quarterGoal;

                    summaryRows.Add(new KpiSummaryRowDto
                    {
                        IndexID = index.ID,
                        ParentID = index.ParentID,
                        IndexCode = index.IndexCode,
                        IndexName = index.IndexName,
                        IndexType = index.IndexType,
                        WeightPercent = index.WeightPercent,
                        IsBold = index.IsBold,
                        SortOrder = index.SortOrder,
                        Depth = 0,
                        HasChildren = indexes.Any(x => x.ParentID == index.ID),
                        MonthlyValues = monthlyValues,
                        QuarterValue = qVal ?? new KpiSummaryValueDto(),
                        ReportScoreAdjustmentType = resultsByPeriodIndex.Values
                            .FirstOrDefault(r => r.KpiIndexID == index.ID)?.ReportScoreAdjustmentType ?? 0,
                        ReportScoreValue = resultsByPeriodIndex.Values
                            .FirstOrDefault(r => r.KpiIndexID == index.ID)?.ReportScoreValue ?? 0
                    });
                }

                // 8. Compute depth for each row
                ComputeRowDepths(summaryRows);

                // 9. Build performance summary
                decimal m1Score = 0, m2Score = 0, m3Score = 0;
                if (childMonthPeriods.Count >= 1)
                {
                    m1Score = summaryRows
                        .Where(x => resultsByPeriodIndex.ContainsKey((childMonthPeriods[0].ID, x.IndexID)))
                        .Sum(x => x.MonthlyValues.FirstOrDefault()?.Score ?? 0);
                }
                if (childMonthPeriods.Count >= 2)
                    m2Score = summaryRows
                        .Where(x => resultsByPeriodIndex.ContainsKey((childMonthPeriods[1].ID, x.IndexID)))
                        .Sum(x => x.MonthlyValues.Skip(1).FirstOrDefault()?.Score ?? 0);
                if (childMonthPeriods.Count >= 3)
                    m3Score = summaryRows
                        .Where(x => resultsByPeriodIndex.ContainsKey((childMonthPeriods[2].ID, x.IndexID)))
                        .Sum(x => x.MonthlyValues.Skip(2).FirstOrDefault()?.Score ?? 0);

                var quarterTotalPerf = await _kpiSaleRepo.KPISaleTotalPerformances.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.EmployeeID == employeeId
                        && x.PeriodID == quarterPeriodId
                        && x.TemplateID == templateId);

                var response = new KpiSummaryResponse
                {
                    QuarterPeriodID = quarterPeriodId,
                    QuarterCode = quarterPeriod.PeriodCode,
                    QuarterName = quarterPeriod.PeriodName,
                    Periods = childMonthPeriods.Select((p, i) => new PeriodInfoDto
                    {
                        PeriodID = p.ID,
                        PeriodCode = p.PeriodCode,
                        PeriodName = p.PeriodName,
                        PeriodType = p.PeriodType,
                        SortOrder = i
                    }).ToList(),
                    Items = summaryRows,
                    Summary = new KpiSummaryPerformanceDto
                    {
                        Month1Score = Math.Round(m1Score, 2),
                        Month2Score = Math.Round(m2Score, 2),
                        Month3Score = Math.Round(m3Score, 2),
                        QuarterScore = quarterTotalPerf?.FinalScore ?? summaryRows.Sum(x => x.QuarterValue.Score)
                    },
                    Warnings = warnings.Distinct().ToList()
                };

                return Ok(ApiResponseFactory.Success(response, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Tính tổng goal từ các chỉ tiêu con cho một period cụ thể (dùng trong GetKpiSummary).
        /// </summary>
        private decimal SumGoalFromChildrenForPeriod(
            int parentIndexId,
            int periodId,
            Dictionary<(int, int), decimal> targetsByPeriodIndex,
            Dictionary<int, List<KPISaleIndex>> childrenByParent,
            List<KPISaleIndex> allIndexes)
        {
            if (!childrenByParent.TryGetValue(parentIndexId, out var children) || children.Count == 0)
                return 0;

            decimal sum = 0;
            foreach (var child in children.OrderBy(x => x.SortOrder).ThenBy(x => x.ID))
            {
                if (targetsByPeriodIndex.TryGetValue((periodId, child.ID), out var childGoal))
                {
                    sum += childGoal;
                }
                else
                {
                    var childIndexType = NormalizeOptionalCode(child.IndexType, "DETAIL");
                    if (childIndexType == "GROUP" || childIndexType == "FORMULA")
                    {
                        sum += SumGoalFromChildrenForPeriod(child.ID, periodId, targetsByPeriodIndex, childrenByParent, allIndexes);
                    }
                }
            }
            return sum;
        }

        private void ComputeRowDepths(List<KpiSummaryRowDto> rows)
        {
            var indexById = rows.ToDictionary(x => x.IndexID);
            foreach (var row in rows)
            {
                row.Depth = ComputeDepth(row.ParentID, indexById);
            }
        }

        private int ComputeDepth(int? parentId, Dictionary<int, KpiSummaryRowDto> indexById)
        {
            if (!parentId.HasValue || !indexById.ContainsKey(parentId.Value))
                return 0;
            return 1 + ComputeDepth(indexById[parentId.Value].ParentID, indexById);
        }

        #endregion KPI Summary

        #region Calculator

        private async Task<KPISaleCalculateResponse> CalculateInternalAsync(KPISaleCalculateRequest request)
        {
            var period = await _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == request.PeriodID);
            if (period == null || period.ID <= 0)
                throw new Exception("Không tìm thấy kỳ KPI");

            var template = await _kpiSaleRepo.KPISaleTemplates.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == request.TemplateID && x.IsActive);
            if (template == null || template.ID <= 0)
                throw new Exception("Không tìm thấy template KPI đang hoạt động");

            var indexes = await _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                .Where(x => x.TemplateID == request.TemplateID && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .ToListAsync();
            if (indexes.Count == 0)
                throw new Exception("Template chưa có KPI index");

            var indexIds = indexes.Select(x => x.ID).ToList();

            var existingResults = await _kpiSaleRepo.KPISaleResults.AsNoTracking()
                .Where(x => x.EmployeeID == request.EmployeeID
                    && x.PeriodID == request.PeriodID
                    && indexIds.Contains(x.KpiIndexID))
                .ToListAsync();

            var reportAdjustmentsByIndex = existingResults
                .ToDictionary(
                    x => x.KpiIndexID,
                    x => new KPISaleReportAdjustmentInputDto
                    {
                        KpiIndexID = x.KpiIndexID,
                        ReportScoreAdjustmentType = x.ReportScoreAdjustmentType ?? 0,
                        ReportScoreValue = x.ReportScoreValue ?? 0
                    });

            foreach (var adjustment in request.ReportAdjustments ?? new List<KPISaleReportAdjustmentInputDto>())
            {
                if (adjustment == null || adjustment.KpiIndexID <= 0)
                    continue;

                reportAdjustmentsByIndex[adjustment.KpiIndexID] = new KPISaleReportAdjustmentInputDto
                {
                    KpiIndexID = adjustment.KpiIndexID,
                    ReportScoreAdjustmentType = adjustment.ReportScoreAdjustmentType ?? 0,
                    ReportScoreValue = adjustment.ReportScoreValue ?? 0
                };
            }

            // Tính trung bình cộng reportScoreValue từ các tháng con cho kỳ QUARTER/YEAR
            var periodType = NormalizeOptionalCode(period.PeriodType, "MONTH");
            if (periodType == "QUARTER" || periodType == "YEAR")
            {
                var childMonthPeriods = await GetChildMonthPeriodsAsync(period);
                if (childMonthPeriods.Count > 0)
                {
                    var childPeriodIds = childMonthPeriods.Select(x => x.ID).ToList();
                    var totalMonthCount = childMonthPeriods.Count; // Số tháng cố định của quý (3)

                    var childResults = await _kpiSaleRepo.KPISaleResults.AsNoTracking()
                        .Where(x => x.EmployeeID == request.EmployeeID
                            && childPeriodIds.Contains(x.PeriodID)
                            && indexIds.Contains(x.KpiIndexID)
                            && x.KpiIndexID > 0)
                        .ToListAsync();

                    var reportIndexes = indexes.Where(x => NormalizeOptionalCode(x.IndexType, "DETAIL") == "REPORT").Select(x => x.ID).ToList();
                    
                    // Group theo index, tính tổng và lấy adjustment type từ tháng đầu tiên có dữ liệu
                    var monthSumByIndex = childResults
                        .Where(x => reportIndexes.Contains(x.KpiIndexID))
                        .GroupBy(x => x.KpiIndexID)
                        .ToDictionary(
                            g => g.Key,
                            g => new { 
                                Sum = g.Sum(x => x.ReportScoreValue ?? 0),
                                AdjustmentType = g.OrderByDescending(x => x.PeriodID).FirstOrDefault()?.ReportScoreAdjustmentType ?? 0 // Lấy adjustment type từ tháng mới nhất
                            });

                    // Với mỗi report index, chia tổng cho số tháng cố định (coi tháng không có dữ liệu = 0)
                    foreach (var indexId in reportIndexes)
                    {
                        if (monthSumByIndex.TryGetValue(indexId, out var data))
                        {
                            reportAdjustmentsByIndex[indexId] = new KPISaleReportAdjustmentInputDto
                            {
                                KpiIndexID = indexId,
                                ReportScoreAdjustmentType = data.AdjustmentType, // Giữ nguyên adjustment type
                                ReportScoreValue = data.Sum / totalMonthCount // Chia cho số tháng cố định (coi tháng không có dữ liệu = 0)
                            };
                        }
                    }
                }
            }

            var mappings = await _kpiSaleRepo.KPISaleIndexDataMappings.AsNoTracking()
                .Where(x => indexIds.Contains(x.KpiIndexID) && x.IsActive)
                .ToListAsync();

            var formulaItems = await _kpiSaleRepo.KPISaleIndexFormulaItems.AsNoTracking()
                .Where(x => indexIds.Contains(x.ParentKpiIndexID))
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .ToListAsync();

            var targetPeriodIds = new List<int> { request.PeriodID };
            if (periodType == "QUARTER" || periodType == "YEAR")
            {
                var childMonthPeriods = await GetChildMonthPeriodsAsync(period);
                targetPeriodIds.AddRange(childMonthPeriods.Select(x => x.ID));
            }

            var targets = await _kpiSaleRepo.KPISaleTargets.AsNoTracking()
                .Where(x => x.EmployeeID == request.EmployeeID
                    && targetPeriodIds.Contains(x.PeriodID)
                    && indexIds.Contains(x.KpiIndexID))
                .ToListAsync();

            var scoringRules = await _kpiSaleRepo.KPISaleScoringRules.AsNoTracking()
                .Where(x => indexIds.Contains(x.KpiIndexID))
                .ToListAsync();

            var runtime = new KPISaleRuntimeContext
            {
                Request = request,
                Period = period,
                Indexes = indexes,
                IndexById = indexes.ToDictionary(x => x.ID),
                MappingsByIndex = mappings
                    .GroupBy(x => x.KpiIndexID)
                    .ToDictionary(x => x.Key, x => x.ToList()),
                FormulaItemsByParent = formulaItems
                    .GroupBy(x => x.ParentKpiIndexID)
                    .ToDictionary(x => x.Key, x => x.ToList()),
                ChildrenByParent = indexes
                    .Where(x => x.ParentID.HasValue)
                    .GroupBy(x => x.ParentID!.Value)
                    .ToDictionary(x => x.Key, x => x.ToList()),
                TargetsByIndex = targets.ToDictionary(x => (x.PeriodID, x.KpiIndexID), x => x.GoalValue),
                WeightsByIndex = targets
                    .Where(x => x.WeightPercent.HasValue)
                    .ToDictionary(x => (x.PeriodID, x.KpiIndexID), x => x.WeightPercent!.Value),
                ScoringRulesByIndex = scoringRules
                    .GroupBy(x => x.KpiIndexID)
                    .ToDictionary(x => x.Key, x => x.OrderByDescending(r => r.ID).First()),
                ReportAdjustmentsByIndex = reportAdjustmentsByIndex
            };

            foreach (var index in indexes)
                await ResolveIndexResultAsync(index, runtime);

            // Lọc bỏ DETAIL/GROUP/FORMULA có weightPercent = 0 hoặc chưa khai báo
            var noWeightIndexIds = indexes
                .Where(x => NormalizeOptionalCode(x.IndexType, "DETAIL") != "REPORT")
                .Select(x => x.ID)
                .ToHashSet();

            var noWeightTargets = targets
                .Where(t => noWeightIndexIds.Contains(t.KpiIndexID)
                    && (!t.WeightPercent.HasValue || t.WeightPercent.Value == 0))
                .Select(t => t.KpiIndexID)
                .ToHashSet();

            var results = new List<KPISaleCalculateResult>();
            foreach (var index in indexes)
            {
                var indexType = NormalizeOptionalCode(index.IndexType, "DETAIL");

                // Bỏ qua DETAIL không có weight hợp lệ
                if (indexType == "DETAIL" && noWeightIndexIds.Contains(index.ID) && noWeightTargets.Contains(index.ID))
                    continue;

                // GROUP/FORMULA: không bỏ qua chỉ vì không có target trực tiếp.
                // Nếu không có direct target, SumGoalFromChildren/SumGoalFromChildMonths đã được gọi
                // ở trên (dòng 4708-4718) để compute goal từ các con.
                // Chỉ bỏ qua nếu goalValue vẫn = 0 sau khi đã thử sum từ con.

                var resultValue = runtime.CalculatedValues.GetValueOrDefault(index.ID);
                var goalValue = runtime.TargetsByIndex.GetValueOrDefault((period.ID, index.ID));

                // GROUP/FORMULA ở QUÝ/NĂM: LUÔN compute goal từ tháng con,
                // bất kể direct target có tồn tại hay không (tránh bị fix cứng GoalValue=0).
                // DIRECTION: QUÝ/NĂM → tháng con
                var periodTypeNorm = NormalizeOptionalCode(runtime.Period.PeriodType, "MONTH");
                if ((indexType == "GROUP" || indexType == "FORMULA")
                    && (periodTypeNorm == "QUARTER" || periodTypeNorm == "YEAR"))
                {
                    goalValue = await SumGoalFromChildrenAsync(index, runtime);
                }
                else if (goalValue == 0)
                {
                    // THÁNG hoặc DETAIL: chỉ fallback khi direct goal = 0
                    if (indexType == "GROUP" || indexType == "FORMULA")
                    {
                        goalValue = await SumGoalFromChildrenAsync(index, runtime);
                    }

                    // Nếu vẫn = 0 và là kỳ QUÝ/NĂM, thử sum từ các tháng con
                    if (goalValue == 0)
                        goalValue = await SumGoalFromChildMonthsAsync(index, runtime);
                }

                runtime.ScoringRulesByIndex.TryGetValue(index.ID, out var scoringRule);
                var scoreType = NormalizeOptionalCode(scoringRule?.ScoreType, "NORMAL_PERCENT");
                var achievedPercent = CalculateAchievedPercent(goalValue, resultValue, scoreType);
                var weightPercent = runtime.WeightsByIndex.GetValueOrDefault((period.ID, index.ID), 0m);
                var finalScore = CalculateFinalScore(achievedPercent, weightPercent, scoringRule, scoreType);
                reportAdjustmentsByIndex.TryGetValue(index.ID, out var reportAdjustment);
                var reportScoreValue = reportAdjustment?.ReportScoreValue ?? 0;
                var reportScoreAdjustmentType = reportAdjustment?.ReportScoreAdjustmentType ?? 0;

                if (indexType == "REPORT")
                {
                    // REPORT index: chỉ lưu điểm điều chỉnh, không lưu ResultValue/AchievedPercent
                    // FinalScore vẫn tính theo điểm điều chỉnh (cộng thêm hoặc trừ đi)
                    resultValue = 0;
                    achievedPercent = 0;
                    finalScore = reportScoreAdjustmentType == 2
                        ? reportScoreValue
                        : reportScoreAdjustmentType == 1
                            ? -reportScoreValue
                            : 0;
                }

                results.Add(new KPISaleCalculateResult
                {
                    KpiIndexID = index.ID,
                    ParentID = index.ParentID,
                    EmployeeID = request.EmployeeID,
                    PeriodID = request.PeriodID,
                    PeriodCode = period.PeriodCode,
                    CalculatedDate = DateTime.Now,
                    IndexCode = index.IndexCode,
                    IndexName = index.IndexName,
                    IndexType = index.IndexType,
                    GoalValue = goalValue,
                    ResultValue = resultValue,
                    AchievedPercent = achievedPercent,
                    WeightPercent = weightPercent,
                    FinalScore = finalScore,
                    UnitType = index.UnitType,
                    ReportScoreAdjustmentType = reportScoreAdjustmentType,
                    ReportScoreValue = reportScoreValue,
                    SortOrder = index.SortOrder,
                    IsMainIndex = index.IsMainIndex,
                    IsBold = index.IsBold
                });
            }

            var totalFinalScore = results.Sum(x => x.FinalScore);
            var totalPerformance = new KPISaleTotalPerformanceDto
            {
                EmployeeID = request.EmployeeID,
                PeriodID = request.PeriodID,
                TemplateID = request.TemplateID,
                FinalScore = totalFinalScore,
                CalculatedDate = DateTime.Now
            };

            if (request.SaveSnapshot)
                await SaveSnapshotAsync(request, results, totalPerformance);

            return new KPISaleCalculateResponse
            {
                Items = results,
                TotalPerformance = totalPerformance
            };
        }

        private async Task<decimal> ResolveIndexResultAsync(KPISaleIndex index, KPISaleRuntimeContext runtime)
        {
            if (runtime.CalculatedValues.TryGetValue(index.ID, out var value))
                return value;

            if (!runtime.Visiting.Add(index.ID))
                throw new Exception($"Cấu hình KPI formula bị vòng lặp tại index {index.IndexCode}");

            var indexType = NormalizeOptionalCode(index.IndexType, "DETAIL");
            decimal result;

            if (indexType == "DETAIL")
            {
                result = await CalculateDetailIndexAsync(index, runtime);
            }
            else                 if (indexType == "REPORT")
                {
                    runtime.ReportAdjustmentsByIndex.TryGetValue(index.ID, out var reportAdjustment);
                    result = reportAdjustment?.ReportScoreValue ?? 0;
                }
            else
            {
                result = await CalculateFormulaIndexAsync(index, runtime);
            }

            runtime.Visiting.Remove(index.ID);
            runtime.CalculatedValues[index.ID] = result;
            return result;
        }

        private async Task<decimal> CalculateDetailIndexAsync(KPISaleIndex index, KPISaleRuntimeContext runtime)
        {
            if (!runtime.MappingsByIndex.TryGetValue(index.ID, out var mappings) || mappings.Count == 0)
                return 0;

            var quarterResultType = NormalizeOptionalCode(index.QuarterResultCalculateType, "SUM_MONTH");
            var periodType = NormalizeOptionalCode(runtime.Period.PeriodType, "MONTH");

            // MANUAL: nhập tay, không tự tính kết quả quý
            if (quarterResultType == "MANUAL"
                && (periodType == "QUARTER" || periodType == "YEAR"))
                return 0;

            // SUM_MONTH: tính riêng từng tháng rồi cộng lại (distinct theo tháng)
            if (quarterResultType == "SUM_MONTH"
                && (periodType == "QUARTER" || periodType == "YEAR"))
            {
                var childMonthPeriods = await GetChildMonthPeriodsAsync(runtime.Period);
                if (childMonthPeriods.Count > 0)
                {
                    decimal total = 0;
                    foreach (var monthPeriod in childMonthPeriods)
                    {
                        var monthRuntime = runtime.CloneWithPeriod(monthPeriod);
                        foreach (var mapping in mappings)
                            total += await CalculateMappingAsync(mapping, monthRuntime);
                    }
                    return total;
                }
            }

            // FULL_PERIOD hoặc kỳ tháng: tính trực tiếp trên toàn bộ khoảng ngày kỳ hiện tại (distinct toàn quý)
            decimal result = 0;
            foreach (var mapping in mappings)
                result += await CalculateMappingAsync(mapping, runtime);

            return result;
        }

        /// <summary>
        /// Lấy danh sách kỳ tháng con của một kỳ quý/năm.
        /// Nếu là YEAR thì lấy tất cả tháng thuộc các quý con.
        /// </summary>
        private async Task<List<KPISalePeriod>> GetChildMonthPeriodsAsync(KPISalePeriod parentPeriod)
        {
            var periodType = NormalizeOptionalCode(parentPeriod.PeriodType, "MONTH");

            if (periodType == "QUARTER")
            {
                return await _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                    .Where(x => x.ParentPeriodID == parentPeriod.ID && x.PeriodType == "MONTH")
                    .OrderBy(x => x.DateStart)
                    .ToListAsync();
            }

            if (periodType == "YEAR")
            {
                var quarterIds = await _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                    .Where(x => x.ParentPeriodID == parentPeriod.ID && x.PeriodType == "QUARTER")
                    .Select(x => x.ID)
                    .ToListAsync();

                return await _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                    .Where(x => x.ParentPeriodID.HasValue
                        && quarterIds.Contains(x.ParentPeriodID.Value)
                        && x.PeriodType == "MONTH")
                    .OrderBy(x => x.DateStart)
                    .ToListAsync();
            }

            return new List<KPISalePeriod>();
        }

        /// <summary>
        /// Tính tổng goal value từ các tháng con cho một index khi kỳ hiện tại là QUARTER hoặc YEAR.
        /// Dùng QuarterResultCalculateType: SUM_MONTH, FULL_PERIOD, MANUAL.
        /// </summary>
        private async Task<decimal> SumGoalFromChildMonthsAsync(KPISaleIndex index, KPISaleRuntimeContext runtime)
        {
            var quarterGoalType = NormalizeOptionalCode(index.QuarterGoalCalculateType, "SUM_MONTH");
            var periodType = NormalizeOptionalCode(runtime.Period.PeriodType, "MONTH");

            // MANUAL: goal được nhập trực tiếp cho QUÝ, không sum từ tháng
            if (quarterGoalType == "MANUAL")
                return 0;

            if ((periodType == "QUARTER" || periodType == "YEAR") && quarterGoalType == "SUM_MONTH")
            {
                var childMonthPeriods = await GetChildMonthPeriodsAsync(runtime.Period);
                decimal total = 0;
                foreach (var monthPeriod in childMonthPeriods)
                {
                    if (runtime.TargetsByIndex.TryGetValue((monthPeriod.ID, index.ID), out var monthGoal))
                        total += monthGoal;
                }
                return total;
            }

            return 0;
        }

        /// <summary>
        /// Tính tổng goal value từ các chỉ tiêu con cùng kỳ.
        /// Áp dụng cho GROUP và FORMULA index.
        /// Nếu kỳ hiện tại là QUÝ/NĂM, sẽ tìm goal của con từ các tháng con.
        /// </summary>
        private async Task<decimal> SumGoalFromChildrenAsync(KPISaleIndex index, KPISaleRuntimeContext runtime)
        {
            var children = ResolveChildren(index, runtime);
            if (children.Count == 0)
                return 0;

            var periodType = NormalizeOptionalCode(runtime.Period.PeriodType, "MONTH");
            List<KPISalePeriod>? childMonthPeriods = null;
            if (periodType == "QUARTER" || periodType == "YEAR")
            {
                childMonthPeriods = await GetChildMonthPeriodsAsync(runtime.Period);
            }

            decimal sum = 0;
            foreach (var child in children.OrderBy(x => x.SortOrder).ThenBy(x => x.ID))
            {
                var childIndexType = NormalizeOptionalCode(child.IndexType, "DETAIL");

                // Lấy goal trực tiếp của con cho kỳ hiện tại
                if (runtime.TargetsByIndex.TryGetValue((runtime.Period.ID, child.ID), out var childGoal))
                {
                    sum += childGoal;
                }
                else if (childIndexType == "GROUP" || childIndexType == "FORMULA")
                {
                    // Nếu con là GROUP/FORMULA và không có goal, gọi đệ quy
                    sum += await SumGoalFromChildrenAsync(child, runtime);
                }
                else if (childIndexType == "DETAIL" && childMonthPeriods != null)
                {
                    // Nếu con là DETAIL và kỳ hiện tại là QUÝ/NĂM, sum goal từ các tháng con
                    foreach (var monthPeriod in childMonthPeriods)
                    {
                        if (runtime.TargetsByIndex.TryGetValue((monthPeriod.ID, child.ID), out var monthGoal))
                            sum += monthGoal;
                    }
                }
            }
            return sum;
        }

        /// <summary>
        /// Lấy danh sách child index của 1 GROUP.
        /// Ưu tiên ChildrenByParent (group theo KPISaleIndex.ParentID).
        /// Fallback: nếu rỗng, dùng FormulaItemsByParent (group theo KPISaleIndexFormulaItems.ParentKpiIndexID)
        /// — trường hợp ParentID trong bảng Index không được set đúng nhưng formula items có.
        /// </summary>
        private List<KPISaleIndex> ResolveChildren(KPISaleIndex index, KPISaleRuntimeContext runtime)
        {
            if (runtime.ChildrenByParent.TryGetValue(index.ID, out var children) && children.Count > 0)
                return children;

            // Fallback: dùng formula items
            if (runtime.FormulaItemsByParent.TryGetValue(index.ID, out var formulaItems) && formulaItems.Count > 0)
            {
                var resolved = new List<KPISaleIndex>();
                var seenIds = new HashSet<int>();
                foreach (var item in formulaItems.OrderBy(x => x.SortOrder).ThenBy(x => x.ID))
                {
                    if (seenIds.Contains(item.ChildKpiIndexID)) continue;
                    if (runtime.IndexById.TryGetValue(item.ChildKpiIndexID, out var child))
                    {
                        resolved.Add(child);
                        seenIds.Add(child.ID);
                    }
                }
                if (resolved.Count > 0)
                    return resolved;
            }
            return new List<KPISaleIndex>();
        }

        private async Task<decimal> CalculateFormulaIndexAsync(KPISaleIndex index, KPISaleRuntimeContext runtime)
        {
            if (runtime.FormulaItemsByParent.TryGetValue(index.ID, out var items) && items.Count > 0)
            {
                decimal? result = null;

                foreach (var item in items)
                {
                    if (!runtime.IndexById.TryGetValue(item.ChildKpiIndexID, out var childIndex))
                        continue;

                    var childValue = await ResolveIndexResultAsync(childIndex, runtime);
                    var op = NormalizeFormulaOperator(item.Operator);

                    if (!result.HasValue)
                    {
                        result = op == "-" ? -childValue : childValue;
                        continue;
                    }

                    result = ApplyFormulaOperator(result.Value, childValue, op);
                }

                return result ?? 0;
            }

            var resolvedChildren = ResolveChildren(index, runtime);
            if (resolvedChildren.Count == 0)
                return 0;

            decimal sum = 0;
            foreach (var child in resolvedChildren.OrderBy(x => x.SortOrder).ThenBy(x => x.ID))
                sum += await ResolveIndexResultAsync(child, runtime);

            return sum;
        }

        private async Task<decimal> CalculateMappingAsync(KPISaleIndexDataMapping mapping, KPISaleRuntimeContext runtime)
        {
            var aggregateType = NormalizeOptionalCode(mapping.AggregateType, "SUM");
            if (!AllowedAggregateTypes.Contains(aggregateType))
                throw new Exception($"AggregateType không hợp lệ: {mapping.AggregateType}");

            var source = await _kpiSaleRepo.KPISaleDataSources.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == mapping.DataSourceID && x.IsActive);
            if (source == null || source.ID <= 0)
                throw new Exception($"Không tìm thấy data source đang hoạt động cho mapping {mapping.ID}");

            var table = await _kpiSaleRepo.KPISaleAllowedTables.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == source.AllowedTableID && x.IsActive);
            if (table == null || table.ID <= 0)
                throw new Exception($"Không tìm thấy bảng được phép cho data source {source.SourceCode}");

            var columns = await _kpiSaleRepo.KPISaleAllowedColumns.AsNoTracking()
                .Where(x => x.TableID == table.ID && x.IsActive)
                .ToListAsync();
            var columnMap = columns.ToDictionary(x => x.ColumnName, StringComparer.OrdinalIgnoreCase);

            ValidateIdentifier(table.SchemaName, nameof(table.SchemaName));
            ValidateIdentifier(table.TableName, nameof(table.TableName));
            ValidateColumnAllowed(columnMap, source.DateColumn, false, false, false);

            if (!string.IsNullOrWhiteSpace(source.EmployeeColumn))
                ValidateColumnAllowed(columnMap, source.EmployeeColumn, false, false, false);

            var valueColumn = NormalizeNullableIdentifier(mapping.ValueColumn) ?? NormalizeNullableIdentifier(source.ValueColumn);
            var distinctColumn = NormalizeNullableIdentifier(mapping.DistinctColumn);

            if (RequiresValueColumn(aggregateType))
            {
                if (string.IsNullOrWhiteSpace(valueColumn))
                    throw new Exception($"Mapping {mapping.ID} cần ValueColumn cho AggregateType {aggregateType}");
                ValidateColumnAllowed(columnMap, valueColumn, false, true, false);
            }

            if (RequiresDistinctColumn(aggregateType))
            {
                if (string.IsNullOrWhiteSpace(distinctColumn))
                    throw new Exception($"Mapping {mapping.ID} cần DistinctColumn cho AggregateType {aggregateType}");
                ValidateColumnAllowed(columnMap, distinctColumn, false, false, true);
            }

            var parameters = new List<SqlParameterValue>();
            string AddParameter(object? value)
            {
                var name = $"@p{parameters.Count}";
                parameters.Add(new SqlParameterValue(name, value));
                return name;
            }

            var whereParts = new List<string>();
            whereParts.Add($"{QuoteIdentifier(source.DateColumn)} >= {AddParameter(runtime.Period.DateStart.ToDateTime(TimeOnly.MinValue))}");
            whereParts.Add($"{QuoteIdentifier(source.DateColumn)} < {AddParameter(runtime.Period.DateEnd.AddDays(1).ToDateTime(TimeOnly.MinValue))}");

            if (!string.IsNullOrWhiteSpace(source.EmployeeColumn))
            {
                // Nếu UseEmployeeID = true, cần resolve EmployeeID từ UserID
                var employeeIdValue = runtime.Request.EmployeeID;
                if (source.UseEmployeeID)
                {
                    var employee = _employeeRepo.GetAll(e => e.UserID == runtime.Request.EmployeeID).FirstOrDefault();
                    if (employee != null && employee.ID > 0)
                        employeeIdValue = employee.ID;
                }
                whereParts.Add($"{QuoteIdentifier(source.EmployeeColumn)} = {AddParameter(employeeIdValue)}");
            }

            var softDeleteSql = BuildSoftDeleteFilterSql(columnMap);
            if (!string.IsNullOrWhiteSpace(softDeleteSql))
                whereParts.Add(softDeleteSql);

            var filterSql = await BuildMappingFilterSqlAsync(mapping.ID, columnMap, AddParameter, runtime);
            if (!string.IsNullOrWhiteSpace(filterSql))
                whereParts.Add(filterSql);

            var qualifiedTableName = $"{QuoteIdentifier(table.SchemaName)}.{QuoteIdentifier(table.TableName)}";
            var whereSql = whereParts.Count > 0
                ? " WHERE " + string.Join(" AND ", whereParts)
                : "";

            var sql = BuildAggregateSql(aggregateType, qualifiedTableName, whereSql, valueColumn, distinctColumn);
            return await ExecuteScalarDecimalAsync(sql, parameters);
        }

        private async Task<string> BuildMappingFilterSqlAsync(
            int mappingId,
            Dictionary<string, KPISaleAllowedColumn> columnMap,
            Func<object?, string> addParameter,
            KPISaleRuntimeContext runtime)
        {
            var groups = await _kpiSaleRepo.KPISaleMappingFilterGroups.AsNoTracking()
                .Where(x => x.MappingID == mappingId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .ToListAsync();

            if (groups.Count == 0)
                return "";

            var groupIds = groups.Select(x => x.ID).ToList();
            var conditions = await _kpiSaleRepo.KPISaleMappingFilterConditions.AsNoTracking()
                .Where(x => groupIds.Contains(x.FilterGroupID) && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .ToListAsync();

            var rootGroups = groups
                .Where(x => !x.ParentGroupID.HasValue)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .ToList();

            var parts = rootGroups
                .Select(x => BuildFilterGroupSql(x, groups, conditions, columnMap, addParameter, runtime))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (parts.Count == 0)
                return "";

            return parts.Count == 1
                ? parts[0]
                : "(" + string.Join(" AND ", parts) + ")";
        }

        private string BuildFilterGroupSql(
            KPISaleMappingFilterGroup group,
            List<KPISaleMappingFilterGroup> groups,
            List<KPISaleMappingFilterCondition> conditions,
            Dictionary<string, KPISaleAllowedColumn> columnMap,
            Func<object?, string> addParameter,
            KPISaleRuntimeContext runtime)
        {
            var parts = new List<string>();

            parts.AddRange(conditions
                .Where(x => x.FilterGroupID == group.ID)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .Select(x => BuildFilterConditionSql(x, columnMap, addParameter, runtime))
                .Where(x => !string.IsNullOrWhiteSpace(x)));

            var childParts = groups
                .Where(x => x.ParentGroupID == group.ID)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .Select(x => BuildFilterGroupSql(x, groups, conditions, columnMap, addParameter, runtime))
                .Where(x => !string.IsNullOrWhiteSpace(x));
            parts.AddRange(childParts);

            if (parts.Count == 0)
                return "";

            var logicOperator = NormalizeLogicOperator(group.LogicOperator);
            return parts.Count == 1
                ? parts[0]
                : "(" + string.Join($" {logicOperator} ", parts) + ")";
        }

        private string BuildFilterConditionSql(
            KPISaleMappingFilterCondition condition,
            Dictionary<string, KPISaleAllowedColumn> columnMap,
            Func<object?, string> addParameter,
            KPISaleRuntimeContext runtime)
        {
            ValidateColumnAllowed(columnMap, condition.ColumnName, true, false, false);

            var columnName = QuoteIdentifier(condition.ColumnName);
            var op = NormalizeOperator(condition.Operator);
            var valueType = NormalizeValueType(condition.ValueType);

            if (op == "IS NULL" || op == "IS NOT NULL")
                return $"{columnName} {op}";

            if (valueType == "COLUMN")
            {
                var compareColumn = NormalizeNullableIdentifier(condition.Value1)
                    ?? throw new Exception("Value1 phải là tên cột khi ValueType = COLUMN");
                ValidateColumnAllowed(columnMap, compareColumn, true, false, false);
                return $"{columnName} {op} {QuoteIdentifier(compareColumn)}";
            }

            if (op == "IN")
            {
                var values = ResolveConditionValues(condition, valueType, runtime).ToList();
                if (values.Count == 0)
                    throw new Exception($"Filter condition {condition.ID} dùng IN nhưng chưa có giá trị");

                var parameterNames = values.Select(addParameter).ToList();
                return $"{columnName} IN ({string.Join(", ", parameterNames)})";
            }

            if (op == "BETWEEN")
            {
                var value1 = ResolveConditionValue(condition.Value1, condition.DataType, valueType, runtime);
                var value2 = ResolveConditionValue(condition.Value2, condition.DataType, valueType, runtime);
                return $"{columnName} BETWEEN {addParameter(value1)} AND {addParameter(value2)}";
            }

            var value = ResolveConditionValue(condition.Value1, condition.DataType, valueType, runtime);

            // Tự động convert "true"/"false" sang 1/0 cho cột số (INT, BIT, BOOL)
            if (value is string strVal)
            {
                var colDataType = NormalizeOptionalCode(condition.DataType, "STRING");
                if (colDataType is "NUMBER" or "INT" or "BIGINT" or "DECIMAL" or "MONEY" or "BIT" or "BOOL" or "BOOLEAN")
                {
                    if (strVal.Equals("true", StringComparison.OrdinalIgnoreCase))
                        value = 1;
                    else if (strVal.Equals("false", StringComparison.OrdinalIgnoreCase))
                        value = 0;
                }
            }

            if (op == "LIKE" && value is string strValue && !strValue.Contains('%') && !strValue.Contains('_'))
                value = $"%{strValue}%";

            return $"{columnName} {op} {addParameter(value)}";
        }

        private string BuildSoftDeleteFilterSql(Dictionary<string, KPISaleAllowedColumn> columnMap)
        {
            var softDeleteConditions = new List<string>();

            foreach (var candidate in SoftDeleteColumnCandidates)
            {
                if (!columnMap.TryGetValue(candidate, out var column))
                    continue;

                var dataType = NormalizeOptionalCode(column.DataType, "STRING");
                var quotedColumn = QuoteIdentifier(column.ColumnName);

                //if (dataType is "BOOL" or "BIT" or "BOOLEAN")
                //{
                //    softDeleteConditions.Add($"({quotedColumn} IS NULL OR {quotedColumn} <> 1)");
                //}
                //else
                //{
                //    softDeleteConditions.Add($"({quotedColumn} IS NULL OR {quotedColumn} <> 'true')");
                //}
                if (dataType is "BOOL" or "BIT" or "BOOLEAN")
                {
                    softDeleteConditions.Add($"({quotedColumn} IS NULL OR {quotedColumn} <> 1)");
                }
                else if (dataType is "INT" or "BIGINT" or "NUMBER")
                {
                    softDeleteConditions.Add($"({quotedColumn} IS NULL OR {quotedColumn} <> 1)");
                }
                else
                {
                    softDeleteConditions.Add($"({quotedColumn} IS NULL OR {quotedColumn} <> 'true')");
                }
            }

            if (softDeleteConditions.Count == 0)
                return string.Empty;

            return string.Join(" AND ", softDeleteConditions);
        }

        private static string BuildAggregateSql(
            string aggregateType,
            string qualifiedTableName,
            string whereSql,
            string? valueColumn,
            string? distinctColumn)
        {
            return aggregateType switch
            {
                "SUM" => $"SELECT COALESCE(SUM(CAST({QuoteIdentifier(valueColumn!)} AS decimal(38,4))), 0) FROM {qualifiedTableName}{whereSql}",
                "COUNT" => $"SELECT COUNT(1) FROM {qualifiedTableName}{whereSql}",
                "COUNT_DISTINCT" => $"SELECT COUNT(DISTINCT {QuoteIdentifier(distinctColumn!)}) FROM {qualifiedTableName}{whereSql}",
                "SUM_DISTINCT" => $@"
SELECT COALESCE(SUM(x.ValueColumn), 0)
FROM (
    SELECT DISTINCT {QuoteIdentifier(distinctColumn!)} AS DistinctColumn,
           CAST({QuoteIdentifier(valueColumn!)} AS decimal(38,4)) AS ValueColumn
    FROM {qualifiedTableName}
    {whereSql}
) x",
                "AVG" => $"SELECT COALESCE(AVG(CAST({QuoteIdentifier(valueColumn!)} AS decimal(38,4))), 0) FROM {qualifiedTableName}{whereSql}",
                "MAX" => $"SELECT COALESCE(MAX(CAST({QuoteIdentifier(valueColumn!)} AS decimal(38,4))), 0) FROM {qualifiedTableName}{whereSql}",
                "MIN" => $"SELECT COALESCE(MIN(CAST({QuoteIdentifier(valueColumn!)} AS decimal(38,4))), 0) FROM {qualifiedTableName}{whereSql}",
                _ => throw new Exception($"AggregateType không hỗ trợ: {aggregateType}")
            };
        }

        private async Task<decimal> ExecuteScalarDecimalAsync(string sql, List<SqlParameterValue> parameters)
        {
            return await _kpiSaleRepo.ExecuteScalarDecimalAsync(
                sql,
                parameters.Select(x => new KPISaleSqlParameter(x.Name, x.Value)));
        }

        private async Task SaveSnapshotAsync(
            KPISaleCalculateRequest request,
            List<KPISaleCalculateResult> results,
            KPISaleTotalPerformanceDto totalPerformance)
        {
            var kpiIndexIds = results.Select(x => x.KpiIndexID).ToList();
            var oldResults = await _kpiSaleRepo.KPISaleResults
                .Where(x => x.EmployeeID == request.EmployeeID
                    && x.PeriodID == request.PeriodID
                    && kpiIndexIds.Contains(x.KpiIndexID))
                .ToListAsync();

            _kpiSaleRepo.KPISaleResults.RemoveRange(oldResults);

            var existingTotalPerformance = await _kpiSaleRepo.KPISaleTotalPerformances
                .FirstOrDefaultAsync(x => x.EmployeeID == request.EmployeeID
                    && x.PeriodID == request.PeriodID
                    && x.TemplateID == request.TemplateID);

            if (existingTotalPerformance != null)
                _kpiSaleRepo.KPISaleTotalPerformances.Remove(existingTotalPerformance);

            var now = DateTime.Now;
            var newResults = results.Select(x => new KPISaleResult
            {
                EmployeeID = request.EmployeeID,
                PeriodID = request.PeriodID,
                KpiIndexID = x.KpiIndexID,
                GoalValue = x.GoalValue,
                ResultValue = x.ResultValue,
                AchievedPercent = x.AchievedPercent,
                WeightPercent = x.WeightPercent,
                FinalScore = x.FinalScore,
                ReportScoreAdjustmentType = x.ReportScoreAdjustmentType ?? 0,
                ReportScoreValue = x.ReportScoreValue ?? 0,
                CalculatedDate = now
            }).ToList();

            await _kpiSaleRepo.KPISaleResults.AddRangeAsync(newResults);
            await _kpiSaleRepo.KPISaleTotalPerformances.AddAsync(new KPISaleTotalPerformance
            {
                EmployeeID = request.EmployeeID,
                PeriodID = request.PeriodID,
                TemplateID = request.TemplateID,
                FinalScore = totalPerformance.FinalScore,
                CalculatedDate = totalPerformance.CalculatedDate ?? now
            });
            await _kpiSaleRepo.SaveChangesAsync();
        }

        #endregion Calculator

        #region Validation and helper

        private async Task<KPISaleTarget> UpsertTargetAsync(
            KPISaleTargetUpsertRequest request,
            string currentUserName,
            KPISaleTarget? existing = null)
        {
            ValidateTargetRequest(request);

            var period = await _kpiSaleRepo.KPISalePeriods.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == request.PeriodID);
            if (period == null || period.ID <= 0)
                throw new Exception("Không tìm thấy kỳ KPI");

            // Kỳ QUÝ/NĂM được phép upsert GoalValue khi frontend cascade sum từ MONTH,
            // hoặc chỉ upsert WeightPercent. Không throw lỗi nữa.
            if (period.PeriodType != "MONTH")
            {
                if (period.PeriodType != "QUARTER" && period.PeriodType != "YEAR")
                    throw new Exception("Kỳ KPI không hợp lệ.");

                // Validate weight
                if (request.WeightPercent.HasValue && (request.WeightPercent.Value < 0 || request.WeightPercent.Value > 100))
                    throw new Exception("Trọng số phải nằm trong khoảng 0 - 100");

                var existingQuarter = await FindTargetAsync(request);
                if (existingQuarter == null || existingQuarter.ID <= 0)
                {
                    var model = new KPISaleTarget
                    {
                        EmployeeID = request.EmployeeID,
                        PeriodID = request.PeriodID,
                        KpiIndexID = request.KpiIndexID,
                        GoalValue = request.GoalValue,
                        WeightPercent = request.WeightPercent,
                        ProposedGoalValue = request.ProposedGoalValue,
                        ApprovalStatus = request.ProposedGoalValue.HasValue ? "Pending" : "Default",
                        CreatedBy = currentUserName,
                        CreatedDate = DateTime.Now
                    };
                    await _kpiSaleRepo.KPISaleTargets.AddAsync(model);
                    return model;
                }

                existingQuarter.GoalValue = request.GoalValue;
                existingQuarter.WeightPercent = request.WeightPercent;
                if (request.ProposedGoalValue.HasValue)
                    existingQuarter.ProposedGoalValue = request.ProposedGoalValue;
                existingQuarter.UpdatedBy = currentUserName;
                existingQuarter.UpdatedDate = DateTime.Now;
                return existingQuarter;
            }

            var kpiIndex = await _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == request.KpiIndexID);
            if (kpiIndex == null)
                throw new Exception("Không tìm thấy KPI index");

            // Validate: Employee phải được gán template chứa kpiIndex này
            var assignedTemplate = await _kpiSaleRepo.KPISaleEmployeeTemplates.AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeID == request.EmployeeID
                    && x.IsActive == true);
            if (assignedTemplate != null && assignedTemplate.TemplateID != kpiIndex.TemplateID)
            {
                throw new Exception("Chỉ tiêu KPI không thuộc mẫu KPI đã được gán cho nhân viên này. Vui lòng kiểm tra lại.");
            }

            // Validate WeightPercent (0-100)
            if (request.WeightPercent.HasValue && (request.WeightPercent.Value < 0 || request.WeightPercent.Value > 100))
            {
                throw new Exception("Trọng số phải nằm trong khoảng 0 - 100");
            }

            existing ??= await FindTargetAsync(request);

            if (existing == null || existing.ID <= 0)
            {
                var model = new KPISaleTarget
                {
                    EmployeeID = request.EmployeeID,
                    PeriodID = request.PeriodID,
                    KpiIndexID = request.KpiIndexID,
                    GoalValue = request.GoalValue,
                    WeightPercent = request.WeightPercent,
                    ProposedGoalValue = request.ProposedGoalValue,
                    ApprovalStatus = request.ProposedGoalValue.HasValue ? "Pending" : "Default",
                    CreatedBy = currentUserName,
                    CreatedDate = DateTime.Now
                };
                await _kpiSaleRepo.KPISaleTargets.AddAsync(model);
                return model;
            }

            // Nếu đã Approved → không cho sửa GoalValue
            if (existing.ApprovalStatus == "Approved")
            {
                // Chỉ cho cập nhật WeightPercent (vì GoalValue đã khóa ở FE rồi)
                existing.WeightPercent = request.WeightPercent;
                existing.UpdatedBy = currentUserName;
                existing.UpdatedDate = DateTime.Now;
                return existing;
            }

            existing.GoalValue = request.GoalValue;
            existing.WeightPercent = request.WeightPercent;

            // Nếu có ProposedGoalValue → cập nhật trạng thái chờ duyệt
            if (request.ProposedGoalValue.HasValue)
            {
                existing.ProposedGoalValue = request.ProposedGoalValue;
                existing.ApprovalStatus = "Pending";
                existing.UpdatedBy = currentUserName;
                existing.UpdatedDate = DateTime.Now;
            }

            return existing;
        }

        private async Task<KPISaleTarget?> FindTargetAsync(KPISaleTargetUpsertRequest request)
        {
            if (request.ID > 0)
                return await _kpiSaleRepo.KPISaleTargets.FindAsync(request.ID);

            return await _kpiSaleRepo.KPISaleTargets.FirstOrDefaultAsync(x =>
                x.EmployeeID == request.EmployeeID
                && x.PeriodID == request.PeriodID
                && x.KpiIndexID == request.KpiIndexID);
        }

        private void ValidatePeriod(KPISalePeriod model)
        {
            if (model == null)
                throw new Exception("Dữ liệu kỳ KPI không được để trống");

            if (string.IsNullOrWhiteSpace(model.PeriodCode))
                throw new Exception("PeriodCode không được để trống");

            if (string.IsNullOrWhiteSpace(model.PeriodType))
                throw new Exception("PeriodType không được để trống");

            if (model.DateStart == default || model.DateEnd == default)
                throw new Exception("DateStart/DateEnd không hợp lệ");

            if (model.DateEnd < model.DateStart)
                throw new Exception("DateEnd phải lớn hơn hoặc bằng DateStart");
        }

        private void ValidateTemplate(KPISaleTemplate model)
        {
            if (model == null)
                throw new Exception("Dữ liệu template không được để trống");

            if (string.IsNullOrWhiteSpace(model.TemplateCode))
                throw new Exception("TemplateCode không được để trống");

            if (string.IsNullOrWhiteSpace(model.TemplateName))
                throw new Exception("TemplateName không được để trống");
        }

        private async Task ValidateIndexAsync(KPISaleIndex model)
        {
            if (model == null)
                throw new Exception("Dữ liệu KPI index không được để trống");

            if (model.TemplateID <= 0)
                throw new Exception("TemplateID không hợp lệ");

            if (!await _kpiSaleRepo.KPISaleTemplates.AnyAsync(x => x.ID == model.TemplateID))
                throw new Exception("Không tìm thấy template KPI");

            KPISaleIndex? parentIndex = null;
            if (model.ParentID.HasValue)
            {
                if (model.ParentID.Value == model.ID && model.ID > 0)
                    throw new Exception("ParentID không được trùng với ID");

                parentIndex = await _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == model.ParentID.Value && x.TemplateID == model.TemplateID);
                if (parentIndex == null)
                    throw new Exception("Không tìm thấy KPI index cha trong cùng template");
            }

            if (string.IsNullOrWhiteSpace(model.IndexCode))
                throw new Exception("IndexCode không được để trống");

            if (string.IsNullOrWhiteSpace(model.IndexName))
                throw new Exception("IndexName không được để trống");

            if (string.IsNullOrWhiteSpace(model.IndexType))
                throw new Exception("IndexType không được để trống");

            if (string.IsNullOrWhiteSpace(model.UnitType))
                throw new Exception("UnitType không được để trống");

            if (model.WeightPercent < 0 || model.WeightPercent > 100)
                throw new Exception("Trọng số chỉ tiêu phải nằm trong khoảng từ 0 đến 100");

            var normalizedIndexType = model.IndexType.Trim().ToUpperInvariant();
            var normalizedReportAdjustmentType = model.ReportScoreAdjustmentType ?? 0;
            model.ReportScoreAdjustmentType = normalizedReportAdjustmentType;

            if (normalizedIndexType == "REPORT" && normalizedReportAdjustmentType != 0 && normalizedReportAdjustmentType != 1 && normalizedReportAdjustmentType != 2)
                throw new Exception("Kiểu điều chỉnh điểm của chỉ tiêu báo cáo chỉ được là 0 (không chọn), 1 (trừ điểm) hoặc 2 (cộng điểm)");

            var templateIndexes = await _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                .Where(x => x.TemplateID == model.TemplateID && x.IsActive && x.ID != model.ID)
                .ToListAsync();

            if (parentIndex != null)
            {
                var siblingsWeight = templateIndexes
                    .Where(x => x.ParentID == model.ParentID)
                    .Sum(x => x.WeightPercent);
                var maxChildWeight = parentIndex.WeightPercent;
                if (siblingsWeight + model.WeightPercent > maxChildWeight)
                    throw new Exception($"Tổng trọng số các chỉ tiêu con của nhóm '{parentIndex.IndexName}' không được vượt quá {maxChildWeight:0.##}%");
            }

            var totalNonGroupWeight = templateIndexes
                .Where(x => !string.Equals(x.IndexType, "GROUP", StringComparison.OrdinalIgnoreCase))
                .Sum(x => x.WeightPercent);
            if (!string.Equals(normalizedIndexType, "GROUP", StringComparison.OrdinalIgnoreCase))
                totalNonGroupWeight += model.WeightPercent;

            if (totalNonGroupWeight > 100)
                throw new Exception("Tổng trọng số toàn bộ chỉ tiêu, không tính dòng nhóm, không được vượt quá 100%");
        }

        private void ValidateAllowedTable(KPISaleAllowedTable model)
        {
            if (model == null)
                throw new Exception("Dữ liệu bảng được phép không được để trống");

            if (string.IsNullOrWhiteSpace(model.TableName))
                throw new Exception("TableName không được để trống");

            if (string.IsNullOrWhiteSpace(model.DisplayName))
                throw new Exception("DisplayName không được để trống");

            ValidateIdentifier(model.TableName, nameof(model.TableName));
            ValidateIdentifier(NormalizeOptionalIdentifier(model.SchemaName, "dbo"), nameof(model.SchemaName));
        }

        private async Task ValidateAllowedColumnAsync(KPISaleAllowedColumn model)
        {
            if (model == null)
                throw new Exception("Dữ liệu cột được phép không được để trống");

            if (model.TableID <= 0)
                throw new Exception("TableID không hợp lệ");

            if (!await _kpiSaleRepo.KPISaleAllowedTables.AnyAsync(x => x.ID == model.TableID))
                throw new Exception("Không tìm thấy bảng được phép");

            if (string.IsNullOrWhiteSpace(model.ColumnName))
                throw new Exception("ColumnName không được để trống");

            if (string.IsNullOrWhiteSpace(model.DisplayName))
                throw new Exception("DisplayName không được để trống");

            if (string.IsNullOrWhiteSpace(model.DataType))
                throw new Exception("DataType không được để trống");

            ValidateIdentifier(model.ColumnName, nameof(model.ColumnName));
        }

        private async Task ValidateDataSourceAsync(KPISaleDataSource model)
        {
            if (model == null)
                throw new Exception("Dữ liệu data source không được để trống");

            if (string.IsNullOrWhiteSpace(model.SourceCode))
                throw new Exception("SourceCode không được để trống");

            if (string.IsNullOrWhiteSpace(model.SourceName))
                throw new Exception("SourceName không được để trống");

            if (model.AllowedTableID <= 0)
                throw new Exception("AllowedTableID không hợp lệ");

            var columns = await _kpiSaleRepo.KPISaleAllowedColumns.AsNoTracking()
                .Where(x => x.TableID == model.AllowedTableID && x.IsActive)
                .ToListAsync();
            var columnMap = columns.ToDictionary(x => x.ColumnName, StringComparer.OrdinalIgnoreCase);

            ValidateColumnAllowed(columnMap, model.DateColumn, false, false, false);

            if (!string.IsNullOrWhiteSpace(model.EmployeeColumn))
                ValidateColumnAllowed(columnMap, model.EmployeeColumn, false, false, false);

            if (!string.IsNullOrWhiteSpace(model.ValueColumn))
                ValidateColumnAllowed(columnMap, model.ValueColumn, false, true, false);
        }

        private async Task ValidateMappingAsync(KPISaleIndexDataMapping model)
        {
            if (model == null)
                throw new Exception("Dữ liệu KPI mapping không được để trống");

            if (model.KpiIndexID <= 0)
                throw new Exception("KpiIndexID không hợp lệ");

            if (!await _kpiSaleRepo.KPISaleIndices.AnyAsync(x => x.ID == model.KpiIndexID))
                throw new Exception("Không tìm thấy KPI index");

            if (model.DataSourceID <= 0)
                throw new Exception("DataSourceID không hợp lệ");

            var source = await _kpiSaleRepo.KPISaleDataSources.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == model.DataSourceID);
            if (source == null || source.ID <= 0)
                throw new Exception("Không tìm thấy data source");

            var aggregateType = NormalizeOptionalCode(model.AggregateType, "");
            if (!AllowedAggregateTypes.Contains(aggregateType))
                throw new Exception("AggregateType không hợp lệ");

            var columns = await _kpiSaleRepo.KPISaleAllowedColumns.AsNoTracking()
                .Where(x => x.TableID == source.AllowedTableID && x.IsActive)
                .ToListAsync();
            var columnMap = columns.ToDictionary(x => x.ColumnName, StringComparer.OrdinalIgnoreCase);

            var valueColumn = NormalizeNullableIdentifier(model.ValueColumn) ?? NormalizeNullableIdentifier(source.ValueColumn);
            if (RequiresValueColumn(aggregateType))
                ValidateColumnAllowed(columnMap, valueColumn, false, true, false);

            if (RequiresDistinctColumn(aggregateType))
                ValidateColumnAllowed(columnMap, model.DistinctColumn, false, false, true);
        }

        private async Task ValidateFilterGroupAsync(KPISaleMappingFilterGroup model)
        {
            if (model == null)
                throw new Exception("Dữ liệu filter group không được để trống");

            if (model.MappingID <= 0)
                throw new Exception("MappingID không hợp lệ");

            if (!await _kpiSaleRepo.KPISaleIndexDataMappings.AnyAsync(x => x.ID == model.MappingID))
                throw new Exception("Không tìm thấy KPI mapping");

            if (!string.IsNullOrWhiteSpace(model.LogicOperator) && !AllowedLogicOperators.Contains(model.LogicOperator.Trim()))
                throw new Exception("LogicOperator chỉ được là AND hoặc OR");

            if (model.ParentGroupID.HasValue)
            {
                if (model.ParentGroupID.Value == model.ID && model.ID > 0)
                    throw new Exception("ParentGroupID không được trùng với ID");

                var parentExists = await _kpiSaleRepo.KPISaleMappingFilterGroups.AnyAsync(x =>
                    x.ID == model.ParentGroupID.Value && x.MappingID == model.MappingID);
                if (!parentExists)
                    throw new Exception("Không tìm thấy filter group cha cùng mapping");
            }
        }

        private async Task ValidateFilterConditionAsync(KPISaleMappingFilterCondition model)
        {
            if (model == null)
                throw new Exception("Dữ liệu filter condition không được để trống");

            if (model.FilterGroupID <= 0)
                throw new Exception("FilterGroupID không hợp lệ");

            var group = await _kpiSaleRepo.KPISaleMappingFilterGroups.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == model.FilterGroupID);
            if (group == null || group.ID <= 0)
                throw new Exception("Không tìm thấy filter group");

            var mapping = await _kpiSaleRepo.KPISaleIndexDataMappings.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == group.MappingID);
            if (mapping == null || mapping.ID <= 0)
                throw new Exception("Không tìm thấy KPI mapping");

            var source = await _kpiSaleRepo.KPISaleDataSources.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == mapping.DataSourceID);
            if (source == null || source.ID <= 0)
                throw new Exception("Không tìm thấy data source");

            var columns = await _kpiSaleRepo.KPISaleAllowedColumns.AsNoTracking()
                .Where(x => x.TableID == source.AllowedTableID && x.IsActive)
                .ToListAsync();
            var columnMap = columns.ToDictionary(x => x.ColumnName, StringComparer.OrdinalIgnoreCase);
            ValidateColumnAllowed(columnMap, model.ColumnName, true, false, false);

            NormalizeOperator(model.Operator);
            NormalizeValueType(model.ValueType);
        }

        private async Task ValidateFormulaItemAsync(KPISaleIndexFormulaItem model)
        {
            if (model == null)
                throw new Exception("Dữ liệu formula item không được để trống");

            if (model.ParentKpiIndexID <= 0 || model.ChildKpiIndexID <= 0)
                throw new Exception("ParentKpiIndexID/ChildKpiIndexID không hợp lệ");

            if (model.ParentKpiIndexID == model.ChildKpiIndexID)
                throw new Exception("KPI cha và KPI con không được trùng nhau");

            var indexes = await _kpiSaleRepo.KPISaleIndices.AsNoTracking()
                .Where(x => x.ID == model.ParentKpiIndexID || x.ID == model.ChildKpiIndexID)
                .ToListAsync();

            if (indexes.Count != 2)
                throw new Exception("Không tìm thấy KPI index cha hoặc con");

            if (indexes.Select(x => x.TemplateID).Distinct().Count() > 1)
                throw new Exception("KPI index cha và con phải cùng template");

            NormalizeFormulaOperator(model.Operator);
        }

        private async Task ValidateScoringRuleAsync(KPISaleScoringRule model)
        {
            if (model == null)
                throw new Exception("Dữ liệu scoring rule không được để trống");

            if (model.KpiIndexID <= 0)
                throw new Exception("KpiIndexID không hợp lệ");

            if (!await _kpiSaleRepo.KPISaleIndices.AnyAsync(x => x.ID == model.KpiIndexID))
                throw new Exception("Không tìm thấy KPI index");

            if (string.IsNullOrWhiteSpace(model.ScoreType))
                throw new Exception("ScoreType không được để trống");
        }

        private static void ValidateTargetRequest(KPISaleTargetUpsertRequest request)
        {
            if (request == null)
                throw new Exception("Dữ liệu target không được để trống");

            if (request.EmployeeID <= 0)
                throw new Exception("EmployeeID không hợp lệ");

            if (request.PeriodID <= 0)
                throw new Exception("PeriodID không hợp lệ");

            if (request.KpiIndexID <= 0)
                throw new Exception("KpiIndexID không hợp lệ");
        }

        private static void ValidateCalculateRequest(KPISaleCalculateRequest request)
        {
            if (request == null)
                throw new Exception("Dữ liệu tính KPI không được để trống");

            if (request.EmployeeID <= 0)
                throw new Exception("EmployeeID không hợp lệ");

            if (request.PeriodID <= 0)
                throw new Exception("PeriodID không hợp lệ");

            if (request.TemplateID <= 0)
                throw new Exception("TemplateID không hợp lệ");
        }

        private static void ValidateIdentifier(string? identifier, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new Exception($"{fieldName} không được để trống");

            if (!SqlIdentifierRegex.IsMatch(identifier.Trim()))
                throw new Exception($"{fieldName} không hợp lệ");
        }

        private static void ValidateColumnAllowed(
            Dictionary<string, KPISaleAllowedColumn> columnMap,
            string? columnName,
            bool requireFilter,
            bool requireAggregate,
            bool requireDistinct)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new Exception("ColumnName không được để trống");

            ValidateIdentifier(columnName, "ColumnName");

            if (!columnMap.TryGetValue(columnName.Trim(), out var column))
                throw new Exception($"Cột không nằm trong danh sách được phép: {columnName}");

            if (requireFilter && !column.CanFilter)
                throw new Exception($"Cột không được phép filter: {columnName}");

            if (requireAggregate && !column.CanAggregate)
                throw new Exception($"Cột không được phép aggregate: {columnName}");

            if (requireDistinct && !column.CanDistinct)
                throw new Exception($"Cột không được phép distinct: {columnName}");
        }

        private static string NormalizeOptionalIdentifier(string? value, string defaultValue)
        {
            var normalized = string.IsNullOrWhiteSpace(value)
                ? defaultValue
                : value.Trim();
            ValidateIdentifier(normalized, nameof(value));
            return normalized;
        }

        private static string? NormalizeNullableIdentifier(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var normalized = value.Trim();
            ValidateIdentifier(normalized, nameof(value));
            return normalized;
        }

        private static string[] SplitMultiValue(string? rawValue)
        {
            return string.IsNullOrWhiteSpace(rawValue)
                ? Array.Empty<string>()
                : rawValue
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();
        }

        private async Task<string?> BuildLookupDisplayTextAsync(string lookupSchema, string lookupTable, string valueColumn, string displayCol, string? rawValue)
        {
            var values = SplitMultiValue(rawValue);
            if (values.Length == 0)
                return null;

            var displays = new List<string>();
            foreach (var value in values)
            {
                var display = await _kpiSaleRepo.GetLookupDisplayValueAsync(lookupSchema, lookupTable, valueColumn, displayCol, value);
                displays.Add(string.IsNullOrWhiteSpace(display) ? value : display);
            }

            return displays.Count == 0 ? null : string.Join(", ", displays);
        }

        private static string? NormalizeManualValueMapJson(string? rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
                return null;

            _ = ParseManualLookupValues(rawJson);
            return rawJson.Trim();
        }

        private static List<KPISaleLookupValue> ParseManualLookupValues(string rawJson)
        {
            try
            {
                var json = rawJson.Trim();
                using var document = JsonDocument.Parse(json);
                var result = new List<KPISaleLookupValue>();

                if (document.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var property in document.RootElement.EnumerateObject())
                    {
                        result.Add(new KPISaleLookupValue
                        {
                            Value = property.Name,
                            Display = property.Value.ValueKind == JsonValueKind.Null ? property.Name : (property.Value.ToString() ?? property.Name)
                        });
                    }

                    return result;
                }

                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in document.RootElement.EnumerateArray())
                    {
                        if (item.ValueKind != JsonValueKind.Object)
                            continue;

                        string? value = null;
                        string? display = null;
                        foreach (var property in item.EnumerateObject())
                        {
                            if (property.NameEquals("value") || property.NameEquals("Value"))
                                value = property.Value.ToString();
                            else if (property.NameEquals("label") || property.NameEquals("Label") || property.NameEquals("display") || property.NameEquals("Display"))
                                display = property.Value.ToString();
                        }

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            result.Add(new KPISaleLookupValue
                            {
                                Value = value,
                                Display = string.IsNullOrWhiteSpace(display) ? value : display
                            });
                        }
                    }

                    return result;
                }
            }
            catch (JsonException ex)
            {
                throw new Exception($"ManualValueMapJson không đúng định dạng JSON: {ex.Message}");
            }

            throw new Exception("ManualValueMapJson chỉ hỗ trợ dạng object hoặc array");
        }

        private static string? BuildManualDisplayText(string? rawJson, string? rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
                return null;

            var values = SplitMultiValue(rawValue);
            if (values.Length == 0)
                return null;

            var map = ParseManualLookupValues(rawJson)
                .GroupBy(x => x.Value, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Display, StringComparer.OrdinalIgnoreCase);

            var displays = values.Select(value => map.TryGetValue(value, out var display) ? display : value);
            return string.Join(", ", displays);
        }

        private static string NormalizeOptionalCode(string? value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value)
                ? defaultValue
                : value.Trim().ToUpperInvariant();
        }

        private static string NormalizeLogicOperator(string? value)
        {
            var normalized = NormalizeOptionalCode(value, "AND");
            if (!AllowedLogicOperators.Contains(normalized))
                throw new Exception("LogicOperator chỉ được là AND hoặc OR");
            return normalized;
        }

        private static string NormalizeOperator(string? value)
        {
            var normalized = NormalizeOptionalCode(value, "");
            if (!AllowedOperators.Contains(normalized))
                throw new Exception($"Operator không hợp lệ: {value}");
            return normalized;
        }

        private static string NormalizeValueType(string? value)
        {
            var normalized = NormalizeOptionalCode(value, "STATIC");
            if (!AllowedValueTypes.Contains(normalized))
                throw new Exception($"ValueType không hợp lệ: {value}");
            return normalized;
        }

        private static string NormalizeFormulaOperator(string? value)
        {
            var normalized = string.IsNullOrWhiteSpace(value) ? "+" : value.Trim();
            if (normalized != "+" && normalized != "-" && normalized != "*" && normalized != "/")
                throw new Exception("Operator công thức chỉ được là +, -, *, /");
            return normalized;
        }

        private static void NormalizeFilterCondition(KPISaleMappingFilterCondition condition)
        {
            condition.ColumnName = condition.ColumnName.Trim();
            condition.Operator = NormalizeOperator(condition.Operator);
            condition.ValueType = NormalizeValueType(condition.ValueType);
            condition.Value1 = condition.Value1?.Trim();
            condition.Value2 = condition.Value2?.Trim();
            condition.DataType = NormalizeOptionalCode(condition.DataType, "STRING");
        }

        private static string QuoteIdentifier(string identifier)
        {
            ValidateIdentifier(identifier, nameof(identifier));
            return $"[{identifier.Trim()}]";
        }

        private object? ResolveConditionValue(
            string? rawValue,
            string dataType,
            string valueType,
            KPISaleRuntimeContext runtime)
        {
            if (valueType == "PARAM")
                return ResolveSystemParameter(rawValue, runtime);

            return ConvertStaticValue(rawValue, dataType);
        }

        private IEnumerable<object?> ResolveConditionValues(
            KPISaleMappingFilterCondition condition,
            string valueType,
            KPISaleRuntimeContext runtime)
        {
            if (valueType == "PARAM")
                return new[] { ResolveSystemParameter(condition.Value1, runtime) };

            return (condition.Value1 ?? "")
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => ConvertStaticValue(x, condition.DataType));
        }

        private object? ResolveSystemParameter(string? code, KPISaleRuntimeContext runtime)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new Exception("System parameter không được để trống");

            var paramCode = code.Trim();
            if (!AllowedSystemParameters.Contains(paramCode))
                throw new Exception($"System parameter không được phép: {paramCode}");

            return paramCode.ToUpperInvariant() switch
            {
                "EMPLOYEEID" => runtime.Request.EmployeeID,
                "DATESTART" => runtime.Period.DateStart.ToDateTime(TimeOnly.MinValue),
                "DATEEND" => runtime.Period.DateEnd.ToDateTime(TimeOnly.MaxValue),
                "DEPARTMENTID" => runtime.Request.DepartmentID ?? GetCurrentUser().DepartmentID,
                "PERIODID" => runtime.Request.PeriodID,
                _ => throw new Exception($"System parameter không hỗ trợ: {paramCode}")
            };
        }

        private static object? ConvertStaticValue(string? rawValue, string dataType)
        {
            if (rawValue == null)
                return null;

            var value = rawValue.Trim();
            var type = NormalizeOptionalCode(dataType, "STRING");

            if (type is "NUMBER" or "INT" or "BIGINT")
            {
                if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var intValue))
                    return intValue;

                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    return decimalValue;

                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out var currentDecimalValue))
                    return currentDecimalValue;

                throw new Exception($"Giá trị number không hợp lệ: {rawValue}");
            }

            if (type is "DECIMAL" or "MONEY")
            {
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    return decimalValue;

                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out var currentDecimalValue))
                    return currentDecimalValue;

                throw new Exception($"Giá trị decimal không hợp lệ: {rawValue}");
            }

            if (type is "DATE" or "DATETIME")
            {
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
                    return dateValue;

                if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnlyValue))
                    return dateOnlyValue.ToDateTime(TimeOnly.MinValue);

                throw new Exception($"Giá trị date không hợp lệ: {rawValue}");
            }

            if (type is "BOOL" or "BIT" or "BOOLEAN")
            {
                if (bool.TryParse(value, out var boolValue))
                    return boolValue;

                if (value == "1")
                    return true;

                if (value == "0")
                    return false;

                throw new Exception($"Giá trị boolean không hợp lệ: {rawValue}");
            }

            return value;
        }

        private static bool RequiresValueColumn(string aggregateType)
        {
            return aggregateType is "SUM" or "SUM_DISTINCT" or "AVG" or "MAX" or "MIN";
        }

        private static bool RequiresDistinctColumn(string aggregateType)
        {
            return aggregateType is "COUNT_DISTINCT" or "SUM_DISTINCT";
        }

        private static decimal ApplyFormulaOperator(decimal current, decimal next, string op)
        {
            return op switch
            {
                "+" => current + next,
                "-" => current - next,
                "*" => current * next,
                "/" => next == 0 ? 0 : current / next,
                _ => current
            };
        }

        private static decimal CalculateAchievedPercent(decimal goalValue, decimal resultValue, string scoreType)
        {
            decimal achievedPercent;

            if (scoreType == "REVERSE_PERCENT")
            {
                if (goalValue <= 0)
                    achievedPercent = resultValue <= 0 ? 100 : 0;
                else
                    achievedPercent = (2 - (resultValue / goalValue)) * 100;
            }
            else
            {
                if (goalValue <= 0)
                    achievedPercent = resultValue > 0 ? 100 : 0;
                else
                    achievedPercent = resultValue / goalValue * 100;
            }

            return Math.Max(0, Math.Round(achievedPercent, 4));
        }

        private static decimal CalculateFinalScore(
            decimal achievedPercent,
            decimal weightPercent,
            KPISaleScoringRule? scoringRule,
            string scoreType)
        {
            if (scoreType == "FIXED_IF_REACHED")
                return achievedPercent >= 100 ? weightPercent : 0;

            // Cap ở 250% trọng số: min(achievedPercent * weight, 2.5 * weight)
            var rawScore = achievedPercent * weightPercent / 100;
            var maxScore = 2.5m * weightPercent;
            return Math.Round(Math.Min(rawScore, maxScore), 4);
        }

        private CurrentUser GetCurrentUser()
        {
            var claims = User.Claims
                .GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x => x.First().Value);
            return ObjectMapper.GetCurrentUser(claims);
        }

        private static List<KPISaleFilterGroupNode> BuildFilterTree(
            List<KPISaleMappingFilterGroup> groups,
            List<KPISaleFilterConditionDto> conditions)
        {
            var nodeMap = groups.ToDictionary(x => x.ID, x => new KPISaleFilterGroupNode
            {
                ID = x.ID,
                MappingID = x.MappingID,
                ParentGroupID = x.ParentGroupID,
                LogicOperator = x.LogicOperator,
                SortOrder = x.SortOrder,
                Conditions = conditions
                    .Where(c => c.FilterGroupID == x.ID)
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.ID)
                    .ToList()
            });

            foreach (var group in groups.Where(x => x.ParentGroupID.HasValue))
            {
                if (nodeMap.TryGetValue(group.ParentGroupID!.Value, out var parent) &&
                    nodeMap.TryGetValue(group.ID, out var child))
                {
                    parent.Children.Add(child);
                }
            }

            return groups
                .Where(x => !x.ParentGroupID.HasValue)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .Select(x => nodeMap[x.ID])
                .ToList();
        }

        private static HashSet<int> GetDescendantGroupIds(int rootGroupId, List<KPISaleMappingFilterGroup> groups)
        {
            var result = new HashSet<int> { rootGroupId };
            var changed = true;

            while (changed)
            {
                changed = false;
                foreach (var group in groups)
                {
                    if (group.ParentGroupID.HasValue &&
                        result.Contains(group.ParentGroupID.Value) &&
                        result.Add(group.ID))
                    {
                        changed = true;
                    }
                }
            }

            return result;
        }

        private sealed class KPISaleRuntimeContext
        {
            public KPISaleCalculateRequest Request { get; set; } = new();
            public KPISalePeriod Period { get; set; } = new();
            public List<KPISaleIndex> Indexes { get; set; } = new();
            public Dictionary<int, KPISaleIndex> IndexById { get; set; } = new();
            public Dictionary<int, List<KPISaleIndexDataMapping>> MappingsByIndex { get; set; } = new();
            public Dictionary<int, List<KPISaleIndexFormulaItem>> FormulaItemsByParent { get; set; } = new();
            public Dictionary<int, List<KPISaleIndex>> ChildrenByParent { get; set; } = new();
            public Dictionary<(int PeriodID, int IndexID), decimal> TargetsByIndex { get; set; } = new();
            public Dictionary<(int PeriodID, int IndexID), decimal> WeightsByIndex { get; set; } = new();
            public Dictionary<int, KPISaleScoringRule> ScoringRulesByIndex { get; set; } = new();
            public Dictionary<int, KPISaleReportAdjustmentInputDto> ReportAdjustmentsByIndex { get; set; } = new();
            public Dictionary<int, decimal> CalculatedValues { get; } = new();
            public HashSet<int> Visiting { get; } = new();

            /// <summary>
            /// Tạo bản sao runtime nhưng dùng kỳ khác (dùng khi tính SUM_MONTH: tính từng tháng riêng).
            /// Giữ nguyên indexes, mappings, targets, scoring rules. Reset CalculatedValues và Visiting.
            /// </summary>
            public KPISaleRuntimeContext CloneWithPeriod(KPISalePeriod newPeriod)
            {
                return new KPISaleRuntimeContext
                {
                    Request = Request,
                    Period = newPeriod,
                    Indexes = Indexes,
                    IndexById = IndexById,
                    MappingsByIndex = MappingsByIndex,
                    FormulaItemsByParent = FormulaItemsByParent,
                    ChildrenByParent = ChildrenByParent,
                    TargetsByIndex = TargetsByIndex,
                    WeightsByIndex = WeightsByIndex,
                    ScoringRulesByIndex = ScoringRulesByIndex,
                    ReportAdjustmentsByIndex = ReportAdjustmentsByIndex,
                };
            }
        }

        private sealed class SqlParameterValue
        {
            public SqlParameterValue(string name, object? value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; }
            public object? Value { get; }
        }

        #endregion Validation and helper
    }
}