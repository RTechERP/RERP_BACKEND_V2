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


        //TN.Bình update 01/08/25
        public static void ExcuteProcedure(string storeProcedureName, string[] paramName, object[] paramValue)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            try
            {
                cn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(storeProcedureName, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                SqlParameter sqlParam;
                cn.Open();
                if (paramName != null)
                {
                    for (int i = 0; i < paramName.Length; i++)
                    {
                        sqlParam = new SqlParameter(paramName[i], paramValue[i]);
                        cmd.Parameters.Add(sqlParam);
                    }
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cn.Close();
            }

        }

        public static object ExcuteScalar(string procedureName, string[] paramName, object[] paramValue)
        {
            object value = null;
            SqlConnection mySqlConnection = new SqlConnection(connectionString);
            try
            {

                SqlParameter sqlParam;
                mySqlConnection.Open();

                SqlCommand mySqlCommand = new SqlCommand(procedureName, mySqlConnection);
                mySqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter mySqlDataAdapter = new SqlDataAdapter(mySqlCommand);

                if (paramName != null)
                {
                    for (int i = 0; i < paramName.Length; i++)
                    {
                        sqlParam = new SqlParameter(paramName[i], paramValue[i]);
                        mySqlCommand.Parameters.Add(sqlParam);
                    }
                }

                value = mySqlCommand.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                mySqlConnection.Close();
            }

            return value;
        }


        //end update

        public static List<T> FindByAttribute(string fieldName, object fieldValue)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            List<T> lst = new List<T>();
            T model = new T();
            Type type = model.GetType();
            string tableName = type.Name.StartsWith("Model") ? type.Name : type.Name.Replace("Model", "");
            try
            {
                string sql = $"SELECT * FROM {tableName} with (nolock)  WHERE {fieldName} = {fieldValue}";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = commandTimeout;
                cmd.CommandType = CommandType.Text;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                lst = reader.MapToList<T>();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return lst;
        }
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
                                        expando[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
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
                //return resultLists;
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



        public static List<T> FindAll()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            List<T> lst = new List<T>();
            T model = new T();
            Type type = model.GetType();
            string tableName = type.Name.StartsWith("Model") ? type.Name : type.Name.Replace("Model", "");
            try
            {
                string sql = string.Format("SELECT * FROM [{0}] with (nolock)", tableName);
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = 2000;
                cmd.CommandType = CommandType.Text;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                lst = reader.MapToList<T>();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return lst;
        }
    
/*   public static List<T> ProcedureToList(string procedureName, string[] paramName, object[] paramValue)
   {
       List<T> lst = new List<T>();
       SqlConnection mySqlConnection = new SqlConnection(connectionString);
       SqlParameter sqlParam;
       mySqlConnection.Open();

   public static List<T> FindByExpression(Expression exp)
   {
       SqlConnection conn = new SqlConnection(connectionString);
       List<T> lst = new List<T>();
       T model = new T();
       Type type = model.GetType();
       string tableName = type.Name.StartsWith("Model") ? type.Name : type.Name.Replace("Model", "");
       try
       {
           string sql = DBUtils.SQLSelect(tableName, exp);
           SqlCommand cmd = new SqlCommand(sql, conn);
           ////cmd.CommandTimeout = Timeout;
           cmd.CommandType = CommandType.Text;
           conn.Open();
           SqlDataReader reader = cmd.ExecuteReader();
           lst = reader.MapToList<T>();
           reader.Close();
       }
       catch (Exception e)
       {
           throw new Exception(e.Message);
       }
       finally
       {
           conn.Close();
       }

       return lst;
   }
*/
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
   //public static List<T> FindAll()
   //{
   //    SqlConnection conn = new SqlConnection(connectionString);
   //    List<T> lst = new List<T>();
   //    T model = new T();
   //    Type type = model.GetType();
   //    string tableName = type.Name.StartsWith("Model") ? type.Name : type.Name.Replace("Model", "");
   //    try
   //    {
   //        string sql = string.Format("SELECT * FROM [{0}] with (nolock)", tableName);
   //        SqlCommand cmd = new SqlCommand(sql, conn);
   //        cmd.CommandTimeout = 2000;
   //        cmd.CommandType = CommandType.Text;
   //        conn.Open();
   //        SqlDataReader reader = cmd.ExecuteReader();
   //        lst = reader.MapToList<T>();
   //    }
   //    catch (Exception e)
   //    {
   //        throw new Exception(e.ToString());
   //    }
   //    finally
   //    {
   //        conn.Close();
   //    }

   //    return lst;
   //}

   //public static List<T> FindByAttribute(string fieldName, object fieldValue)
   //{

   //    SqlConnection conn = new SqlConnection(connectionString);
   //    List<T> lst = new List<T>();
   //    T model = new T();
   //    Type type = model.GetType();
   //    string tableName = type.Name.StartsWith("Model") ? type.Name : type.Name.Replace("Model", "");
   //    try
   //    {
   //        string sql = DBUtils.SQLSelect(tableName, fieldName, fieldValue);
   //        //string sql = $"SELECT * FROM {tableName} with (nolock)  WHERE {fieldName} = {fieldValue}";

   //        SqlCommand cmd = new SqlCommand(sql, conn);
   //        cmd.CommandTimeout = 2000;
   //        cmd.CommandType = CommandType.Text;
   //        conn.Open();
   //        SqlDataReader reader = cmd.ExecuteReader();
   //        lst = reader.MapToList<T>();
   //    }
   //    catch (Exception e)
   //    {
   //        throw new Exception(e.ToString());
   //    }
   //    finally
   //    {
   //        conn.Close();
   //    }

   //    return lst;
   //}
}
}