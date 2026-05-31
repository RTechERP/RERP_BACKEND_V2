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
