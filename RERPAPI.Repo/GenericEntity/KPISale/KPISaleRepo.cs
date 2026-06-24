using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Data;
using System.Globalization;

namespace RERPAPI.Repo.GenericEntity.KPISale
{
    public class KPISaleRepo
    {
        private readonly RTCContext _context;

        public KPISaleRepo(RTCContext context, CurrentUser currentUser)
        {
            _context = context;
            _context.CurrentUser = currentUser;
        }

        public DbSet<KPISaleAllowedColumn> KPISaleAllowedColumns => _context.KPISaleAllowedColumns;
        public DbSet<KPISaleAllowedTable> KPISaleAllowedTables => _context.KPISaleAllowedTables;
        public DbSet<KPISaleDataSource> KPISaleDataSources => _context.KPISaleDataSources;
        public DbSet<KPISaleIndex> KPISaleIndices => _context.KPISaleIndices;
        public DbSet<KPISaleIndexDataMapping> KPISaleIndexDataMappings => _context.KPISaleIndexDataMappings;
        public DbSet<KPISaleIndexFormulaItem> KPISaleIndexFormulaItems => _context.KPISaleIndexFormulaItems;
        public DbSet<KPISaleMappingFilterCondition> KPISaleMappingFilterConditions => _context.KPISaleMappingFilterConditions;
        public DbSet<KPISaleMappingFilterGroup> KPISaleMappingFilterGroups => _context.KPISaleMappingFilterGroups;
        public DbSet<KPISalePeriod> KPISalePeriods => _context.KPISalePeriods;
        public DbSet<KPISaleResult> KPISaleResults => _context.KPISaleResults;
        public DbSet<KPISaleTotalPerformance> KPISaleTotalPerformances => _context.KPISaleTotalPerformances;
        public DbSet<KPISaleScoringRule> KPISaleScoringRules => _context.KPISaleScoringRules;
        //public DbSet<KPISaleReportMetricConfig> KPISaleReportMetricConfigs => _context.KPISaleReportMetricConfigs;
        //public DbSet<KPISaleReportMetricAssignment> KPISaleReportMetricAssignments => _context.KPISaleReportMetricAssignments;
        //public DbSet<KPISaleReportMonthlyInput> KPISaleReportMonthlyInputs => _context.KPISaleReportMonthlyInputs;
        public DbSet<KPISaleSystemParameter> KPISaleSystemParameters => _context.KPISaleSystemParameters;
        public DbSet<KPISaleTarget> KPISaleTargets => _context.KPISaleTargets;
        public DbSet<KPISaleTemplate> KPISaleTemplates => _context.KPISaleTemplates;
        public DbSet<KPISaleTeam> KPISaleTeams => _context.KPISaleTeams;
        public DbSet<KPISaleTeamMember> KPISaleTeamMembers => _context.KPISaleTeamMembers;
        public DbSet<KPISaleRewardConfig> KPISaleRewardConfig => _context.KPISaleRewardConfigs;
        public DbSet<KPISaleRewardCoefficient> KPISaleRewardCoefficient => _context.KPISaleRewardCoefficients;
        public DbSet<KPISaleEmployeeRewardMapping> KPISaleEmployeeRewardMapping => _context.KPISaleEmployeeRewardMappings;
        public DbSet<KPISaleRankingResult> KPISaleRankingResult => _context.KPISaleRankingResults;
        public DbSet<KPISaleEmployeeTemplate> KPISaleEmployeeTemplates => _context.KPISaleEmployeeTemplates;
        public DbSet<KPISaleTeamTemplate> KPISaleTeamTemplates => _context.KPISaleTeamTemplates;

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<decimal> ExecuteScalarDecimalAsync(
            string sql,
            IEnumerable<KPISaleSqlParameter> parameters)
        {
            var connection = _context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            foreach (var parameterValue in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterValue.Name;
                parameter.Value = parameterValue.Value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                return 0;

            return Convert.ToDecimal(result, CultureInfo.InvariantCulture);
        }

        public async Task<List<KPISaleLookupValue>> GetUniqueValuesAsync(
            string schemaName,
            string tableName,
            string valueColumn,
            string? displayColumn = null,
            string? preFilterColumn = null,
            string? preFilterOperator = null,
            string? preFilterValue = null,
            string? preFilterValue2 = null,
            Dictionary<string, object>? systemParams = null)
        {
            var result = new List<KPISaleLookupValue>();
            var connection = _context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            var qualifiedTable = $"[{schemaName.Trim()}].[{tableName.Trim()}]";
            var displayColName = string.IsNullOrWhiteSpace(displayColumn) ? valueColumn : displayColumn;

            var whereClauses = new List<string> { $"[{valueColumn.Trim()}] IS NOT NULL" };

            if (!string.IsNullOrWhiteSpace(preFilterColumn) && !string.IsNullOrWhiteSpace(preFilterOperator))
            {
                var col = $"[{preFilterColumn.Trim()}]";
                switch (preFilterOperator.Trim().ToUpperInvariant())
                {
                    case "=":
                    case ">":
                    case ">=":
                    case "<":
                    case "<=":
                    case "<>":
                        if (!string.IsNullOrWhiteSpace(preFilterValue))
                        {
                            if (preFilterValue.StartsWith("{{") && preFilterValue.EndsWith("}}"))
                            {
                                var paramKey = preFilterValue.Substring(2, preFilterValue.Length - 4).Trim();
                                whereClauses.Add($"{col} {preFilterOperator} {{{paramKey}}}");
                            }
                            else
                            {
                                whereClauses.Add($"{col} {preFilterOperator} '{preFilterValue.Replace("'", "''")}'");
                            }
                        }
                        break;
                    case "LIKE":
                        if (!string.IsNullOrWhiteSpace(preFilterValue))
                        {
                            whereClauses.Add($"{col} LIKE '%{preFilterValue.Replace("'", "''")}%'");
                        }
                        break;
                    case "IN":
                        if (!string.IsNullOrWhiteSpace(preFilterValue))
                        {
                            var inValues = preFilterValue.Split(',').Select(v => $"'{v.Trim().Replace("'", "''")}'");
                            whereClauses.Add($"{col} IN ({string.Join(", ", inValues)})");
                        }
                        break;
                    case "BETWEEN":
                        if (!string.IsNullOrWhiteSpace(preFilterValue) && !string.IsNullOrWhiteSpace(preFilterValue2))
                        {
                            whereClauses.Add($"{col} BETWEEN '{preFilterValue.Replace("'", "''")}' AND '{preFilterValue2.Replace("'", "''")}'");
                        }
                        break;
                    case "IS NULL":
                        whereClauses.Add($"{col} IS NULL");
                        break;
                    case "IS NOT NULL":
                        whereClauses.Add($"{col} IS NOT NULL");
                        break;
                }
            }

            command.CommandText = $"SELECT DISTINCT TOP 500 [{valueColumn.Trim()}] AS [Value], [{displayColName.Trim()}] AS [Display] FROM {qualifiedTable} WHERE {string.Join(" AND ", whereClauses)} ORDER BY [{displayColName.Trim()}]";
            command.CommandType = CommandType.Text;

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var val = reader.GetValue(0);
                var disp = reader.GetValue(1);
                if (val != null && val != DBNull.Value)
                {
                    result.Add(new KPISaleLookupValue
                    {
                        Value = val.ToString() ?? "",
                        Display = (disp != null && disp != DBNull.Value) ? disp.ToString() ?? "" : val.ToString() ?? ""
                    });
                }
            }
            return result;
        }

        public async Task<string?> GetLookupDisplayValueAsync(
            string schemaName,
            string tableName,
            string valueColumn,
            string displayColumn,
            string value)
        {
            var connection = _context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            var qualifiedTable = $"[{schemaName.Trim()}].[{tableName.Trim()}]";
            command.CommandText = $"SELECT TOP 1 [{displayColumn.Trim()}] FROM {qualifiedTable} WHERE [{valueColumn.Trim()}] = @Value";
            command.CommandType = CommandType.Text;

            var param = command.CreateParameter();
            param.ParameterName = "@Value";
            param.Value = value;
            command.Parameters.Add(param);

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                return null;
            return result.ToString();
        }
    }

    public class KPISaleLookupValue
    {
        public string Value { get; set; } = null!;
        public string Display { get; set; } = null!;
    }

    public sealed class KPISaleSqlParameter
    {
        public KPISaleSqlParameter(string name, object? value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object? Value { get; }
    }
}