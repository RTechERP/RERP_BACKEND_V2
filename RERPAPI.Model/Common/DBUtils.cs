using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Common
{
    public class DBUtils
    {
        public static string SQLSelect(string tableName, string field, object value)
        {
            return SQLSelect(tableName, new Expression(field, value));
        }

        public static string SQLSelect(string tableName, string field, object value, string op)
        {
            return SQLSelect(tableName, new Expression(field, value, op));
        }

        public static string SQLSelect(string tableName, Expression exp)
        {
            //string sql = "SELECT *, ROW_NUMBER() over (order by (select 1)) as STT FROM " + tableName + " with (nolock) ";
            string sql = "SELECT * FROM " + tableName + " with (nolock) ";
            if (exp != null)
                sql += " WHERE " + exp.ToString();
            return sql;
        }
    }
}
