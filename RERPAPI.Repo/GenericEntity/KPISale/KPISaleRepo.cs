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
        public DbSet<KPISaleScoringRule> KPISaleScoringRules => _context.KPISaleScoringRules;
        public DbSet<KPISaleSystemParameter> KPISaleSystemParameters => _context.KPISaleSystemParameters;
        public DbSet<KPISaleTarget> KPISaleTargets => _context.KPISaleTargets;
        public DbSet<KPISaleTemplate> KPISaleTemplates => _context.KPISaleTemplates;

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
            string? displayColumn = null)
        {
            var result = new List<KPISaleLookupValue>();
            var connection = _context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            var qualifiedTable = $"[{schemaName.Trim()}].[{tableName.Trim()}]";
            var displayColName = string.IsNullOrWhiteSpace(displayColumn) ? valueColumn : displayColumn;
            command.CommandText = $"SELECT DISTINCT TOP 500 [{valueColumn.Trim()}] AS [Value], [{displayColName.Trim()}] AS [Display] FROM {qualifiedTable} WHERE [{valueColumn.Trim()}] IS NOT NULL ORDER BY [{displayColName.Trim()}]";
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