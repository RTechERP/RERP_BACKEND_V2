using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.KPISale;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.KPISale;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RERPAPI.Controllers.KPISale
{
    [Route("api/kpi")]
    [ApiController]
    [Authorize]
    public class KPISaleController : ControllerBase
    {
        private readonly KPISaleRepo _kpiSaleRepo;

        private static readonly Regex SqlIdentifierRegex = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

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

        public KPISaleController(KPISaleRepo kpiSaleRepo)
        {
            _kpiSaleRepo = kpiSaleRepo;
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
                    .OrderByDescending(x => x.DateStart)
                    .ThenByDescending(x => x.ID)
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

        #endregion

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

        #endregion

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

        #endregion

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
                model.IsActive = request.IsActive;

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

        #endregion

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

        #endregion

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

                var nodes = BuildFilterTree(groups, conditions);

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

        #endregion

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

        #endregion

        #region Target and result

        [HttpGet("targets")]
        public async Task<IActionResult> GetTargets(int? employeeId = null, int? periodId = null, int? templateId = null)
        {
            try
            {
                var query =
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
                        target.KpiIndexID,
                        index.TemplateID,
                        index.IndexCode,
                        index.IndexName,
                        index.UnitType,
                        index.SortOrder,
                        target.GoalValue,
                        target.CreatedBy,
                        target.CreatedDate,
                        target.UpdatedBy,
                        target.UpdatedDate
                    };

                if (employeeId.HasValue)
                    query = query.Where(x => x.EmployeeID == employeeId.Value);

                if (periodId.HasValue)
                    query = query.Where(x => x.PeriodID == periodId.Value);

                if (templateId.HasValue)
                    query = query.Where(x => x.TemplateID == templateId.Value);

                var data = await query
                    .OrderBy(x => x.EmployeeID)
                    .ThenBy(x => x.PeriodID)
                    .ThenBy(x => x.SortOrder)
                    .ToListAsync();

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

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] KPISaleCalculateRequest request)
        {
            try
            {
                ValidateCalculateRequest(request);

                var result = await CalculateInternalAsync(request);
                return Ok(ApiResponseFactory.Success(result, "Tính KPI thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("results")]
        public async Task<IActionResult> GetResults(int? employeeId = null, int? periodId = null, int? templateId = null)
        {
            try
            {
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
                        result.GoalValue,
                        result.ResultValue,
                        result.AchievedPercent,
                        result.WeightPercent,
                        result.FinalScore,
                        result.CalculatedDate
                    };

                if (employeeId.HasValue)
                    query = query.Where(x => x.EmployeeID == employeeId.Value);

                if (periodId.HasValue)
                    query = query.Where(x => x.PeriodID == periodId.Value);

                if (templateId.HasValue)
                    query = query.Where(x => x.TemplateID == templateId.Value);

                var data = await query
                    .OrderBy(x => x.EmployeeID)
                    .ThenBy(x => x.PeriodID)
                    .ThenBy(x => x.SortOrder)
                    .ToListAsync();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion

        #region Calculator

        private async Task<List<KPISaleCalculateResult>> CalculateInternalAsync(KPISaleCalculateRequest request)
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

            var mappings = await _kpiSaleRepo.KPISaleIndexDataMappings.AsNoTracking()
                .Where(x => indexIds.Contains(x.KpiIndexID) && x.IsActive)
                .ToListAsync();

            var formulaItems = await _kpiSaleRepo.KPISaleIndexFormulaItems.AsNoTracking()
                .Where(x => indexIds.Contains(x.ParentKpiIndexID))
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ID)
                .ToListAsync();

            var targets = await _kpiSaleRepo.KPISaleTargets.AsNoTracking()
                .Where(x => x.EmployeeID == request.EmployeeID
                    && x.PeriodID == request.PeriodID
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
                TargetsByIndex = targets.ToDictionary(x => x.KpiIndexID, x => x.GoalValue),
                ScoringRulesByIndex = scoringRules
                    .GroupBy(x => x.KpiIndexID)
                    .ToDictionary(x => x.Key, x => x.OrderByDescending(r => r.ID).First())
            };

            foreach (var index in indexes)
                await ResolveIndexResultAsync(index, runtime);

            var results = indexes.Select(index =>
            {
                var resultValue = runtime.CalculatedValues.GetValueOrDefault(index.ID);
                var goalValue = runtime.TargetsByIndex.GetValueOrDefault(index.ID);
                runtime.ScoringRulesByIndex.TryGetValue(index.ID, out var scoringRule);
                var scoreType = NormalizeOptionalCode(scoringRule?.ScoreType, "NORMAL_PERCENT");
                var achievedPercent = CalculateAchievedPercent(goalValue, resultValue, scoreType);
                var finalScore = CalculateFinalScore(achievedPercent, index.WeightPercent, scoringRule, scoreType);

                return new KPISaleCalculateResult
                {
                    KpiIndexID = index.ID,
                    //ParentID = index.ParentID,
                    IndexCode = index.IndexCode,
                    IndexName = index.IndexName,
                    GoalValue = goalValue,
                    ResultValue = resultValue,
                    AchievedPercent = achievedPercent,
                    WeightPercent = index.WeightPercent,
                    FinalScore = finalScore,
                    UnitType = index.UnitType,
                    SortOrder = index.SortOrder,
                    IsMainIndex = index.IsMainIndex,
                    IsBold = index.IsBold
                };
            }).ToList();

            if (request.SaveSnapshot)
                await SaveSnapshotAsync(request, results);

            return results;
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

            decimal result = 0;
            foreach (var mapping in mappings)
                result += await CalculateMappingAsync(mapping, runtime);

            return result;
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

            if (!runtime.ChildrenByParent.TryGetValue(index.ID, out var children) || children.Count == 0)
                return 0;

            decimal sum = 0;
            foreach (var child in children.OrderBy(x => x.SortOrder).ThenBy(x => x.ID))
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
                whereParts.Add($"{QuoteIdentifier(source.EmployeeColumn)} = {AddParameter(runtime.Request.EmployeeID)}");

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
            if (op == "LIKE" && value is string strValue && !strValue.Contains('%') && !strValue.Contains('_'))
                value = $"%{strValue}%";

            return $"{columnName} {op} {addParameter(value)}";
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

        private async Task SaveSnapshotAsync(KPISaleCalculateRequest request, List<KPISaleCalculateResult> results)
        {
            var kpiIndexIds = results.Select(x => x.KpiIndexID).ToList();
            var oldResults = await _kpiSaleRepo.KPISaleResults
                .Where(x => x.EmployeeID == request.EmployeeID
                    && x.PeriodID == request.PeriodID
                    && kpiIndexIds.Contains(x.KpiIndexID))
                .ToListAsync();

            _kpiSaleRepo.KPISaleResults.RemoveRange(oldResults);

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
                CalculatedDate = now
            }).ToList();

            await _kpiSaleRepo.KPISaleResults.AddRangeAsync(newResults);
            await _kpiSaleRepo.SaveChangesAsync();
        }

        #endregion

        #region Validation and helper

        private async Task<KPISaleTarget> UpsertTargetAsync(
            KPISaleTargetUpsertRequest request,
            string currentUserName,
            KPISaleTarget? existing = null)
        {
            ValidateTargetRequest(request);

            var periodExists = await _kpiSaleRepo.KPISalePeriods.AnyAsync(x => x.ID == request.PeriodID);
            if (!periodExists)
                throw new Exception("Không tìm thấy kỳ KPI");

            var indexExists = await _kpiSaleRepo.KPISaleIndices.AnyAsync(x => x.ID == request.KpiIndexID);
            if (!indexExists)
                throw new Exception("Không tìm thấy KPI index");

            existing ??= await FindTargetAsync(request);

            if (existing == null || existing.ID <= 0)
            {
                var model = new KPISaleTarget
                {
                    EmployeeID = request.EmployeeID,
                    PeriodID = request.PeriodID,
                    KpiIndexID = request.KpiIndexID,
                    GoalValue = request.GoalValue,
                    CreatedBy = currentUserName,
                    CreatedDate = DateTime.Now
                };
                await _kpiSaleRepo.KPISaleTargets.AddAsync(model);
                return model;
            }

            existing.GoalValue = request.GoalValue;
            existing.UpdatedBy = currentUserName;
            existing.UpdatedDate = DateTime.Now;
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

            if (model.ParentID.HasValue)
            {
                if (model.ParentID.Value == model.ID && model.ID > 0)
                    throw new Exception("ParentID không được trùng với ID");

                var parentExists = await _kpiSaleRepo.KPISaleIndices.AnyAsync(x =>
                    x.ID == model.ParentID.Value && x.TemplateID == model.TemplateID);
                if (!parentExists)
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

            var maxAchievedPercent = scoringRule?.MaxAchievedPercent ?? 100;
            var cappedPercent = Math.Min(achievedPercent, maxAchievedPercent);
            return Math.Round(cappedPercent * weightPercent / 100, 4);
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
            List<KPISaleMappingFilterCondition> conditions)
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
                    .Cast<object>()
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
            public Dictionary<int, decimal> TargetsByIndex { get; set; } = new();
            public Dictionary<int, KPISaleScoringRule> ScoringRulesByIndex { get; set; } = new();
            public Dictionary<int, decimal> CalculatedValues { get; } = new();
            public HashSet<int> Visiting { get; } = new();
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

        #endregion
    }
}
