using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data;
using System.Dynamic;
using System.Data.Common;

namespace RERPAPI.Model.Common
{
    public static class SqlDapper<T> where T : class, new()
    {
        static string connectionString = Config.ConnectionString;
        static int commandTimeout = 200;
        public static async Task<object> ProcedureToListAsync(string procedureName, object param)
        {
            //var connection = new SqlConnection(connectionString);
            //try
            //{
            //    connection.Open();
            //    var data = await connection.QueryMultipleAsync(procedureName, param, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: commandTimeout);
            //    var result = (await data.ReadAsync()).ToList();
            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    connection.Close();
            //}

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    //var storedProcedureName = "spGetProduct";
                    //var values = new { type = 6 };
                    var data = await connection.QueryMultipleAsync(procedureName, param, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: commandTimeout);
                    //results.ForEach(r => textBox1.Text += $"{r.Code} {r.Name}" + Environment.NewLine);
                    var result = (await data.ReadAsync()).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                //return resultLists;
                throw new Exception(ex.Message);
            }
        }


        public static async Task<List<T>> ProcedureToListTAsync(string procedureName, object param)
        {
            //var connection = new SqlConnection(connectionString);
            //try
            //{
            //    connection.Open();
            //    var data = await connection.QueryMultipleAsync(procedureName, param, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: commandTimeout);
            //    var result = (await data.ReadAsync<T>()).ToList();
            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    connection.Close();
            //}

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    //var storedProcedureName = "spGetProduct";
                    //var values = new { type = 6 };
                    var data = await connection.QueryMultipleAsync(procedureName, param, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: commandTimeout);
                    //results.ForEach(r => textBox1.Text += $"{r.Code} {r.Name}" + Environment.NewLine);
                    var result = (await data.ReadAsync<T>()).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                //return resultLists;
                throw new Exception(ex.Message);
            }
        }

        public static async Task<(List<T1>, List<T2>)> QueryMultipleAsync<T1, T2>(
         string procedureName,
         object? parameters = null,
         IDbTransaction? transaction = null)
        {
            var connection = new SqlConnection(connectionString);
            using var multi = await connection.QueryMultipleAsync(
                procedureName,
                parameters,
                transaction,
                commandType: CommandType.StoredProcedure);

            return (
                multi.Read<T1>().AsList(),
                multi.Read<T2>().AsList()
            );
        }
        public static async Task<int> ExecuteStoredProcedure(
        string procedureName,
        object parameters = null,
        int? commandTimeout = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return await connection.ExecuteAsync(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: commandTimeout
                );
            }
        }
        public static async Task<T> ExecuteScalarStoredProcedure<T>(
        string procedureName,
        object parameters = null,
        int? commandTimeout = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                return await connection.ExecuteScalarAsync<T>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: commandTimeout
                );
            }
        }


    }
}
