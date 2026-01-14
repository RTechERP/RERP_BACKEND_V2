using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace RERPAPI.Model.Common
{
    public static class SqlDapper<T> where T : class, new()
    {
        static string connectionString = Config.ConnectionString;
        static int commandTimeout = 2000;
        public static async Task<object> ProcedureToListAsync(string procedureName,object param)
        {
            try
            {
                var connection = new SqlConnection(connectionString);
                //var param = new { Keyword = keyword, UserID = _currentUser.ID };
                var data = await connection.QueryMultipleAsync(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);

                //var menus = (await data.ReadAsync()).ToList();
                //var userGroups = (await data.ReadAsync()).ToList();
                var result = (await data.ReadAsync()).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        public static async Task<List<T>> ProcedureToListTAsync(string procedureName, object param)
        {
            try
            {
                var connection = new SqlConnection(connectionString);
                //var param = new { Keyword = keyword, UserID = _currentUser.ID };
                var data = await connection.QueryMultipleAsync(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);

                //var menus = (await data.ReadAsync()).ToList();
                //var userGroups = (await data.ReadAsync()).ToList();
                var result = (await data.ReadAsync<T>()).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
