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
    }
}
