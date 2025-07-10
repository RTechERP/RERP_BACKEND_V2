using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Common
{
    public static class SQLHelper<T> where T : class, new()
    {
        static string connectionString = Config.ConnectionString;
        static int commandTimeout = 2000;
        public static T ProcedureToModel(string procedureName, string[] paramName, object[] paramValue)
        {
            T model = new T();
            SqlConnection mySqlConnection = new SqlConnection(connectionString);
            SqlParameter sqlParam;
            mySqlConnection.Open();

            try
            {
                SqlCommand mySqlCommand = new SqlCommand(procedureName, mySqlConnection);
                mySqlCommand.CommandType = CommandType.StoredProcedure;
                if (paramName != null)
                {
                    for (int i = 0; i < paramName.Length; i++)
                    {
                        sqlParam = new SqlParameter(paramName[i], paramValue[i]);
                        mySqlCommand.Parameters.Add(sqlParam);
                    }
                }
                SqlDataReader reader = mySqlCommand.ExecuteReader();
                model = reader.MapToSingle<T>();
            }
            catch (SqlException e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                mySqlConnection.Close();
            }

            return model;
        }
        //public static List<T> ProcedureToList(string procedureName, string[] paramName, object[] paramValue)
        //{
        //    List<T> lst = new List<T>();
        //    SqlConnection mySqlConnection = new SqlConnection(connectionString);
        //    SqlParameter sqlParam;
        //    mySqlConnection.Open();

        //    try
        //    {
        //        SqlCommand mySqlCommand = new SqlCommand(procedureName, mySqlConnection);
        //        mySqlCommand.CommandType = CommandType.StoredProcedure;
        //        mySqlCommand.CommandTimeout = commandTimeout;
        //        if (paramName != null)
        //        {
        //            for (int i = 0; i < paramName.Length; i++)
        //            {
        //                sqlParam = new SqlParameter(paramName[i], paramValue[i]);
        //                mySqlCommand.Parameters.Add(sqlParam);
        //            }
        //        }
        //        SqlDataReader reader = mySqlCommand.ExecuteReader();
        //        lst = reader.MapToList<T>();
        //    }
        //    catch (SqlException e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //    finally
        //    {
        //        mySqlConnection.Close();
        //    }

        //    return lst;
        //}


        public static List<List<dynamic>> ProcedureToList(string procedureName, string[] paramName, object[] paramValue)
        {
            List<List<dynamic>> resultLists = new List<List<dynamic>>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = commandTimeout;

                        if (paramName != null)
                        {
                            for (int i = 0; i < paramName.Length; i++)
                            {
                                cmd.Parameters.Add(new SqlParameter(paramName[i], paramValue[i]));
                            }
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);

                            foreach (DataTable table in ds.Tables)
                            {
                                List<dynamic> dynamicList = new List<dynamic>();

                                foreach (DataRow row in table.Rows)
                                {
                                    IDictionary<string, object> expando = new ExpandoObject();
                                    foreach (DataColumn col in table.Columns)
                                    {
                                        expando[col.ColumnName] = row[col] == DBNull.Value ? null: row[col];
                                    }
                                    dynamicList.Add(expando);
                                } 

                                resultLists.Add(dynamicList);
                            }
                        }
                    }
                }

                return resultLists;
            }
            catch (Exception ex)
            {
                return resultLists;
                throw new Exception(ex.Message);
            }
        }

        //public static List<List<dynamic>> ProcedureToList(string procedureName, string[] paramName, object[] paramValue)
        //{
        //    var resultLists = new List<List<dynamic>>();

        //    try
        //    {
        //        using (var conn = new SqlConnection(connectionString))
        //        using (var cmd = new SqlCommand(procedureName, conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.CommandTimeout = commandTimeout;

        //            if (paramName != null)
        //            {
        //                for (int i = 0; i < paramName.Length; i++)
        //                    cmd.Parameters.AddWithValue(paramName[i], paramValue[i] ?? DBNull.Value);
        //            }

        //            conn.Open();
        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                do
        //                {
        //                    if (!reader.HasRows)
        //                        continue;

        //                    var table = new List<dynamic>();
        //                    while (reader.Read())
        //                    {
        //                        IDictionary<string, object> row = new ExpandoObject();
        //                        for (int i = 0; i < reader.FieldCount; i++)
        //                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);

        //                        table.Add(row);
        //                    }

        //                    resultLists.Add(table);
        //                }
        //                while (reader.NextResult());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error executing stored procedure: " + ex.Message, ex);
        //    }

        //    return resultLists;
        //}



        public static List<dynamic> GetListData(List<List<dynamic>> dynamics, int tableIndex)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                list = dynamics[tableIndex];
                return list;
            }
            catch (Exception ex)
            {
                return list;
                throw new Exception(ex.Message);
            }
        }
    }
}
